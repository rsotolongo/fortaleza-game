using Fortaleza.Localization;

namespace Fortaleza.Api.Games;

public static class GameEndpoints
{
    private const int _maxCommandLength = 200;

    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder games = app.MapGroup("/api/games");

        games.MapPost("/start", Start);
        games.MapPost("/play", Play);
        games.MapPut("/load", LoadAsync);

        return app;
    }

    private static IResult Start(StartGameRequest request)
    {
        Dictionary<string, string[]> errors = Validate(request.Game, request.Language);
        return errors.Count > 0
            ? Results.ValidationProblem(errors)
            : Results.Ok(GameRunner.Start(request.Game, ToLanguage(request.Language)));
    }

    private static IResult Play(PlayRequest request)
    {
        Dictionary<string, string[]> errors = Validate(request.Game, request.Language);
        string command = request.Command?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(command) || command.Length > _maxCommandLength || command.Any(char.IsControl))
        {
            errors[nameof(request.Command)] = [$"Command must be 1 to {_maxCommandLength} printable characters."];
        }

        IReadOnlyList<string> track = request.Track ?? [];
        string trackError = TrackFile.Validate(track);
        if (!string.IsNullOrEmpty(trackError))
        {
            errors[nameof(request.Track)] = [trackError];
        }

        return errors.Count > 0
            ? Results.ValidationProblem(errors)
            : Results.Ok(GameRunner.Play(request.Game, ToLanguage(request.Language), track, command));
    }

    private static async Task<IResult> LoadAsync(int game, string language, HttpRequest request, CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> errors = Validate(game, language);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        if (request.ContentLength > TrackFile.MaxBytes)
        {
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        await using var buffer = new MemoryStream();
        await request.Body.CopyToAsync(buffer, cancellationToken);
        if (buffer.Length > TrackFile.MaxBytes)
        {
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        if (!TrackFile.TryParse(buffer.ToArray(), out IReadOnlyList<string>? commands, out string? error))
        {
            return Results.Problem(string.IsNullOrEmpty(error) ? "The track file is empty." : error, statusCode: StatusCodes.Status400BadRequest);
        }

        if (commands.Count is 0)
        {
            return Results.Problem("The track file is empty.", statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.Ok(GameRunner.Load(game, ToLanguage(language), commands));
    }

    private static Dictionary<string, string[]> Validate(int game, string language)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (game is not (1 or 2))
        {
            errors["Game"] = ["Game must be 1 or 2."];
        }

        if (language is not ("es" or "en"))
        {
            errors["Language"] = ["Language must be 'es' or 'en'."];
        }

        return errors;
    }

    private static Language ToLanguage(string language)
    {
        return language is "en" ? Language.English : Language.Spanish;
    }
}
