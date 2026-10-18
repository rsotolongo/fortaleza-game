namespace Fortaleza.Api.Games;

public record StartGameRequest(int Game, string Language);

public record PlayRequest(int Game, string Language, IReadOnlyList<string> Track, string Command);

public record GameResponse(int Game, string Language, string Status, IReadOnlyList<string> Output, IReadOnlyList<string> Track);
