using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class GameEngineTests
{
    private sealed class TestEngine(Room start) : GameEngine(start)
    {
        public override bool CheckGoal() => false;
    }

    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    private static TestEngine BuildMinimalEngine()
    {
        var room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));
        var engine = new TestEngine(room);
        engine.AddRoom(room);
        engine.Start();
        return engine;
    }

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
    public void ProcessCommand_Quit_EndsGame()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("abandonar");

        Assert.True(engine.IsOver);
    }

    [Fact]
    public void IsOver_PlayerDies_ReturnsTrue()
    {
        var engine = BuildMinimalEngine();

        engine.Player.Die("");

        Assert.True(engine.IsOver);
    }

    [Fact]
    public void Track_StateChangingCommands_AreLoggedAndReadOnlyOnesAreNot()
    {
        var engine = BuildMinimalEngine();

        engine.ProcessCommand("mirar");
        engine.ProcessCommand("inventario");
        engine.ProcessCommand("coger maza");
        engine.ProcessCommand("xyzzy");

        Assert.Equal(["coger maza"], engine.Track);
    }

    [Fact]
    public void SaveTrack_ThenLoadTrack_ReplaysCommands()
    {
        var path = Path.Combine(Path.GetTempPath(), $"fortaleza-{Guid.NewGuid():N}.trk");
        var source = BuildMinimalEngine();
        source.ProcessCommand("coger maza");
        source.SaveTrack(path);
        var target = BuildMinimalEngine();

        var loaded = target.LoadTrack(path);
        File.Delete(path);

        Assert.True(loaded);
        Assert.Single(target.Player.Bag.Children);
    }

    [Fact]
    public void LoadTrack_MissingFile_ReturnsFalse()
    {
        var engine = BuildMinimalEngine();

        var loaded = engine.LoadTrack(Path.Combine(Path.GetTempPath(), "missing-fortaleza.trk"));

        Assert.False(loaded);
    }

    [Fact]
    public void LoadTrack_RestoresVisibility()
    {
        var path = Path.Combine(Path.GetTempPath(), $"fortaleza-{Guid.NewGuid():N}.trk");
        File.WriteAllLines(path, ["coger maza"]);
        var engine = BuildMinimalEngine();

        engine.LoadTrack(path);
        File.Delete(path);

        Assert.True(Thing.Visible);
    }

    [Fact]
    public void ShowPercent_OneOfTwoRoomsVisited_ReportsFiftyPercent()
    {
        var engine = BuildMinimalEngine();
        var other = MakeRoom();
        engine.AddRoom(other);
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);

        try { engine.ShowPercent(); }
        finally { Console.SetOut(original); }

        Assert.Contains("50", writer.ToString());
    }

    [Fact]
    public void ProcessCommand_WalkThroughLink_UpdatesCurrentRoom()
    {
        var engine = BuildMinimalEngine();
        var next = MakeRoom();
        engine.AddRoom(next);
        engine.Player.CurrentRoom.Children.Add(new OpenLink(new("Puerta", "Door"), new("", ""), new("", ""), next));

        engine.ProcessCommand("ir puerta");

        Assert.Same(next, engine.Player.CurrentRoom);
    }
}
