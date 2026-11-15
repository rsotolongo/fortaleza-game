namespace Fortaleza.Api.Games;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public record class PlayRequest(int Game, string Language, IReadOnlyList<string> Track, string Command)
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();
}
