using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Fortaleza.Api.Games;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fortaleza.Api.Tests;

public sealed class GameApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData(1, "es", "Usted se encuentra en el exterior de la fortaleza.")]
    [InlineData(1, "en", "You are in the exterior of the fortress.")]
    [InlineData(2, "es", "Usted se encuentra en una habitación para huéspedes.")]
    public async Task StartValidRequestReturnsOpeningRoomAndEmptyTrackAsync(int game, string language, string expected)
    {
        GameResponse started = await StartAsync(game, language).ConfigureAwait(true);

        Assert.Equal(GameStatus.Playing, started.Status);
        Assert.Equal(expected, started.Output[0]);
        Assert.Empty(started.Track);
    }

    [Theory]
    [InlineData(3, "es")]
    [InlineData(1, "fr")]
    public async Task StartInvalidRequestReturnsBadRequestAsync(int game, string language)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/games/start", new StartGameRequest(game, language)).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1, "es", "FORT1_ES.TRK")]
    [InlineData(1, "en", "FORT1_EN.TRK")]
    [InlineData(2, "es", "FORT2_ES.TRK")]
    [InlineData(2, "en", "FORT2_EN.TRK")]
    public async Task PlayMinimalPathOneCommandPerRequestWinsOnTheLastCommandAsync(int game, string language, string file)
    {
        var commands = (await File.ReadAllLinesAsync(SavePath(file)).ConfigureAwait(true)).Where(static line => !string.IsNullOrEmpty(line)).ToList();
        IReadOnlyList<string> track = [];
        var statuses = new List<string>();

        foreach (string? command in commands)
        {
            GameResponse response = await PlayAsync(game, language, track, command).ConfigureAwait(true);
            track = response.Track;
            statuses.Add(response.Status);
        }

        Assert.Equal(commands, track);
        Assert.All(statuses.SkipLast(1), static status => Assert.Equal(GameStatus.Playing, status));
        Assert.Equal(GameStatus.Won, statuses[^1]);
    }

    [Theory]
    [InlineData(1, "es", "FORT1_ES.TRK")]
    [InlineData(1, "en", "FORT1_EN.TRK")]
    [InlineData(2, "es", "FORT2_ES.TRK")]
    [InlineData(2, "en", "FORT2_EN.TRK")]
    public async Task LoadMinimalPathWinsTheGameAndReturnsTheTrackAsync(int game, string language, string file)
    {
        HttpResponseMessage response = await LoadAsync(game, language, await File.ReadAllBytesAsync(SavePath(file)).ConfigureAwait(true)).ConfigureAwait(true);

        response.EnsureSuccessStatusCode();
        GameResponse? result = await response.Content.ReadFromJsonAsync<GameResponse>().ConfigureAwait(true);
        Assert.Equal(GameStatus.Won, result!.Status);
        Assert.NotEmpty(result.Track);
    }

    [Fact]
    public async Task PlayNonTrackedCommandDoesNotGrowTheTrackAsync()
    {
        GameResponse first = await PlayAsync(1, "es", [], "tomar pastel").ConfigureAwait(true);

        GameResponse bag = await PlayAsync(1, "es", first.Track, "inventario").ConfigureAwait(true);

        Assert.Equal(["tomar pastel"], bag.Track);
        Assert.Contains("Pastel de cerezas (2)", bag.Output, StringComparer.Ordinal);
    }

    [Fact]
    public async Task PlayAfterWinningReportsTheGameIsOverAsync()
    {
        HttpResponseMessage loaded = await LoadAsync(1, "es", await File.ReadAllBytesAsync(SavePath("FORT1_ES.TRK")).ConfigureAwait(true)).ConfigureAwait(true);
        GameResponse? won = await loaded.Content.ReadFromJsonAsync<GameResponse>().ConfigureAwait(true);

        GameResponse result = await PlayAsync(1, "es", won!.Track, "mirar").ConfigureAwait(true);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(["La partida ha terminado. Inicie una nueva."], result.Output);
    }

    [Theory]
    [InlineData("salvar")]
    [InlineData("ejecutar")]
    [InlineData("save")]
    public async Task PlayTrackCommandsAreNotPassedToTheConsoleAsync(string command)
    {
        GameResponse result = await PlayAsync(1, "es", [], command).ConfigureAwait(true);

        Assert.Equal(["Use los botones Guardar y Cargar para manejar el rastro."], result.Output);
    }

    [Fact]
    public async Task PlayTrackContainingExecuteCommandReturnsBadRequestAsync()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(1, "es", ["tomar pastel", "ejecutar"], "mirar")).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PlayTrackTooLongReturnsBadRequestAsync()
    {
        var track = Enumerable.Repeat("mirar", TrackFile.MaxLines + 1).ToList();

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(1, "es", track, "mirar")).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoadContainingExecuteCommandReturnsBadRequestAsync()
    {
        HttpResponseMessage response = await LoadAsync(1, "es", "tomar pastel\r\nejecutar\r\n"u8.ToArray()).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoadEmptyFileReturnsBadRequestAsync()
    {
        HttpResponseMessage response = await LoadAsync(1, "es", []).ConfigureAwait(true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoadLegacyCodePageFileIsDecodedAsync()
    {
        byte[] bytes = [0x69, 0x72, 0x20, 0x74, 0xA3, 0x6E, 0x65, 0x6C, 0x0D, 0x0A];

        HttpResponseMessage response = await LoadAsync(1, "es", bytes).ConfigureAwait(true);

        response.EnsureSuccessStatusCode();
        GameResponse? result = await response.Content.ReadFromJsonAsync<GameResponse>().ConfigureAwait(true);
        Assert.Contains(result!.Output, static line => line.Contains("El olor de la carne descompuesta", StringComparison.Ordinal));
    }

    [Fact]
    public async Task PlayConcurrentSpanishAndEnglishPlayersNeverMixLanguagesAsync()
    {
        IEnumerable<Task<(bool Spanish, string Text)>> tasks = Enumerable.Range(0, 60).Select(async i =>
        {
            bool spanish = i % 2 is 0;
            GameResponse result = await PlayAsync(1, spanish ? "es" : "en", ["tomar pastel"], "mirar").ConfigureAwait(true);
            return (Spanish: spanish, Text: string.Join('\n', result.Output));
        });
        (bool Spanish, string Text)[] results = await Task.WhenAll(tasks).ConfigureAwait(true);

        Assert.All(results.Where(static r => r.Spanish), static r => Assert.Contains("Usted se encuentra", r.Text, StringComparison.Ordinal));
        Assert.All(results.Where(static r => !r.Spanish), static r => Assert.Contains("You are in", r.Text, StringComparison.Ordinal));
    }

    [Fact]
    public async Task PlayConcurrentPlayersWithDifferentTracksKeepIndependentStateAsync()
    {
        IEnumerable<Task<(bool Carrying, IReadOnlyList<string> Output)>> tasks = Enumerable.Range(0, 40).Select(async i =>
        {
            bool carrying = i % 2 is 0;
            GameResponse result = await PlayAsync(1, "es", carrying ? ["tomar pastel"] : [], "inventario").ConfigureAwait(true);
            return (Carrying: carrying, result.Output);
        });
        (bool Carrying, IReadOnlyList<string> Output)[] results = await Task.WhenAll(tasks).ConfigureAwait(true);

        Assert.All(results.Where(static r => r.Carrying), static r => Assert.Contains("Pastel de cerezas (2)", r.Output, StringComparer.Ordinal));
        Assert.All(results.Where(static r => !r.Carrying), static r => Assert.Equal(["Usted no lleva nada consigo."], r.Output));
    }

    private static string SavePath(string file)
    {
        return Path.Combine(AppContext.BaseDirectory, "saves", file);
    }

    private async Task<GameResponse> StartAsync(int game, string language)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/games/start", new StartGameRequest(game, language)).ConfigureAwait(true);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>().ConfigureAwait(true))!;
    }

    private async Task<GameResponse> PlayAsync(int game, string language, IReadOnlyList<string> track, string command)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/games/play", new PlayRequest(game, language, track, command)).ConfigureAwait(true);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<GameResponse>().ConfigureAwait(true))!;
    }

    private async Task<HttpResponseMessage> LoadAsync(int game, string language, byte[] bytes)
    {
        using ByteArrayContent content = new(bytes);
        return await _client.PutAsync(
            new Uri($"/api/games/load?game={game.ToString(CultureInfo.InvariantCulture)}&language={language}", UriKind.Relative),
            content).ConfigureAwait(true);
    }
}
