using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Engine;

public abstract class GameEngine(Room startRoom)
{
    public const string DefaultTrackFile = "TRAVEL.TRK";

    private readonly List<Room> _rooms = [];
    private readonly List<string> _track = [];
    private string _wishBack = "";
    private bool _executing;
    private bool _quit;

    public Player Player { get; } = new(startRoom);
    public string Wish { get; private set; } = "";
    public bool IsOver => _quit || Player.Dead;
    public IReadOnlyList<string> Track => _track;

    public abstract bool CheckGoal();

    public void AddRoom(Room room) => _rooms.Add(room);

    public Room GetRoom(int index) => _rooms[index];

    public void Start() => Player.Go(startRoom);

    public void AskPlayer()
    {
        Player.Output("");
        Player.Output(Localizer.Msg("WhatToDo"));
        var line = ReadLine();
        if (line is null)
            _quit = true;
        SetWish(line ?? "");
    }

    public void ProcessCommand(string input)
    {
        SetWish(input);
        ProcessWish();
    }

    public void ProcessWish()
    {
        var cmd = CommandParser.Parse(Wish);
        var relevant = true;
        switch (cmd.Type)
        {
            case CommandType.Go:
                OpenSilently(cmd.Arg1);
                Player.Walk(cmd.Arg1);
                break;
            case CommandType.Take: Player.Take(cmd.Arg1); break;
            case CommandType.TakeAll: Player.Output(Localizer.Msg("TakeOneAtATime")); break;
            case CommandType.Leave: Player.LeaveItem(cmd.Arg1); break;
            case CommandType.LeaveAll: Player.LeaveAll(); break;
            case CommandType.Open: Player.OpenDoor(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Kill: Player.Kill(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Look: Player.Look(); break;
            case CommandType.See: Player.See(cmd.Arg1); break;
            case CommandType.Break: Player.Break(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Ask: Player.Ask(cmd.Arg1); break;
            case CommandType.Give: Player.Give(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Piss: Player.Output(Localizer.Msg("Piss")); break;
            case CommandType.Clear: Player.Output(Localizer.Msg("Cls")); break;
            case CommandType.Inventory:
                Player.Inventory();
                relevant = false;
                break;
            case CommandType.Quit:
                Player.Die("");
                relevant = false;
                break;
            case CommandType.Weigh:
                Player.Weigh(cmd.Arg1);
                relevant = false;
                break;
            case CommandType.Exec:
                LoadTrackInteractive();
                relevant = false;
                break;
            case CommandType.Save:
                SaveTrackInteractive();
                relevant = false;
                break;
            case CommandType.Percent:
                ShowPercent();
                relevant = false;
                break;
            default:
                if (Wish.Trim() != "")
                    Player.Output(Localizer.Msg("DontUnderstand"));
                relevant = false;
                break;
        }

        if (relevant && _wishBack != "" && !_executing)
            _track.Add(_wishBack);
    }

    public void SaveTrack(string path)
    {
        if (path == "") return;
        if (_track.Count <= 1)
        {
            Player.Output(Localizer.Msg("NothingToSave"));
            return;
        }
        try
        {
            File.WriteAllLines(path, _track);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Player.Output(Localizer.Msg("SaveError"));
        }
    }

    public bool LoadTrack(string path, bool verbose = true)
    {
        var wasVisible = Thing.Visible;
        Player.Output(Localizer.Msg("ExecRunning"));
        if (!verbose)
            Thing.Visible = false;
        try
        {
            if (path == "") return false;
            if (!File.Exists(path))
            {
                Thing.Visible = true;
                Player.Output(Localizer.Msg("FileNotFound"));
                return false;
            }

            var lines = File.ReadLines(path).Where(l => l != "").ToList();
            Replay(lines);
            return true;
        }
        finally
        {
            _executing = false;
            Thing.Visible = wasVisible;
        }
    }

    public void ExecuteTrack(IEnumerable<string> commands, bool verbose = true)
    {
        var wasVisible = Thing.Visible;
        Player.Output(Localizer.Msg("ExecRunning"));
        if (!verbose)
            Thing.Visible = false;
        try
        {
            Replay(commands.Where(l => l != "").ToList());
        }
        finally
        {
            Thing.Visible = wasVisible;
        }
    }

    public void ShowPercent()
    {
        var reachable = _rooms.Count - 1;
        var percent = reachable <= 0
            ? 0
            : (int)Math.Round((double)Player.VisitCount / reachable * 100, MidpointRounding.AwayFromZero);
        Player.Output(Localizer.Msg("Advanced") + percent + "%");
    }

    private void Replay(List<string> lines)
    {
        _track.Clear();
        _track.AddRange(lines);
        _executing = true;
        try
        {
            foreach (var line in lines)
            {
                Player.Output("");
                Player.Output(" " + line);
                ProcessCommand(line);
                if (CheckGoal() || IsOver) break;
            }
        }
        finally
        {
            _executing = false;
        }
    }

    protected virtual string? ReadLine() => Console.ReadLine();

    private void SetWish(string input)
    {
        Wish = input;
        if (input != "")
            _wishBack = input;
    }

    private void OpenSilently(string door)
    {
        var wasVisible = Thing.Visible;
        Thing.Visible = false;
        try
        {
            Player.OpenDoor(door, "");
        }
        finally
        {
            Thing.Visible = wasVisible;
        }
    }

    private void SaveTrackInteractive()
    {
        Player.Output(Localizer.Msg("EnterFileName"));
        var path = (ReadLine() ?? "").Trim();
        SaveTrack(path == "" ? DefaultTrackFile : path);
    }

    private void LoadTrackInteractive()
    {
        Player.Output(Localizer.Msg("EnterFileName"));
        LoadTrack((ReadLine() ?? "").Trim());
    }
}
