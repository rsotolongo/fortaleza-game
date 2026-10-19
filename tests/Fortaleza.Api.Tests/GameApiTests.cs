using System.Net;
using System.Net.Http.Json;
using Fortaleza.Api.Games;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fortaleza.Api.Tests;

public class GameApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static string SavePath(string file) => Path.Combine(AppContext.BaseDirectory, "saves", file);

    private async Task<GameResponse> StartAsync(int game, string language)
    {
        var response = await _client.PostAsJsonAsync("/api/games/start", new StartGameRequest(game, language));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>())!;
    }

    private async Task<GameResponse> PlayAsync(int game, string language, IReadOnlyList<string> track, string command)
    {
        var response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(game, language, track, command));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>())!;
    }

    private Task<HttpResponseMessage> LoadAsync(int game, string language, byte[] bytes) =>
        _client.PutAsync($"/api/games/load?game={game}&language={language}", new ByteArrayContent(bytes));

    [Theory]
    [InlineData(1, "es", "Usted se encuentra en el exterior de la fortaleza.")]
    [InlineData(1, "en", "You are in the exterior of the fortress.")]
    [InlineData(2, "es", "Usted se encuentra en una habitación para huéspedes.")]
    public async Task Start_ValidRequest_ReturnsOpeningRoomAndEmptyTrack(int game, string language, string expected)
    {
        var started = await StartAsync(game, language);

        Assert.Equal(GameStatus.Playing, started.Status);
        Assert.Equal(expected, started.Output[0]);
        Assert.Empty(started.Track);
    }

    [Theory]
    [InlineData(3, "es")]
    [InlineData(1, "fr")]
    public async Task Start_InvalidRequest_ReturnsBadRequest(int game, string language)
    {
        var response = await _client.PostAsJsonAsync("/api/games/start", new StartGameRequest(game, language));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1, "es", "FORT1_ES.TRK")]
    [InlineData(1, "en", "FORT1_EN.TRK")]
    [InlineData(2, "es", "FORT2_ES.TRK")]
    [InlineData(2, "en", "FORT2_EN.TRK")]
    public async Task Play_MinimalPathOneCommandPerRequest_WinsOnTheLastCommand(int game, string language, string file)
    {
        var commands = (await File.ReadAllLinesAsync(SavePath(file))).Where(line => line != "").ToList();
        IReadOnlyList<string> track = [];
        var statuses = new List<string>();

        foreach (var command in commands)
        {
            var response = await PlayAsync(game, language, track, command);
            track = response.Track;
            statuses.Add(response.Status);
        }

        Assert.Equal(commands, track);
        Assert.All(statuses.SkipLast(1), status => Assert.Equal(GameStatus.Playing, status));
        Assert.Equal(GameStatus.Won, statuses[^1]);
    }

    [Theory]
    [InlineData(1, "es", "FORT1_ES.TRK")]
    [InlineData(1, "en", "FORT1_EN.TRK")]
    [InlineData(2, "es", "FORT2_ES.TRK")]
    [InlineData(2, "en", "FORT2_EN.TRK")]
    public async Task Load_MinimalPath_WinsTheGameAndReturnsTheTrack(int game, string language, string file)
    {
        var response = await LoadAsync(game, language, await File.ReadAllBytesAsync(SavePath(file)));

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.Equal(GameStatus.Won, result!.Status);
        Assert.NotEmpty(result.Track);
    }

    [Fact]
    public async Task Play_NonTrackedCommand_DoesNotGrowTheTrack()
    {
        var first = await PlayAsync(1, "es", [], "tomar pastel");

        var bag = await PlayAsync(1, "es", first.Track, "inventario");

        Assert.Equal(["tomar pastel"], bag.Track);
        Assert.Contains("Pastel de cerezas (2)", bag.Output);
    }

    [Fact]
    public async Task Play_AfterWinning_ReportsTheGameIsOver()
    {
        var loaded = await LoadAsync(1, "es", await File.ReadAllBytesAsync(SavePath("FORT1_ES.TRK")));
        var won = await loaded.Content.ReadFromJsonAsync<GameResponse>();

        var result = await PlayAsync(1, "es", won!.Track, "mirar");

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(["La partida ha terminado. Inicie una nueva."], result.Output);
    }

    [Theory]
    [InlineData("salvar")]
    [InlineData("ejecutar")]
    [InlineData("save")]
    public async Task Play_TrackCommands_AreNotPassedToTheConsole(string command)
    {
        var result = await PlayAsync(1, "es", [], command);

        Assert.Equal(["Use los botones Guardar y Cargar para manejar el rastro."], result.Output);
    }

    [Fact]
    public async Task Play_TrackContainingExecuteCommand_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(1, "es", ["tomar pastel", "ejecutar"], "mirar"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Play_TrackTooLong_ReturnsBadRequest()
    {
        var track = Enumerable.Repeat("mirar", TrackFile.MaxLines + 1).ToList();

        var response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(1, "es", track, "mirar"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Load_ContainingExecuteCommand_ReturnsBadRequest()
    {
        var response = await LoadAsync(1, "es", "tomar pastel\r\nejecutar\r\n"u8.ToArray());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Load_EmptyFile_ReturnsBadRequest()
    {
        var response = await LoadAsync(1, "es", []);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Load_LegacyCodePageFile_IsDecoded()
    {
        var bytes = new byte[] { (byte)'i', (byte)'r', (byte)' ', (byte)'t', 0xA3, (byte)'n', (byte)'e', (byte)'l', 0x0D, 0x0A };

        var response = await LoadAsync(1, "es", bytes);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameResponse>();
        Assert.Contains(result!.Output, line => line.Contains("El olor de la carne descompuesta"));
    }

    [Fact]
    public async Task Play_ConcurrentSpanishAndEnglishPlayers_NeverMixLanguages()
    {
        var tasks = Enumerable.Range(0, 60).Select(async i =>
        {
            var spanish = i % 2 == 0;
            var result = await PlayAsync(1, spanish ? "es" : "en", ["tomar pastel"], "mirar");
            return (Spanish: spanish, Text: string.Join('\n', result.Output));
        });
        var results = await Task.WhenAll(tasks);

        Assert.All(results.Where(r => r.Spanish), r => Assert.Contains("Usted se encuentra", r.Text));
        Assert.All(results.Where(r => !r.Spanish), r => Assert.Contains("You are in", r.Text));
    }

    [Fact]
    public async Task Play_ConcurrentPlayersWithDifferentTracks_KeepIndependentState()
    {
        var tasks = Enumerable.Range(0, 40).Select(async i =>
        {
            var carrying = i % 2 == 0;
            var result = await PlayAsync(1, "es", carrying ? ["tomar pastel"] : [], "inventario");
            return (Carrying: carrying, Output: result.Output);
        });
        var results = await Task.WhenAll(tasks);

        Assert.All(results.Where(r => r.Carrying), r => Assert.Contains("Pastel de cerezas (2)", r.Output));
        Assert.All(results.Where(r => !r.Carrying), r => Assert.Equal(["Usted no lleva nada consigo."], r.Output));
    }
}
