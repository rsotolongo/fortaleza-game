namespace Fortaleza.Api.Games;

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public record class StartGameRequest(int Game, string Language)
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();
}
