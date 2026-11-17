using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for link.
/// </summary>
public sealed class LinkTests
{
    /// <summary>
    /// Link try open succeeds with empty key.
    /// </summary>
    [Fact]
    public void LinkTryOpenSucceedsWithEmptyKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new("you pass", "you pass"), string.Empty, MakeRoom());
        Assert.True(link.TryOpen(string.Empty));
    }

    /// <summary>
    /// Link try open succeeds with correct key.
    /// </summary>
    [Fact]
    public void LinkTryOpenSucceedsWithCorrectKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", MakeRoom());
        Assert.True(link.TryOpen("Fortaleza"));
        Assert.True(link.TryOpen("FORTALEZA"));
    }

    /// <summary>
    /// Link try open fails with wrong key.
    /// </summary>
    [Fact]
    public void LinkTryOpenFailsWithWrongKey()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", MakeRoom());
        Assert.False(link.TryOpen("wrong"));
    }

    /// <summary>
    /// Link get dest returns null when closed.
    /// </summary>
    [Fact]
    public void LinkGetDestReturnsNullWhenClosed()
    {
        Room dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Fortaleza", dest);
        Assert.Null(link.GetDest());
    }

    /// <summary>
    /// Link get dest returns room when open.
    /// </summary>
    [Fact]
    public void LinkGetDestReturnsRoomWhenOpen()
    {
        Room dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), Link.OpenDoor, dest);
        Assert.Same(dest, link.GetDest());
    }

    /// <summary>
    /// Open link is always open.
    /// </summary>
    [Fact]
    public void OpenLinkIsAlwaysOpen()
    {
        Room dest = MakeRoom();
        var link = new OpenLink(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), dest);
        Assert.NotNull(link.GetDest());
        Assert.True(link.TryOpen("anything"));
    }

    // Review Focus: RiddleLink diacritic-insensitive match
    /// <summary>
    /// Riddle link try open ignores diacritics and case.
    /// </summary>
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

    /// <summary>
    /// Riddle link try open returns false on wrong answer.
    /// </summary>
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

    /// <summary>
    /// Riddle link get dest returns null until solved.
    /// </summary>
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
    /// <summary>
    /// Danger link pass kills player without talisman.
    /// </summary>
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

    /// <summary>
    /// Danger link pass succeeds with talisman.
    /// </summary>
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
    /// <summary>
    /// Danger link2 pass kills player with forbidden item.
    /// </summary>
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

    /// <summary>
    /// Danger link2 pass succeeds without forbidden item.
    /// </summary>
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
