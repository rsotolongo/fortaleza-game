namespace Fortaleza.Api.Games;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
internal sealed record StartGameRequest(int Game, string Language)
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();
}
