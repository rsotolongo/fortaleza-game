namespace Fortaleza.Api.Games;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public record class GameResponse(int Game, string Language, string Status, IReadOnlyList<string> Output, IReadOnlyList<string> Track)
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();
}
