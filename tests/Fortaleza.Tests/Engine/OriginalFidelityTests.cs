using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

public sealed class OriginalFidelityTests
{
    [Fact]
    public void WeightsNpcsHiddenAndLinksMatchOriginal()
    {
        var troll = new Troll(new("Bruja", "Witch"), new(string.Empty, string.Empty), "x", new(string.Empty, string.Empty), new(string.Empty, string.Empty));
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "x");
        var hidden = new Hidden(new("Jarrón", "Vase"), new(string.Empty, string.Empty), "x", new Item(new("a", "a"), new(string.Empty, string.Empty), 1));
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), string.Empty, MakeRoom());

        Assert.Equal(100, troll.Weight);
        Assert.Equal(Inventory.LWeight + 1, guard.Weight);
        Assert.Equal(Inventory.LWeight + 1, hidden.Weight);
        Assert.Equal(Inventory.LWeight + 1, link.Weight);
    }

    [Fact]
    public void TakeGuardCannotBeCarried()
    {
        Room room = MakeRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Látigo");
        room.Children.Add(guard);
        var player = new Player(room);

        player.Take("Lobo");

        Assert.Empty(player.Bag.Children);
        Assert.Contains(guard, room.Children);
    }

    [Fact]
    public void GetPartialWordsOfNameFindsThing()
    {
        Room room = MakeRoom();
        var door = new Link(new("Puerta de hierro", "Iron door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), string.Empty, MakeRoom());
        room.Children.Add(door);

        Assert.Same(door, room.Get("hierro"));
        Assert.Same(door, room.Get("PUERTA HIERRO"));
    }

    [Fact]
    public void GetExactNameWinsOverEarlierPartialMatch()
    {
        Room room = MakeRoom();
        var big = new Item(new("Espejo grande", "Big mirror"), new(string.Empty, string.Empty), 1);
        var exact = new Item(new("Espejo", "Mirror"), new(string.Empty, string.Empty), 1);
        room.Children.Add(big);
        room.Children.Add(exact);

        Assert.Same(exact, room.Get("espejo"));
    }

    [Theory]
    [InlineData("la maza")]
    [InlineData("maza roja")]
    [InlineData("")]
    public void GetQueryWordNotInNameReturnsNull(string query)
    {
        Room room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 1));

        Assert.Null(room.Get(query));
    }

    [Fact]
    public void LinkEmptyKeyStartsClosedAndOpensWithAnyWord()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), string.Empty, MakeRoom());

        Room? closedAtStart = link.GetDest();
        bool opened = link.TryOpen(string.Empty);

        Assert.Null(closedAtStart);
        Assert.True(opened);
        Assert.NotNull(link.GetDest());
    }

    [Fact]
    public void LinkOpenDoorSentinelStartsOpen()
    {
        Room dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), Link.OpenDoor, dest);

        Assert.Same(dest, link.GetDest());
    }

    [Theory]
    [InlineData(1, "AlmostFull")]
    [InlineData(0, "CannotCarryMore")]
    public void InventoryNearlyFullUsesOriginalThresholds(int remaining, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());
        player.Bag.Children.Add(new Item(new("Yunque", "Anvil"), new(string.Empty, string.Empty), Inventory.LWeight - remaining));

        string output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg(messageKey), output, StringComparison.Ordinal);
    }

    [Fact]
    public void InventoryPlentyOfRoomReportsRemainingBags()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));

        string output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg("CouldCarryMore") + "35" + Localizer.Msg("MoreBags"), output, StringComparison.Ordinal);
    }

    [Fact]
    public void InventoryEmptyBagReportsNothingCarried()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        string output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg("BagEmpty"), output, StringComparison.Ordinal);
        Assert.DoesNotContain(Localizer.Msg("BagContains"), output, StringComparison.Ordinal);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
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
}
