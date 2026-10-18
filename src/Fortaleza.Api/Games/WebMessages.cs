using Fortaleza.Localization;

namespace Fortaleza.Api.Games;

public static class WebMessages
{
    private static readonly LocalizedText Finished = new(
        "La partida ha terminado. Inicie una nueva.",
        "The game is over. Start a new one.");

    private static readonly LocalizedText UseButtons = new(
        "Use los botones Guardar y Cargar para manejar el rastro.",
        "Use the Save and Load buttons to manage the track.");

    public static string GameFinished => Finished.Get();

    public static string UseTrackButtons => UseButtons.Get();
}
