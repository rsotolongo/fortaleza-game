using System.Net;
using System.Net.Http.Json;
using Fortaleza.Api.Games;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fortaleza.Api.Tests;

public class GameApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<GameResponse> StartAsync(int game, string language)
    {
        var response = await _client.PostAsJsonAsync("/api/games", new StartGameRequest(game, language));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>())!;
    }

    private async Task<GameResponse> SendAsync(Guid id, string command)
    {
        var response = await _client.PostAsJsonAsync($"/api/games/{id}/commands", new CommandRequest(command));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>())!;
    }

    [Theory]
    [InlineData(1, "es", "Usted se encuentra en el exterior de la fortaleza.")]
    [InlineData(1, "en", "You are in the exterior of the fortress.")]
    [InlineData(2, "es", "Usted se encuentra en una habitación para huéspedes.")]
    public async Task StartGame_ValidRequest_ReturnsOpeningRoomInTheChosenLanguage(int game, string language, string expected)
    {
        var started = await StartAsync(game, language);

        Assert.Equal(GameStatus.Playing, started.Status);
        Assert.Equal(expected, started.Output[0]);
    }

    [Theory]
    [InlineData(3, "es")]
    [InlineData(1, "fr")]
    public async Task StartGame_InvalidRequest_ReturnsBadRequest(int game, string language)
    {
        var response = await _client.PostAsJsonAsync("/api/games", new StartGameRequest(game, language));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1, "es", "FORT1_ES.TRK")]
    [InlineData(1, "en", "FORT1_EN.TRK")]
    [InlineData(2, "es", "FORT2_ES.TRK")]
    [InlineData(2, "en", "FORT2_EN.TRK")]
    public async Task UploadTrack_MinimalPath_WinsTheGame(int game, string language, string file)
    {
        var started = await StartAsync(game, language);
        var bytes = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "saves", file));

        var response = await _client.PutAsync($"/api/games/{started.Id}/track", new ByteArrayContent(bytes));

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.Equal(GameStatus.Won, result!.Status);
    }

    [Fact]
    public async Task SendCommand_AfterWinning_ReportsTheGameIsOver()
    {
        var started = await StartAsync(1, "es");
        var bytes = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory, "saves", "FORT1_ES.TRK"));
        await _client.PutAsync($"/api/games/{started.Id}/track", new ByteArrayContent(bytes));

        var result = await SendAsync(started.Id, "mirar");

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(["La partida ha terminado. Inicie una nueva."], result.Output);
    }

    [Fact]
    public async Task DownloadTrack_AfterCommands_ReturnsThemWithoutAccents()
    {
        var started = await StartAsync(1, "es");
        await SendAsync(started.Id, "tomar pastel");
        await SendAsync(started.Id, "ir túnel");

        var response = await _client.GetAsync($"/api/games/{started.Id}/track");

        response.EnsureSuccessStatusCode();
        Assert.Equal("FORT1_ES.TRK", response.Content.Headers.ContentDisposition!.FileName);
        Assert.Equal("tomar pastel\r\nir tunel\r\n", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DownloadTrack_NothingPlayedYet_ReturnsConflict()
    {
        var started = await StartAsync(1, "es");

        var response = await _client.GetAsync($"/api/games/{started.Id}/track");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("salvar")]
    [InlineData("ejecutar")]
    [InlineData("save")]
    public async Task SendCommand_TrackCommands_AreNotPassedToTheConsole(string command)
    {
        var started = await StartAsync(1, "es");

        var result = await SendAsync(started.Id, command);

        Assert.Equal(["Use los botones Guardar y Cargar para manejar el rastro."], result.Output);
    }

    [Fact]
    public async Task UploadTrack_ContainingExecuteCommand_ReturnsBadRequest()
    {
        var started = await StartAsync(1, "es");

        var response = await _client.PutAsync($"/api/games/{started.Id}/track", new StringContent("tomar pastel\r\nejecutar\r\n"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadTrack_LegacyCodePageFile_IsDecoded()
    {
        var started = await StartAsync(1, "es");
        var bytes = new byte[] { (byte)'i', (byte)'r', (byte)' ', (byte)'t', 0xA3, (byte)'n', (byte)'e', (byte)'l', 0x0D, 0x0A };

        var response = await _client.PutAsync($"/api/games/{started.Id}/track", new ByteArrayContent(bytes));

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.Contains(result!.Output, line => line.Contains("El olor de la carne descompuesta"));
    }

    [Fact]
    public async Task SendCommand_UnknownGame_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync($"/api/games/{Guid.NewGuid()}/commands", new CommandRequest("mirar"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendCommand_TwoPlayers_HaveIndependentState()
    {
        var first = await StartAsync(1, "es");
        var second = await StartAsync(1, "es");
        await SendAsync(first.Id, "tomar pastel");

        var firstBag = await SendAsync(first.Id, "inventario");
        var secondBag = await SendAsync(second.Id, "inventario");
        var secondTake = await SendAsync(second.Id, "tomar pastel");

        Assert.Contains("Pastel de cerezas (2)", firstBag.Output);
        Assert.Equal(["Usted no lleva nada consigo."], secondBag.Output);
        Assert.Equal(["Ok."], secondTake.Output);
    }

    [Fact]
    public async Task SendCommand_ConcurrentSpanishAndEnglishPlayers_NeverMixLanguages()
    {
        var spanish = await StartAsync(1, "es");
        var english = await StartAsync(1, "en");

        var tasks = Enumerable.Range(0, 60).Select(async i =>
        {
            var id = i % 2 == 0 ? spanish.Id : english.Id;
            var result = await SendAsync(id, "mirar");
            return (Spanish: i % 2 == 0, Text: string.Join('\n', result.Output));
        });
        var results = await Task.WhenAll(tasks);

        Assert.All(results.Where(r => r.Spanish), r => Assert.Contains("Usted se encuentra", r.Text));
        Assert.All(results.Where(r => !r.Spanish), r => Assert.Contains("You are in", r.Text));
    }
}
