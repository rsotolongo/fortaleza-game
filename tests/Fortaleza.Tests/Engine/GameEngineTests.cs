using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class GameEngineTests
{
    private sealed class TestEngine(Room start) : GameEngine(start)
    {
        public Queue<string> Input { get; } = new();

        public override bool CheckGoal() => false;

        protected override string? ReadLine() => Input.Count > 0 ? Input.Dequeue() : null;
    }

    private static Room MakeRoom() =>
        new(new("sala", "room"), new("descripción", "description"), Inventory.LWeight + 1);

    private static TestEngine BuildMinimalEngine(int extraRooms = 0)
    {
        var room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("una maza", "a mace"), 5));
        var engine = new TestEngine(room);
        engine.AddRoom(room);
        for (var i = 0; i < extraRooms; i++)
            engine.AddRoom(MakeRoom());
        engine.Start();
        return engine;
    }

    private static string Capture(Action action)
    {
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try { action(); }
        finally { Console.SetOut(original); }
        return writer.ToString().Replace(Environment.NewLine, " ");
    }

    private static string TempTrack() =>
        Path.Combine(Path.GetTempPath(), $"fortaleza-{Guid.NewGuid():N}.trk");

    [Fact]
    public void ProcessCommand_TakeAndLeave_RoundTrips()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("coger maza");
        var afterTake = engine.Player.Bag.Children.Count;
        engine.ProcessCommand("dejar maza");

        Assert.Equal(1, afterTake);
        Assert.Empty(engine.Player.Bag.Children);
    }

    [Fact]
    public void ProcessCommand_LeaveAll_EmptiesBag()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("take mace");
        engine.ProcessCommand("drop all");

        Assert.Empty(engine.Player.Bag.Children);
    }

    [Fact]
    public void ProcessCommand_TakeAll_AsksToTakeOneThingAtATime()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.ProcessCommand("tomar todo"));

        Assert.Empty(engine.Player.Bag.Children);
        Assert.Contains(Localizer.Msg("TakeOneAtATime"), output);
    }

    [Fact]
    public void ProcessCommand_Quit_KillsPlayerAndEndsGame()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("abandonar");

        Assert.True(engine.IsOver);
        Assert.True(engine.Player.Dead);
    }

    [Fact]
    public void IsOver_PlayerDies_ReturnsTrue()
    {
        var engine = BuildMinimalEngine();

        engine.Player.Die("");

        Assert.True(engine.IsOver);
    }

    [Fact]
    public void AskPlayer_EndOfInput_EndsGame()
    {
        var engine = BuildMinimalEngine();

        Capture(engine.AskPlayer);

        Assert.True(engine.IsOver);
    }

    [Fact]
    public void ProcessCommand_LookWithObject_ShowsItsDescription()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.ProcessCommand("mirar maza"));

        Assert.Contains("una maza", output);
    }

    [Fact]
    public void ProcessCommand_GoThroughClosedUnlockedDoor_OpensItSilentlyThenWalks()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();
        var next = MakeRoom();
        engine.Player.CurrentRoom.Children.Add(
            new Link(new("Puerta azul", "Blue door"), new("", ""), new("", ""), "", next));

        var output = Capture(() => engine.ProcessCommand("ir puerta azul"));

        Assert.Same(next, engine.Player.CurrentRoom);
        Assert.DoesNotContain(Localizer.Msg("Ok"), output);
        Assert.True(Thing.Visible);
    }

    [Fact]
    public void ProcessCommand_GoThroughLockedDoor_BumpsNose()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();
        var start = engine.Player.CurrentRoom;
        start.Children.Add(new Link(new("Puerta azul", "Blue door"), new("", ""), new("", ""), "secreto", MakeRoom()));

        var output = Capture(() => engine.ProcessCommand("ir puerta azul"));

        Assert.Same(start, engine.Player.CurrentRoom);
        Assert.Contains(Localizer.Msg("Ouch"), output);
    }

    [Theory]
    [InlineData("cls", "Cls")]
    [InlineData("orinar", "Piss")]
    [InlineData("xyzzy", "DontUnderstand")]
    public void ProcessCommand_JokeAndUnknownCommands_ShowTheirMessages(string input, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.ProcessCommand(input));

        Assert.Contains(Localizer.Msg(messageKey), output);
    }

    [Fact]
    public void ProcessCommand_EmptyInput_SaysNothing()
    {
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.ProcessCommand(""));

        Assert.Equal("", output);
    }

    [Fact]
    public void Track_OnlyRelevantCommandsAreLogged()
    {
        var engine = BuildMinimalEngine(2);

        engine.ProcessCommand("mirar");
        engine.ProcessCommand("inventario");
        engine.ProcessCommand("porciento");
        engine.ProcessCommand("pesar maza");
        engine.ProcessCommand("xyzzy");
        engine.ProcessCommand("coger maza");
        engine.ProcessCommand("ver maza");

        Assert.Equal(["mirar", "coger maza", "ver maza"], engine.Track);
    }

    [Fact]
    public void Track_StoresTheOriginalTypedText()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("Coger la MAZA");

        Assert.Equal(["Coger la MAZA"], engine.Track);
    }

    [Fact]
    public void SaveTrack_SingleCommand_SaysNothingToSaveAndWritesNoFile()
    {
        Localizer.Current = Language.Spanish;
        var path = TempTrack();
        var engine = BuildMinimalEngine();
        engine.ProcessCommand("coger maza");

        var output = Capture(() => engine.SaveTrack(path));

        Assert.False(File.Exists(path));
        Assert.Contains(Localizer.Msg("NothingToSave"), output);
    }

    [Fact]
    public void SaveTrack_ThenLoadTrack_ReplaysCommands()
    {
        var path = TempTrack();
        var source = BuildMinimalEngine();
        source.ProcessCommand("coger maza");
        source.ProcessCommand("mirar");
        source.SaveTrack(path);
        var target = BuildMinimalEngine();

        var loaded = false;
        Capture(() => loaded = target.LoadTrack(path));
        File.Delete(path);

        Assert.True(loaded);
        Assert.Single(target.Player.Bag.Children);
        Assert.Equal(["coger maza", "mirar"], target.Track);
    }

    [Fact]
    public void LoadTrack_Verbose_EchoesEachCommand()
    {
        Localizer.Current = Language.Spanish;
        var path = TempTrack();
        File.WriteAllLines(path, ["coger maza"]);
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.LoadTrack(path));
        File.Delete(path);

        Assert.Contains(Localizer.Msg("ExecRunning"), output);
        Assert.Contains(" coger maza", output);
    }

    [Fact]
    public void LoadTrack_Silent_HidesCommandOutputButRestoresVisibility()
    {
        Localizer.Current = Language.Spanish;
        var path = TempTrack();
        File.WriteAllLines(path, ["coger maza"]);
        var engine = BuildMinimalEngine();

        var output = Capture(() => engine.LoadTrack(path, verbose: false));
        File.Delete(path);

        Assert.DoesNotContain("coger maza", output);
        Assert.Single(engine.Player.Bag.Children);
        Assert.True(Thing.Visible);
    }

    [Fact]
    public void LoadTrack_StopsWhenPlayerDies()
    {
        var path = TempTrack();
        File.WriteAllLines(path, ["abandonar", "coger maza"]);
        var engine = BuildMinimalEngine();

        Capture(() => engine.LoadTrack(path));
        File.Delete(path);

        Assert.Empty(engine.Player.Bag.Children);
    }

    [Fact]
    public void LoadTrack_MissingFile_ReportsItAndReturnsFalse()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine();
        var loaded = true;

        var output = Capture(() => loaded = engine.LoadTrack(TempTrack()));

        Assert.False(loaded);
        Assert.Contains(Localizer.Msg("FileNotFound"), output);
        Assert.True(Thing.Visible);
    }

    [Fact]
    public void ProcessCommand_ExecuteWithoutFileName_DoesNothingMore()
    {
        var engine = BuildMinimalEngine();
        engine.Input.Enqueue("");

        Capture(() => engine.ProcessCommand("ejecutar"));

        Assert.Empty(engine.Track);
    }

    [Theory]
    [InlineData(0, 2, "50%")]
    [InlineData(1, 2, "100%")]
    [InlineData(1, 3, "67%")]
    [InlineData(0, 3, "33%")]
    public void ShowPercent_VisitedOverRoomsMinusOne_IsRounded(int moves, int extraRooms, string expected)
    {
        var engine = BuildMinimalEngine(extraRooms);
        var rooms = Enumerable.Range(1, extraRooms).Select(engine.GetRoom).Take(moves).ToList();
        foreach (var room in rooms)
            engine.Player.Go(room);

        var output = Capture(engine.ShowPercent);

        Assert.Contains(expected, output);
    }

    [Fact]
    public void ShowPercent_UsesAdvancedMessage()
    {
        Localizer.Current = Language.Spanish;
        var engine = BuildMinimalEngine(2);

        var output = Capture(engine.ShowPercent);

        Assert.Contains(Localizer.Msg("Advanced") + "50%", output);
    }

    [Fact]
    public void ProcessCommand_WalkThroughOpenLink_UpdatesCurrentRoom()
    {
        var engine = BuildMinimalEngine();
        var next = MakeRoom();
        engine.AddRoom(next);
        engine.Player.CurrentRoom.Children.Add(new OpenLink(new("Puerta", "Door"), new("", ""), new("", ""), next));

        engine.ProcessCommand("ir puerta");

        Assert.Same(next, engine.Player.CurrentRoom);
    }
}
