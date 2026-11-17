namespace Fortaleza.Api.Games;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
internal sealed record GameResponse(int Game, string Language, string Status, IReadOnlyList<string> Output, IReadOnlyList<string> Track)
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();
}
