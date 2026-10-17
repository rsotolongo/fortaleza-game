using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Engine;

public abstract class GameEngine(Room startRoom)
{
    private readonly List<Room> _rooms = [];
    private readonly List<string> _track = [];
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
        Player.Output(Localizer.Msg("WhatToDo"));
        var line = ReadLine();
        if (line is null)
            _quit = true;
        Wish = line ?? "";
    }

    public void ClearWish() => Wish = "";

    public void ProcessCommand(string input)
    {
        Wish = input;
        ProcessWish();
    }

    public void ProcessWish()
    {
        var cmd = CommandParser.Parse(Wish);
        switch (cmd.Type)
        {
            case CommandType.Go: Player.Walk(cmd.Arg1); break;
            case CommandType.Take: Player.Take(cmd.Arg1); break;
            case CommandType.Leave: Player.LeaveItem(cmd.Arg1); break;
            case CommandType.LeaveAll: Player.LeaveAll(); break;
            case CommandType.Open: Player.OpenDoor(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Kill: Player.Kill(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Look: Player.Look(); break;
            case CommandType.See: Player.See(cmd.Arg1); break;
            case CommandType.Break: Player.Break(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Ask: Player.Ask(cmd.Arg1); break;
            case CommandType.Inventory: Player.Inventory(); break;
            case CommandType.Give: Player.Give(cmd.Arg1, cmd.Arg2); break;
            case CommandType.Quit: _quit = true; break;
            case CommandType.Save: SaveTrackInteractive(); break;
            case CommandType.Exec: LoadTrackInteractive(); break;
            case CommandType.Percent: ShowPercent(); break;
            case CommandType.Weigh: Player.Weigh(cmd.Arg1); break;
            case CommandType.Clear: Console.Clear(); break;
            default: Player.Output(Localizer.Msg("DontUnderstand")); break;
        }

        if (IsTracked(cmd.Type))
            _track.Add(Wish.Trim());
    }

    public void SaveTrack(string path)
    {
        if (_track.Count == 0)
        {
            Player.Output(Localizer.Msg("NothingToSave"));
            return;
        }
        try
        {
            File.WriteAllLines(path, _track);
        }
        catch (IOException)
        {
            Player.Output(Localizer.Msg("SaveError"));
        }
    }

    public bool LoadTrack(string path)
    {
        if (!File.Exists(path))
        {
            Player.Output(Localizer.Msg("FileNotFound"));
            return false;
        }
        var wasVisible = Thing.Visible;
        Thing.Visible = false;
        try
        {
            foreach (var line in File.ReadLines(path))
            {
                if (IsOver) break;
                if (line.Trim() != "")
                    ProcessCommand(line);
            }
        }
        finally
        {
            Thing.Visible = wasVisible;
        }
        return true;
    }

    public void ShowPercent()
    {
        var percent = _rooms.Count == 0 ? 0 : Player.VisitCount * 100 / _rooms.Count;
        Player.Output(Localizer.Msg("Advanced") + percent + "%");
    }

    protected virtual string? ReadLine() => Console.ReadLine();

    private void SaveTrackInteractive()
    {
        Player.Output(Localizer.Msg("EnterFileName"));
        var path = ReadLine();
        if (!string.IsNullOrWhiteSpace(path))
            SaveTrack(path.Trim());
    }

    private void LoadTrackInteractive()
    {
        Player.Output(Localizer.Msg("EnterFileName"));
        var path = ReadLine();
        if (!string.IsNullOrWhiteSpace(path))
            LoadTrack(path.Trim());
    }

    private static bool IsTracked(CommandType type) => type is not (
        CommandType.Look or CommandType.See or CommandType.Inventory or CommandType.Ask
        or CommandType.Percent or CommandType.Save or CommandType.Exec or CommandType.Quit
        or CommandType.Clear or CommandType.Unknown);
}
