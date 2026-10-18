using Fortaleza.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Fortaleza.Api.Games;

public static class GameEndpoints
{
    private const int MaxCommandLength = 200;

    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        var games = app.MapGroup("/api/games");

        games.MapPost("/", StartGame);
        games.MapGet("/{id:guid}", GetGame);
        games.MapDelete("/{id:guid}", EndGame);
        games.MapPost("/{id:guid}/commands", SendCommand);
        games.MapGet("/{id:guid}/track", DownloadTrack);
        games.MapPut("/{id:guid}/track", UploadTrack);

        return app;
    }

    private static IResult StartGame(StartGameRequest request, GameSessionStore store)
    {
        var errors = new Dictionary<string, string[]>();
        if (request.Game is not (1 or 2))
            errors[nameof(request.Game)] = ["Game must be 1 or 2."];
        if (request.Language is not ("es" or "en"))
            errors[nameof(request.Language)] = ["Language must be 'es' or 'en'."];
        if (errors.Count > 0)
            return Results.ValidationProblem(errors);

        var language = request.Language == "en" ? Language.English : Language.Spanish;
        var session = store.Create(request.Game, language);
        if (session is null)
            return Results.Problem("The server is full. Try again later.", statusCode: StatusCodes.Status503ServiceUnavailable);

        return Results.Ok(session.Begin());
    }

    private static IResult GetGame(Guid id, GameSessionStore store)
    {
        var session = store.Find(id);
        return session is null
            ? NotFound()
            : Results.Ok(new GameResponse(session.Id, session.Game, session.LanguageCode, session.Status, []));
    }

    private static IResult EndGame(Guid id, GameSessionStore store)
    {
        store.Remove(id);
        return Results.NoContent();
    }

    private static IResult SendCommand(Guid id, CommandRequest request, GameSessionStore store)
    {
        var command = request.Command?.Trim() ?? "";
        if (command == "" || command.Length > MaxCommandLength || command.Any(char.IsControl))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Command)] = [$"Command must be 1 to {MaxCommandLength} printable characters."]
            });

        var session = store.Find(id);
        return session is null ? NotFound() : Results.Ok(session.Execute(command));
    }

    private static IResult DownloadTrack(Guid id, GameSessionStore store)
    {
        var session = store.Find(id);
        if (session is null)
            return NotFound();

        var track = session.Track();
        if (track.Count == 0)
            return Results.Problem("There is nothing to save yet.", statusCode: StatusCodes.Status409Conflict);

        return Results.File(TrackFile.Encode(track), "text/plain", $"FORT{session.Game}_{session.LanguageCode.ToUpperInvariant()}.TRK");
    }

    private static async Task<IResult> UploadTrack(Guid id, HttpRequest request, GameSessionStore store, CancellationToken cancellationToken)
    {
        var session = store.Find(id);
        if (session is null)
            return NotFound();

        if (request.ContentLength > TrackFile.MaxBytes)
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);

        using var buffer = new MemoryStream();
        await request.Body.CopyToAsync(buffer, cancellationToken);
        if (buffer.Length > TrackFile.MaxBytes)
            return Results.Problem("The track file is too large.", statusCode: StatusCodes.Status413PayloadTooLarge);

        if (!TrackFile.TryParse(buffer.ToArray(), out var commands, out var error))
            return Results.Problem(error, statusCode: StatusCodes.Status400BadRequest);

        return Results.Ok(session.Restart(commands));
    }

    private static IResult NotFound() =>
        Results.Problem("The game was not found or has expired.", statusCode: StatusCodes.Status404NotFound);
}
