using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class OriginalFidelityTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    [Fact]
    public void Weights_NpcsHiddenAndLinks_MatchOriginal()
    {
        var troll = new Troll(new("Bruja", "Witch"), new("", ""), "x", new("", ""), new("", ""));
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("", ""), "x");
        var hidden = new Hidden(new("Jarrón", "Vase"), new("", ""), "x", new Item(new("a", "a"), new("", ""), 1));
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "", MakeRoom());

        Assert.Equal(100, troll.Weight);
        Assert.Equal(Inventory.LWeight + 1, guard.Weight);
        Assert.Equal(Inventory.LWeight + 1, hidden.Weight);
        Assert.Equal(Inventory.LWeight + 1, link.Weight);
    }

    [Fact]
    public void Take_Guard_CannotBeCarried()
    {
        var room = MakeRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("", ""), "Látigo");
        room.Children.Add(guard);
        var player = new Player(room);

        player.Take("Lobo");

        Assert.Empty(player.Bag.Children);
        Assert.Contains(guard, room.Children);
    }

    [Fact]
    public void Get_PartialWordsOfName_FindsThing()
    {
        var room = MakeRoom();
        var door = new Link(new("Puerta de hierro", "Iron door"), new("", ""), new("", ""), "", MakeRoom());
        room.Children.Add(door);

        Assert.Same(door, room.Get("hierro"));
        Assert.Same(door, room.Get("PUERTA HIERRO"));
    }

    [Fact]
    public void Get_ExactNameWins_OverEarlierPartialMatch()
    {
        var room = MakeRoom();
        var big = new Item(new("Espejo grande", "Big mirror"), new("", ""), 1);
        var exact = new Item(new("Espejo", "Mirror"), new("", ""), 1);
        room.Children.Add(big);
        room.Children.Add(exact);

        Assert.Same(exact, room.Get("espejo"));
    }

    [Theory]
    [InlineData("la maza")]
    [InlineData("maza roja")]
    [InlineData("")]
    public void Get_QueryWordNotInName_ReturnsNull(string query)
    {
        var room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 1));

        Assert.Null(room.Get(query));
    }

    [Fact]
    public void Link_EmptyKey_StartsClosedAndOpensWithAnyWord()
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "", MakeRoom());

        var closedAtStart = link.GetDest();
        var opened = link.TryOpen("");

        Assert.Null(closedAtStart);
        Assert.True(opened);
        Assert.NotNull(link.GetDest());
    }

    [Fact]
    public void Link_OpenDoorSentinel_StartsOpen()
    {
        var dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), Link.OpenDoor, dest);

        Assert.Same(dest, link.GetDest());
    }

    private static string Capture(Action action)
    {
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try { action(); }
        finally { Console.SetOut(original); }
        return writer.ToString();
    }

    [Theory]
    [InlineData(1, "AlmostFull")]
    [InlineData(0, "CannotCarryMore")]
    public void Inventory_NearlyFull_UsesOriginalThresholds(int remaining, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());
        player.Bag.Children.Add(new Item(new("Yunque", "Anvil"), new("", ""), Inventory.LWeight - remaining));

        var output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg(messageKey), output);
    }

    [Fact]
    public void Inventory_PlentyOfRoom_ReportsRemainingBags()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));

        var output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg("CouldCarryMore") + "35" + Localizer.Msg("MoreBags"), output);
    }

    [Fact]
    public void Inventory_EmptyBag_ReportsNothingCarried()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        var output = Capture(player.Inventory);

        Assert.Contains(Localizer.Msg("BagEmpty"), output);
        Assert.DoesNotContain(Localizer.Msg("BagContains"), output);
    }
}
