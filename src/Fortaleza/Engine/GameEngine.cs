using System.Globalization;
using Fortaleza.Localization;
using Fortaleza.Parser;

namespace Fortaleza.Engine;

internal abstract class GameEngine(Room startRoom)
{
    public const string DefaultTrackFile = "TRAVEL.TRK";

    private readonly List<Room> _rooms = [];
    private readonly List<string> _track = [];
    private string _wishBack = string.Empty;
    private bool _executing;

    public Player Player { get; } = new(startRoom);
    public string Wish { get; private set; } = string.Empty;
    public bool IsOver { get => field || Player.Dead; private set; }
    public IReadOnlyList<string> Track => _track;

    public abstract bool CheckGoal();

    public void AddRoom(Room room)
    {
        _rooms.Add(room);
    }

    public Room GetRoom(int index)
    {
        return _rooms[index];
    }

    public void Start()
    {
        Player.Go(startRoom);
    }

    public void AskPlayer()
    {
        Thing.Output(string.Empty);
        Thing.Output(Localizer.Msg("WhatToDo"));
        string? line = ReadLine();
        if (line is null)
        {
            IsOver = true;
        }

        SetWish(line ?? string.Empty);
    }

    public void ProcessCommand(string input)
    {
        SetWish(input);
        ProcessWish();
    }

    public void ProcessWish()
    {
        ParsedCommand cmd = CommandParser.Parse(Wish);
        bool relevant = true;
        switch (cmd.Type)
        {
            case CommandType.Go:
                {
                    OpenSilently(cmd.Arg1);
                    Player.Walk(cmd.Arg1);
                    break;
                }

            case CommandType.Take:
                {
                    Player.Take(cmd.Arg1);
                    break;
                }

            case CommandType.TakeAll:
                {
                    Thing.Output(Localizer.Msg("TakeOneAtATime"));
                    break;
                }

            case CommandType.Leave:
                {
                    Player.LeaveItem(cmd.Arg1);
                    break;
                }

            case CommandType.LeaveAll:
                {
                    Player.LeaveAll();
                    break;
                }

            case CommandType.Open:
                {
                    Player.OpenDoor(cmd.Arg1, cmd.Arg2);
                    break;
                }

            case CommandType.Kill:
                {
                    Player.Kill(cmd.Arg1, cmd.Arg2);
                    break;
                }

            case CommandType.Look:
                {
                    Player.Look();
                    break;
                }

            case CommandType.See:
                {
                    Player.See(cmd.Arg1);
                    break;
                }

            case CommandType.Break:
                {
                    Player.Break(cmd.Arg1, cmd.Arg2);
                    break;
                }

            case CommandType.Ask:
                {
                    Player.Ask(cmd.Arg1);
                    break;
                }

            case CommandType.Give:
                {
                    Player.Give(cmd.Arg1, cmd.Arg2);
                    break;
                }

            case CommandType.Piss:
                {
                    Thing.Output(Localizer.Msg("Piss"));
                    break;
                }

            case CommandType.Clear:
                {
                    Thing.Output(Localizer.Msg("Cls"));
                    break;
                }

            case CommandType.Inventory:
                {
                    Player.Inventory();
                    relevant = false;
                    break;
                }

            case CommandType.Quit:
                {
                    Player.Die(string.Empty);
                    relevant = false;
                    break;
                }

            case CommandType.Weigh:
                {
                    Player.Weigh(cmd.Arg1);
                    relevant = false;
                    break;
                }

            case CommandType.Exec:
                {
                    LoadTrackInteractive();
                    relevant = false;
                    break;
                }

            case CommandType.Save:
                {
                    SaveTrackInteractive();
                    relevant = false;
                    break;
                }

            case CommandType.Percent:
                {
                    ShowPercent();
                    relevant = false;
                    break;
                }

            default:
                {
                    if (!string.IsNullOrEmpty(Wish.Trim()))
                    {
                        Thing.Output(Localizer.Msg("DontUnderstand"));
                    }

                    relevant = false;
                    break;
                }
        }

        if (!relevant || string.IsNullOrEmpty(_wishBack) || _executing)
        {
            return;
        }

        _track.Add(_wishBack);
    }

    public void SaveTrack(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (_track.Count <= 1)
        {
            Thing.Output(Localizer.Msg("NothingToSave"));
            return;
        }
        try
        {
            File.WriteAllLines(path, _track);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Thing.Output(Localizer.Msg("SaveError"));
        }
    }

    public bool LoadTrack(string path, bool verbose = true)
    {
        bool wasVisible = Thing.Visible;
        Thing.Output(Localizer.Msg("ExecRunning"));
        if (!verbose)
        {
            Thing.Visible = false;
        }

        try
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                Thing.Visible = true;
                Thing.Output(Localizer.Msg("FileNotFound"));
                return false;
            }

            var lines = File.ReadLines(path).Where(static l => !string.IsNullOrEmpty(l)).ToList();
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
        bool wasVisible = Thing.Visible;
        Thing.Output(Localizer.Msg("ExecRunning"));
        if (!verbose)
        {
            Thing.Visible = false;
        }

        try
        {
            Replay([.. commands.Where(static l => !string.IsNullOrEmpty(l))]);
        }
        finally
        {
            Thing.Visible = wasVisible;
        }
    }

    public void ShowPercent()
    {
        int reachable = _rooms.Count - 1;
        int percent = reachable <= 0
            ? 0
            : Convert.ToInt32(
                Math.Round(Convert.ToDouble(Player.VisitCount, CultureInfo.InvariantCulture) / reachable * 100, MidpointRounding.AwayFromZero),
                CultureInfo.InvariantCulture);
        Thing.Output(Localizer.Msg("Advanced") + percent.ToString(CultureInfo.InvariantCulture) + "%");
    }

    protected virtual string? ReadLine()
    {
        return Console.ReadLine();
    }

    private void Replay(List<string> lines)
    {
        _track.Clear();
        _track.AddRange(lines);
        _executing = true;
        try
        {
            bool finished = false;
            for (int i = 0; i < lines.Count && !finished; i++)
            {
                Thing.Output(string.Empty);
                Thing.Output(" " + lines[i]);
                ProcessCommand(lines[i]);
                finished = CheckGoal() || IsOver;
            }
        }
        finally
        {
            _executing = false;
        }
    }

    private void SetWish(string input)
    {
        Wish = input;
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        _wishBack = input;
    }

    private void OpenSilently(string door)
    {
        bool wasVisible = Thing.Visible;
        Thing.Visible = false;
        try
        {
            Player.OpenDoor(door, string.Empty);
        }
        finally
        {
            Thing.Visible = wasVisible;
        }
    }

    private void SaveTrackInteractive()
    {
        Thing.Output(Localizer.Msg("EnterFileName"));
        string path = (ReadLine() ?? string.Empty).Trim();
        SaveTrack(string.IsNullOrEmpty(path) ? DefaultTrackFile : path);
    }

    private void LoadTrackInteractive()
    {
        Thing.Output(Localizer.Msg("EnterFileName"));
        LoadTrack((ReadLine() ?? string.Empty).Trim());
    }
}
