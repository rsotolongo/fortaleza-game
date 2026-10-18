using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Api.Games;

/// <summary>
/// Runs the game without keeping any server state: every call rebuilds the game and replays the
/// player's track (the list of commands so far), which fully determines the game state.
/// </summary>
public static class GameRunner
{
    private static readonly LocalizedText Finished = new(
        "La partida ha terminado. Inicie una nueva.",
        "The game is over. Start a new one.");

    private static readonly LocalizedText UseTrackButtons = new(
        "Use los botones Guardar y Cargar para manejar el rastro.",
        "Use the Save and Load buttons to manage the track.");

    public static string LanguageCode(Language language) => language == Language.English ? "en" : "es";

    public static bool IsPlayable(string command)
    {
        var type = CommandParser.Parse(command).Type;
        return type is not (CommandType.Exec or CommandType.Save);
    }

    public static GameResponse Start(int game, Language language) => Run(game, language, engine =>
    {
        engine.Start();
        engine.Player.Look();
    });

    public static GameResponse Play(int game, Language language, IReadOnlyList<string> track, string command) =>
        Run(game, language, engine =>
        {
            engine.Start();
            Silently(() => engine.ExecuteTrack(track, verbose: false));

            var status = "";
            Silently(() => status = Evaluate(engine));
            if (status != GameStatus.Playing)
            {
                engine.Player.Output(Finished.Get());
                return;
            }

            if (!IsPlayable(command))
            {
                engine.Player.Output(UseTrackButtons.Get());
                return;
            }

            engine.ProcessCommand(command);
            Evaluate(engine);
        });

    public static GameResponse Load(int game, Language language, IReadOnlyList<string> commands) =>
        Run(game, language, engine =>
        {
            engine.Start();
            engine.ExecuteTrack(commands);
            Evaluate(engine);
        });

    private static GameResponse Run(int game, Language language, Action<GameEngine> action)
    {
        var engine = game == 2 ? (GameEngine)Fort2Builder.Build() : Fort1Builder.Build();
        var writer = new StringWriter();
        var previousLanguage = Localizer.Current;
        var previousWriter = Thing.OutputWriter;
        var previousVisible = Thing.Visible;
        var previousWidth = Thing.LineWidth;
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
        var captured = Thing.OutputWriter;
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
            engine.Player.Output(Localizer.Msg("Defeat"));
            return GameStatus.Lost;
        }

        return GameStatus.Playing;
    }

    private static string StatusOf(GameEngine engine)
    {
        var previous = Thing.OutputWriter;
        Thing.OutputWriter = TextWriter.Null;
        try
        {
            return engine.CheckGoal() ? GameStatus.Won : engine.IsOver ? GameStatus.Lost : GameStatus.Playing;
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
            foreach (var line in Fort2.Escape.Concat(Fort2.EpilogueFirstPage).Concat(Fort2.EpilogueLastPage))
                engine.Player.Output(line.Get());
            return;
        }

        foreach (var key in new[] { "Victory1_Fort1", "Victory2_Fort1", "Victory3_Fort1", "Victory4_Fort1" })
            engine.Player.Output(Localizer.Msg(key));
    }

    private static string[] SplitLines(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n').ToList();
        while (lines.Count > 0 && lines[^1] == "")
            lines.RemoveAt(lines.Count - 1);
        return [.. lines];
    }
}
