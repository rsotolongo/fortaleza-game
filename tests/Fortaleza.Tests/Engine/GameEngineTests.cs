using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for game engine.
/// </summary>
public sealed class GameEngineTests
{
    /// <summary>
    /// Process command take and leave round trips.
    /// </summary>
    [Fact]
    public void ProcessCommandTakeAndLeaveRoundTrips()
    {
        TestEngine engine = BuildMinimalEngine();

        engine.ProcessCommand("coger maza");
        int afterTake = engine.Player.Bag.Children.Count;
        engine.ProcessCommand("dejar maza");

        Assert.Equal(1, afterTake);
        Assert.Empty(engine.Player.Bag.Children);
    }

    /// <summary>
    /// Process command leave all empties bag.
    /// </summary>
    [Fact]
    public void ProcessCommandLeaveAllEmptiesBag()
    {
        TestEngine engine = BuildMinimalEngine();

        engine.ProcessCommand("take mace");
        engine.ProcessCommand("drop all");

        Assert.Empty(engine.Player.Bag.Children);
    }

    /// <summary>
    /// Process command take all asks to take one thing at a time.
    /// </summary>
    [Fact]
    public void ProcessCommandTakeAllAsksToTakeOneThingAtATime()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.ProcessCommand("tomar todo"));

        Assert.Empty(engine.Player.Bag.Children);
        Assert.Contains(Localizer.Msg("TakeOneAtATime"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Process command quit kills player and ends game.
    /// </summary>
    [Fact]
    public void ProcessCommandQuitKillsPlayerAndEndsGame()
    {
        TestEngine engine = BuildMinimalEngine();

        engine.ProcessCommand("abandonar");

        Assert.True(engine.IsOver);
        Assert.True(engine.Player.Dead);
    }

    /// <summary>
    /// Is over player dies returns true.
    /// </summary>
    [Fact]
    public void IsOverPlayerDiesReturnsTrue()
    {
        TestEngine engine = BuildMinimalEngine();

        engine.Player.Die(string.Empty);

        Assert.True(engine.IsOver);
    }

    /// <summary>
    /// Ask player end of input ends game.
    /// </summary>
    [Fact]
    public void AskPlayerEndOfInputEndsGame()
    {
        TestEngine engine = BuildMinimalEngine();

        Capture(engine.AskPlayer);

        Assert.True(engine.IsOver);
    }

    /// <summary>
    /// Process command look with object shows its description.
    /// </summary>
    [Fact]
    public void ProcessCommandLookWithObjectShowsItsDescription()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.ProcessCommand("mirar maza"));

        Assert.Contains("una maza", output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Process command go through closed unlocked door opens it silently then walks.
    /// </summary>
    [Fact]
    public void ProcessCommandGoThroughClosedUnlockedDoorOpensItSilentlyThenWalks()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();
        Room next = MakeRoom();
        engine.Player.CurrentRoom.Children.Add(
            new Link(new("Puerta azul", "Blue door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), string.Empty, next));

        string output = Capture(() => engine.ProcessCommand("ir puerta azul"));

        Assert.Same(next, engine.Player.CurrentRoom);
        Assert.DoesNotContain(Localizer.Msg("Ok"), output, StringComparison.Ordinal);
        Assert.True(Thing.Visible);
    }

    /// <summary>
    /// Process command go through locked door bumps nose.
    /// </summary>
    [Fact]
    public void ProcessCommandGoThroughLockedDoorBumpsNose()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();
        Room start = engine.Player.CurrentRoom;
        start.Children.Add(new Link(new("Puerta azul", "Blue door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "secreto", MakeRoom()));

        string output = Capture(() => engine.ProcessCommand("ir puerta azul"));

        Assert.Same(start, engine.Player.CurrentRoom);
        Assert.Contains(Localizer.Msg("Ouch"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Process command joke and unknown commands show their messages.
    /// </summary>
    [Theory]
    [InlineData("cls", "Cls")]
    [InlineData("orinar", "Piss")]
    [InlineData("xyzzy", "DontUnderstand")]
    public void ProcessCommandJokeAndUnknownCommandsShowTheirMessages(string input, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.ProcessCommand(input));

        Assert.Contains(Localizer.Msg(messageKey), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Process command empty input says nothing.
    /// </summary>
    [Fact]
    public void ProcessCommandEmptyInputSaysNothing()
    {
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.ProcessCommand(string.Empty));

        Assert.Equal(string.Empty, output);
    }

    /// <summary>
    /// Track only relevant commands are logged.
    /// </summary>
    [Fact]
    public void TrackOnlyRelevantCommandsAreLogged()
    {
        TestEngine engine = BuildMinimalEngine(2);

        engine.ProcessCommand("mirar");
        engine.ProcessCommand("inventario");
        engine.ProcessCommand("porciento");
        engine.ProcessCommand("pesar maza");
        engine.ProcessCommand("xyzzy");
        engine.ProcessCommand("coger maza");
        engine.ProcessCommand("ver maza");

        Assert.Equal(["mirar", "coger maza", "ver maza"], engine.Track);
    }

    /// <summary>
    /// Track stores the original typed text.
    /// </summary>
    [Fact]
    public void TrackStoresTheOriginalTypedText()
    {
        TestEngine engine = BuildMinimalEngine();

        engine.ProcessCommand("Coger la MAZA");

        Assert.Equal(["Coger la MAZA"], engine.Track);
    }

    /// <summary>
    /// Save track single command says nothing to save and writes no file.
    /// </summary>
    [Fact]
    public void SaveTrackSingleCommandSaysNothingToSaveAndWritesNoFile()
    {
        Localizer.Current = Language.Spanish;
        string path = TempTrack();
        TestEngine engine = BuildMinimalEngine();
        engine.ProcessCommand("coger maza");

        string output = Capture(() => engine.SaveTrack(path));

        Assert.False(File.Exists(path));
        Assert.Contains(Localizer.Msg("NothingToSave"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Save track then load track replays commands.
    /// </summary>
    [Fact]
    public void SaveTrackThenLoadTrackReplaysCommands()
    {
        string path = TempTrack();
        TestEngine source = BuildMinimalEngine();
        source.ProcessCommand("coger maza");
        source.ProcessCommand("mirar");
        source.SaveTrack(path);
        TestEngine target = BuildMinimalEngine();

        bool loaded = false;
        Capture(() => loaded = target.LoadTrack(path));
        File.Delete(path);

        Assert.True(loaded);
        Assert.Single(target.Player.Bag.Children);
        Assert.Equal(["coger maza", "mirar"], target.Track);
    }

    /// <summary>
    /// Load track verbose echoes each command.
    /// </summary>
    [Fact]
    public void LoadTrackVerboseEchoesEachCommand()
    {
        Localizer.Current = Language.Spanish;
        string path = TempTrack();
        File.WriteAllLines(path, ["coger maza"]);
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.LoadTrack(path));
        File.Delete(path);

        Assert.Contains(Localizer.Msg("ExecRunning"), output, StringComparison.Ordinal);
        Assert.Contains(" coger maza", output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Load track silent hides command output but restores visibility.
    /// </summary>
    [Fact]
    public void LoadTrackSilentHidesCommandOutputButRestoresVisibility()
    {
        Localizer.Current = Language.Spanish;
        string path = TempTrack();
        File.WriteAllLines(path, ["coger maza"]);
        TestEngine engine = BuildMinimalEngine();

        string output = Capture(() => engine.LoadTrack(path, verbose: false));
        File.Delete(path);

        Assert.DoesNotContain("coger maza", output, StringComparison.Ordinal);
        Assert.Single(engine.Player.Bag.Children);
        Assert.True(Thing.Visible);
    }

    /// <summary>
    /// Load track stops when player dies.
    /// </summary>
    [Fact]
    public void LoadTrackStopsWhenPlayerDies()
    {
        string path = TempTrack();
        File.WriteAllLines(path, ["abandonar", "coger maza"]);
        TestEngine engine = BuildMinimalEngine();

        Capture(() => engine.LoadTrack(path));
        File.Delete(path);

        Assert.Empty(engine.Player.Bag.Children);
    }

    /// <summary>
    /// Load track missing file reports it and returns false.
    /// </summary>
    [Fact]
    public void LoadTrackMissingFileReportsItAndReturnsFalse()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine();
        bool loaded = true;

        string output = Capture(() => loaded = engine.LoadTrack(TempTrack()));

        Assert.False(loaded);
        Assert.Contains(Localizer.Msg("FileNotFound"), output, StringComparison.Ordinal);
        Assert.True(Thing.Visible);
    }

    /// <summary>
    /// Process command execute without file name does nothing more.
    /// </summary>
    [Fact]
    public void ProcessCommandExecuteWithoutFileNameDoesNothingMore()
    {
        TestEngine engine = BuildMinimalEngine();
        engine.Input.Enqueue(string.Empty);

        Capture(() => engine.ProcessCommand("ejecutar"));

        Assert.Empty(engine.Track);
    }

    /// <summary>
    /// Show percent visited over rooms minus one is rounded.
    /// </summary>
    [Theory]
    [InlineData(0, 2, "50%")]
    [InlineData(1, 2, "100%")]
    [InlineData(1, 3, "67%")]
    [InlineData(0, 3, "33%")]
    public void ShowPercentVisitedOverRoomsMinusOneIsRounded(int moves, int extraRooms, string expected)
    {
        TestEngine engine = BuildMinimalEngine(extraRooms);
        foreach (Room? room in Enumerable.Range(1, extraRooms).Select(engine.GetRoom).Take(moves).ToList())
        {
            engine.Player.Go(room);
        }

        string output = Capture(engine.ShowPercent);

        Assert.Contains(expected, output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Show percent uses advanced message.
    /// </summary>
    [Fact]
    public void ShowPercentUsesAdvancedMessage()
    {
        Localizer.Current = Language.Spanish;
        TestEngine engine = BuildMinimalEngine(2);

        string output = Capture(engine.ShowPercent);

        Assert.Contains(Localizer.Msg("Advanced") + "50%", output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Process command walk through open link updates current room.
    /// </summary>
    [Fact]
    public void ProcessCommandWalkThroughOpenLinkUpdatesCurrentRoom()
    {
        TestEngine engine = BuildMinimalEngine();
        Room next = MakeRoom();
        engine.AddRoom(next);
        engine.Player.CurrentRoom.Children.Add(new OpenLink(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), next));

        engine.ProcessCommand("ir puerta");

        Assert.Same(next, engine.Player.CurrentRoom);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new("descripción", "description"), Inventory.LWeight + 1);
    }

    private static TestEngine BuildMinimalEngine(int extraRooms = 0)
    {
        Room room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("una maza", "a mace"), 5));
        var engine = new TestEngine(room);
        engine.AddRoom(room);
        for (int i = 0; i < extraRooms; i++)
        {
            engine.AddRoom(MakeRoom());
        }

        engine.Start();
        return engine;
    }

    private static string Capture(Action action)
    {
        var writer = new StringWriter();
        TextWriter original = Console.Out;
        Console.SetOut(writer);
        try
        { action(); }
        finally { Console.SetOut(original); }
        return writer.ToString().Replace(Environment.NewLine, " ", StringComparison.Ordinal);
    }

    private static string TempTrack()
    {
        return Path.Combine(Path.GetTempPath(), $"fortaleza-{Guid.NewGuid():N}.trk");
    }

    private sealed class TestEngine(Room start) : GameEngine(start)
    {
        public Queue<string> Input { get; } = new();

        public override bool CheckGoal()
        {
            return false;
        }

        protected override string? ReadLine()
        {
            return Input.Count > 0 ? Input.Dequeue() : null;
        }
    }
}
