using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Api.Games;

public sealed class GameSession(Guid id, int game, Language language, DateTimeOffset now)
{
    private readonly Lock _gate = new();
    private GameEngine _engine = Build(game);

    public Guid Id { get; } = id;
    public int Game { get; } = game;
    public Language Language { get; } = language;
    public string LanguageCode => Language == Language.English ? "en" : "es";
    public string Status { get; private set; } = GameStatus.Playing;
    public DateTimeOffset LastAccess { get; private set; } = now;

    public void Touch(DateTimeOffset now) => LastAccess = now;

    public GameResponse Begin() => Run(() =>
    {
        _engine.Start();
        _engine.Player.Look();
    });

    public GameResponse Execute(string command) => Run(() =>
    {
        if (Status != GameStatus.Playing)
        {
            _engine.Player.Output(WebMessages.GameFinished);
            return;
        }

        if (!IsPlayable(command))
        {
            _engine.Player.Output(WebMessages.UseTrackButtons);
            return;
        }

        _engine.ProcessCommand(command);
        Evaluate();
    });

    public GameResponse Restart(IReadOnlyList<string> commands) => Run(() =>
    {
        _engine = Build(Game);
        Status = GameStatus.Playing;
        _engine.Start();
        _engine.ExecuteTrack(commands);
        Evaluate();
    });

    public IReadOnlyList<string> Track()
    {
        lock (_gate)
            return _engine.Track.ToList();
    }

    public static bool IsPlayable(string command)
    {
        var type = CommandParser.Parse(command).Type;
        return type is not (CommandType.Exec or CommandType.Save);
    }

    private GameResponse Run(Action action)
    {
        lock (_gate)
        {
            var writer = new StringWriter();
            var previousLanguage = Localizer.Current;
            var previousWriter = Thing.OutputWriter;
            var previousVisible = Thing.Visible;
            var previousWidth = Thing.LineWidth;
            Localizer.Current = Language;
            Thing.OutputWriter = writer;
            Thing.Visible = true;
            Thing.LineWidth = int.MaxValue - 1;
            try
            {
                action();
            }
            finally
            {
                Localizer.Current = previousLanguage;
                Thing.OutputWriter = previousWriter;
                Thing.Visible = previousVisible;
                Thing.LineWidth = previousWidth;
            }

            return new GameResponse(Id, Game, LanguageCode, Status, SplitLines(writer.ToString()));
        }
    }

    private void Evaluate()
    {
        if (_engine.CheckGoal())
        {
            Status = GameStatus.Won;
            WriteEnding();
        }
        else if (_engine.IsOver)
        {
            Status = GameStatus.Lost;
            _engine.Player.Output(Localizer.Msg("Defeat"));
        }
    }

    private void WriteEnding()
    {
        if (_engine is Fort2)
        {
            foreach (var line in Fort2.Escape.Concat(Fort2.EpilogueFirstPage).Concat(Fort2.EpilogueLastPage))
                _engine.Player.Output(line.Get());
            return;
        }

        foreach (var key in new[] { "Victory1_Fort1", "Victory2_Fort1", "Victory3_Fort1", "Victory4_Fort1" })
            _engine.Player.Output(Localizer.Msg(key));
    }

    private static GameEngine Build(int game) => game == 2 ? Fort2Builder.Build() : Fort1Builder.Build();

    private static string[] SplitLines(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n').ToList();
        while (lines.Count > 0 && lines[^1] == "")
            lines.RemoveAt(lines.Count - 1);
        return [.. lines];
    }
}
