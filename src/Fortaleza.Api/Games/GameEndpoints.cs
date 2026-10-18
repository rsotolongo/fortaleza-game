using Fortaleza.Localization;

namespace Fortaleza.Api.Games;

public static class GameEndpoints
{
    private const int MaxCommandLength = 200;

    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var games = app.MapGroup("/api/games");

        games.MapPost("/start", Start);
        games.MapPost("/play", Play);
        games.MapPut("/load", Load);

        return app;
    }

    private static IResult Start(StartGameRequest request)
    {
        var errors = Validate(request.Game, request.Language);
        return errors.Count > 0
            ? Results.ValidationProblem(errors)
            : Results.Ok(GameRunner.Start(request.Game, ToLanguage(request.Language)));
    }

    private static IResult Play(PlayRequest request)
    {
        var errors = Validate(request.Game, request.Language);
        var command = request.Command?.Trim() ?? "";
        if (command == "" || command.Length > MaxCommandLength || command.Any(char.IsControl))
            errors[nameof(request.Command)] = [$"Command must be 1 to {MaxCommandLength} printable characters."];

        var track = request.Track ?? [];
        var trackError = TrackFile.Validate(track);
        if (trackError != "")
            errors[nameof(request.Track)] = [trackError];

        return errors.Count > 0
            ? Results.ValidationProblem(errors)
            : Results.Ok(GameRunner.Play(request.Game, ToLanguage(request.Language), track, command));
    }

    private static async Task<IResult> Load(int game, string language, HttpRequest request, CancellationToken cancellationToken)
    {
        var errors = Validate(game, language);
        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        if (request.ContentLength > TrackFile.MaxBytes)
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);

        using var buffer = new MemoryStream();
        await request.Body.CopyToAsync(buffer, cancellationToken);
        if (buffer.Length > TrackFile.MaxBytes)
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);

        if (!TrackFile.TryParse(buffer.ToArray(), out var commands, out var error))
            return Results.Problem(error == "" ? "The track file is empty." : error, statusCode: StatusCodes.Status400BadRequest);
        if (commands.Count == 0)
            return Results.Problem("The track file is empty.", statusCode: StatusCodes.Status400BadRequest);

        return Results.Ok(GameRunner.Load(game, ToLanguage(language), commands));
    }

    private static Dictionary<string, string[]> Validate(int game, string language)
    {
        var errors = new Dictionary<string, string[]>();
        if (game is not (1 or 2))
            errors["Game"] = ["Game must be 1 or 2."];
        if (language is not ("es" or "en"))
            errors["Language"] = ["Language must be 'es' or 'en'."];
        return errors;
    }

    private static Language ToLanguage(string language) => language == "en" ? Language.English : Language.Spanish;
}
