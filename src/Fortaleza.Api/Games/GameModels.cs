namespace Fortaleza.Api.Games;

public record StartGameRequest(int Game, string Language);

public record CommandRequest(string Command);

public record GameResponse(Guid Id, int Game, string Language, string Status, IReadOnlyList<string> Output);
