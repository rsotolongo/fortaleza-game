using System.Globalization;
using System.Text;
using Fortaleza.Localization;
using Fortaleza.Parser;
using Microsoft.Win32.SafeHandles;

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

    private Dictionary<CommandType, Action<ParsedCommand>> RecordedHandlers => field ??= new()
    {
        [CommandType.Go] = c =>
        {
            OpenSilently(c.Arg1);
            Player.Walk(c.Arg1);
        },
        [CommandType.Take] = c => Player.Take(c.Arg1),
        [CommandType.TakeAll] = static _ => Thing.Output(Localizer.Msg("TakeOneAtATime")),
        [CommandType.Leave] = c => Player.LeaveItem(c.Arg1),
        [CommandType.LeaveAll] = _ => Player.LeaveAll(),
        [CommandType.Open] = c => Player.OpenDoor(c.Arg1, c.Arg2),
        [CommandType.Kill] = c => Player.Kill(c.Arg1, c.Arg2),
        [CommandType.Look] = _ => Player.Look(),
        [CommandType.See] = c => Player.See(c.Arg1),
        [CommandType.Break] = c => Player.Break(c.Arg1, c.Arg2),
        [CommandType.Ask] = c => Player.Ask(c.Arg1),
        [CommandType.Give] = c => Player.Give(c.Arg1, c.Arg2),
        [CommandType.Piss] = static _ => Thing.Output(Localizer.Msg("Piss")),
        [CommandType.Clear] = static _ => Thing.Output(Localizer.Msg("Cls")),
    };

    private Dictionary<CommandType, Action<ParsedCommand>> UnrecordedHandlers => field ??= new()
    {
        [CommandType.Inventory] = _ => Player.Inventory(),
        [CommandType.Quit] = _ => Player.Die(string.Empty),
        [CommandType.Weigh] = c => Player.Weigh(c.Arg1),
        [CommandType.Exec] = _ => LoadTrackInteractive(),
        [CommandType.Save] = _ => SaveTrackInteractive(),
        [CommandType.Percent] = _ => ShowPercent(),
    };

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
        bool relevant = ExecuteCommand(cmd);

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
            using SafeFileHandle handle = File.OpenHandle(path, FileMode.Create, FileAccess.Write);
            RandomAccess.Write(handle, Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, _track) + Environment.NewLine), 0);
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

    private bool ExecuteCommand(ParsedCommand cmd)
    {
        bool recorded = RecordedHandlers.TryGetValue(cmd.Type, out Action<ParsedCommand>? handler);
        if (!recorded)
        {
            _ = UnrecordedHandlers.TryGetValue(cmd.Type, out handler);
        }

        if (handler is null && !string.IsNullOrEmpty(Wish.Trim()))
        {
            Thing.Output(Localizer.Msg("DontUnderstand"));
        }

        handler?.Invoke(cmd);
        return recorded;
    }
}
