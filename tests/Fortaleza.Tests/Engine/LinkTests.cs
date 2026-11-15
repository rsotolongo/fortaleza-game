using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

public sealed class LinkTests
{
    [Fact]
    public void LinkTryOpenSucceedsWithEmptyKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new("you pass", "you pass"), string.Empty, MakeRoom());
        Assert.True(link.TryOpen(string.Empty));
    }

    [Fact]
    public void LinkTryOpenSucceedsWithCorrectKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", MakeRoom());
        Assert.True(link.TryOpen("Fortaleza"));
        Assert.True(link.TryOpen("FORTALEZA"));
    }

    [Fact]
    public void LinkTryOpenFailsWithWrongKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", MakeRoom());
        Assert.False(link.TryOpen("wrong"));
    }

    [Fact]
    public void LinkGetDestReturnsNullWhenClosed()
    {
        Room dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", dest);
        Assert.Null(link.GetDest());
    }

    [Fact]
    public void LinkGetDestReturnsRoomWhenOpen()
    {
        Room dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), Link.OpenDoor, dest);
        Assert.Same(dest, link.GetDest());
    }

    [Fact]
    public void OpenLinkIsAlwaysOpen()
    {
        Room dest = MakeRoom();
        var link = new OpenLink(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), dest);
        Assert.NotNull(link.GetDest());
        Assert.True(link.TryOpen("anything"));
    }

    // Review Focus: RiddleLink diacritic-insensitive match
    [Fact]
    public void RiddleLinkTryOpenIgnoresDiacriticsAndCase()
    {
        var link = new RiddleLink(
            new("Escalera", "Staircase"),
            new(string.Empty, string.Empty),
            new("¿Cuántos peldaños?", "How many steps?"),
            "Treinta y nueve",
            new("you climb", "you climb"),
            MakeRoom());
        Assert.True(link.TryOpen("treinta y nueve"));
        Assert.True(link.TryOpen("TREINTA Y NUEVE"));
        Assert.True(link.TryOpen("Treinta Y Nueve"));
    }

    [Fact]
    public void RiddleLinkTryOpenReturnsFalseOnWrongAnswer()
    {
        var link = new RiddleLink(
            new("Escalera", "Staircase"),
            new(string.Empty, string.Empty),
            new("¿Cuántos peldaños?", "How many steps?"),
            "Treinta y nueve",
            new("you climb", "you climb"),
            MakeRoom());
        Assert.False(link.TryOpen("cuarenta"));
    }

    [Fact]
    public void RiddleLinkGetDestReturnsNullUntilSolved()
    {
        Room dest = MakeRoom();
        var link = new RiddleLink(
            new("Escalera", "Staircase"),
            new(string.Empty, string.Empty),
            new("?", "?"),
            "Treinta y nueve",
            new(string.Empty, string.Empty),
            dest);
        Assert.Null(link.GetDest());
        link.TryOpen("treinta y nueve");
        Assert.Same(dest, link.GetDest());
    }

    // Review Focus: DangerLink kills without talisman
    [Fact]
    public void DangerLinkPassKillsPlayerWithoutTalisman()
    {
        bool killed = false;
        Inventory bag = MakeBag();
        var link = new DangerLink(
            new("Traquea", "Trachea"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            string.Empty,
            "Talismán de aire",
            MakeRoom(),
            () => killed = true);
        link.Pass(bag);
        Assert.True(killed);
    }

    [Fact]
    public void DangerLinkPassSucceedsWithTalisman()
    {
        bool killed = false;
        Inventory bag = MakeBag();
        bag.Children.Add(new Item(new("Talismán de aire", "Air talisman"), new(string.Empty, string.Empty), 1));
        var link = new DangerLink(
            new("Traquea", "Trachea"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            string.Empty,
            "Talismán de aire",
            MakeRoom(),
            () => killed = true);
        link.Pass(bag);
        Assert.False(killed);
    }

    // Review Focus: DangerLink2 item-must-be-absent
    [Fact]
    public void DangerLink2PassKillsPlayerWithForbiddenItem()
    {
        bool killed = false;
        Inventory bag = MakeBag();
        bag.Children.Add(new Item(new("Talismán de Nieve", "Snow talisman"), new(string.Empty, string.Empty), 1));
        var link = new DangerLink2(
            new("Camino", "Path"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            string.Empty,
            "Talismán de Nieve",
            MakeRoom(),
            () => killed = true);
        link.Pass(bag);
        Assert.True(killed);
    }

    [Fact]
    public void DangerLink2PassSucceedsWithoutForbiddenItem()
    {
        bool killed = false;
        Inventory bag = MakeBag();
        var link = new DangerLink2(
            new("Camino", "Path"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            string.Empty,
            "Talismán de Nieve",
            MakeRoom(),
            () => killed = true);
        link.Pass(bag);
        Assert.False(killed);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
    }

    private static Inventory MakeBag()
    {
        return new();
    }
}
