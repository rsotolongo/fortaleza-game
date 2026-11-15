using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Api.Games;

/// <summary>
/// Runs the game without keeping any server state: every call rebuilds the game and replays the
/// player's track (the list of commands so far), which fully determines the game state.
/// </summary>
internal static class GameRunner
{
    private static readonly LocalizedText Finished = new(
        "La partida ha terminado. Inicie una nueva.",
        "The game is over. Start a new one.");

    private static readonly LocalizedText UseTrackButtons = new(
        "Use los botones Guardar y Cargar para manejar el rastro.",
        "Use the Save and Load buttons to manage the track.");

    public static string LanguageCode(Language language)
    {
        return language is Language.English ? "en" : "es";
    }

    public static bool IsPlayable(string command)
    {
        CommandType type = CommandParser.Parse(command).Type;
        return type is not (CommandType.Exec or CommandType.Save);
    }

    public static GameResponse Start(int game, Language language)
    {
        return Run(game, language, static engine =>
    {
        engine.Start();
        engine.Player.Look();
    });
    }

    public static GameResponse Play(int game, Language language, IReadOnlyList<string> track, string command)
    {
        return Run(game, language, engine =>
        {
            engine.Start();
            Silently(() => engine.ExecuteTrack(track, verbose: false));

            string status = string.Empty;
            Silently(() => status = Evaluate(engine));
            if (status != GameStatus.Playing)
            {
                Thing.Output(Finished.Get());
                return;
            }

            if (!IsPlayable(command))
            {
                Thing.Output(UseTrackButtons.Get());
                return;
            }

            engine.ProcessCommand(command);
            Evaluate(engine);
        });
    }

    public static GameResponse Load(int game, Language language, IReadOnlyList<string> commands)
    {
        return Run(game, language, engine =>
        {
            engine.Start();
            engine.ExecuteTrack(commands);
            Evaluate(engine);
        });
    }

    private static GameResponse Run(int game, Language language, Action<GameEngine> action)
    {
        GameEngine engine = game is 2 ? Fort2Builder.Build() : Fort1Builder.Build();
        var writer = new StringWriter();
        Language previousLanguage = Localizer.Current;
        TextWriter? previousWriter = Thing.OutputWriter;
        bool previousVisible = Thing.Visible;
        int previousWidth = Thing.LineWidth;
        Localizer.Current = language;
        Thing.OutputWriter = writer;
        Thing.Visible = true;
        Thing.LineWidth = int.MaxValue - 1;
        try
        {
            action(engine);
            return new GameResponse(game, LanguageCode(language), StatusOf(engine), SplitLines(writer.ToString()), [.. engine.Track]);
        }
        finally
        {
            Localizer.Current = previousLanguage;
            Thing.OutputWriter = previousWriter;
            Thing.Visible = previousVisible;
            Thing.LineWidth = previousWidth;
        }
    }

    private static void Silently(Action action)
    {
        TextWriter? captured = Thing.OutputWriter;
        Thing.OutputWriter = TextWriter.Null;
        try
        {
            action();
        }
        finally
        {
            Thing.OutputWriter = captured;
        }
    }

    private static string Evaluate(GameEngine engine)
    {
        if (engine.CheckGoal())
        {
            WriteEnding(engine);
            return GameStatus.Won;
        }

        if (engine.IsOver)
        {
            Thing.Output(Localizer.Msg("Defeat"));
            return GameStatus.Lost;
        }

        return GameStatus.Playing;
    }

    private static string StatusOf(GameEngine engine)
    {
        TextWriter? previous = Thing.OutputWriter;
        Thing.OutputWriter = TextWriter.Null;
        try
        {
            if (engine.CheckGoal())
            {
                return GameStatus.Won;
            }

            return engine.IsOver ? GameStatus.Lost : GameStatus.Playing;
        }
        finally
        {
            Thing.OutputWriter = previous;
        }
    }

    private static void WriteEnding(GameEngine engine)
    {
        if (engine is Fort2)
        {
            foreach (LocalizedText line in Fort2.Escape.Concat(Fort2.EpilogueFirstPage).Concat(Fort2.EpilogueLastPage))
            {
                Thing.Output(line.Get());
            }

            return;
        }

        foreach (string? key in new[] { "Victory1_Fort1", "Victory2_Fort1", "Victory3_Fort1", "Victory4_Fort1" })
        {
            Thing.Output(Localizer.Msg(key));
        }
    }

    private static string[] SplitLines(string text)
    {
        List<string> lines = [.. text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')];
        while (lines.Count > 0 && string.IsNullOrEmpty(lines[^1]))
        {
            lines.RemoveAt(lines.Count - 1);
        }

        return [.. lines];
    }
}
