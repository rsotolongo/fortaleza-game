using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class LinkTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    private static Inventory MakeBag() => new();

    [Fact]
    public void Link_TryOpen_SucceedsWithEmptyKey()
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("you pass", "you pass"), "", MakeRoom());
        Assert.True(link.TryOpen(""));
    }

    [Fact]
    public void Link_TryOpen_SucceedsWithCorrectKey()
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "Fortaleza", MakeRoom());
        Assert.True(link.TryOpen("Fortaleza"));
        Assert.True(link.TryOpen("FORTALEZA"));
    }

    [Fact]
    public void Link_TryOpen_FailsWithWrongKey()
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "Fortaleza", MakeRoom());
        Assert.False(link.TryOpen("wrong"));
    }

    [Fact]
    public void Link_GetDest_ReturnsNullWhenClosed()
    {
        var dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "Fortaleza", dest);
        Assert.Null(link.GetDest());
    }

    [Fact]
    public void Link_GetDest_ReturnsRoomWhenOpen()
    {
        var dest = MakeRoom();
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "", dest);
        Assert.Same(dest, link.GetDest());
    }

    [Fact]
    public void OpenLink_IsAlwaysOpen()
    {
        var dest = MakeRoom();
        var link = new OpenLink(new("Puerta", "Door"), new("", ""), new("", ""), dest);
        Assert.NotNull(link.GetDest());
        Assert.True(link.TryOpen("anything"));
    }

    // Review Focus: RiddleLink diacritic-insensitive match
    [Fact]
    public void RiddleLink_TryOpen_IgnoresDiacriticsAndCase()
    {
        var link = new RiddleLink(new("Escalera", "Staircase"), new("", ""),
            new("¿Cuántos peldaños?", "How many steps?"), "Treinta y nueve",
            new("you climb", "you climb"), MakeRoom());
        Assert.True(link.TryOpen("treinta y nueve"));
        Assert.True(link.TryOpen("TREINTA Y NUEVE"));
        Assert.True(link.TryOpen("Treinta Y Nueve"));
    }

    [Fact]
    public void RiddleLink_TryOpen_ReturnsFalseOnWrongAnswer()
    {
        var link = new RiddleLink(new("Escalera", "Staircase"), new("", ""),
            new("¿Cuántos peldaños?", "How many steps?"), "Treinta y nueve",
            new("you climb", "you climb"), MakeRoom());
        Assert.False(link.TryOpen("cuarenta"));
    }

    [Fact]
    public void RiddleLink_GetDest_ReturnsNullUntilSolved()
    {
        var dest = MakeRoom();
        var link = new RiddleLink(new("Escalera", "Staircase"), new("", ""),
            new("?", "?"), "Treinta y nueve", new("", ""), dest);
        Assert.Null(link.GetDest());
        link.TryOpen("treinta y nueve");
        Assert.Same(dest, link.GetDest());
    }

    // Review Focus: DangerLink kills without talisman
    [Fact]
    public void DangerLink_Pass_KillsPlayerWithoutTalisman()
    {
        var killed = false;
        var bag = MakeBag();
        var link = new DangerLink(new("Traquea", "Trachea"), new("", ""),
            new("", ""), "", "Talismán de aire", MakeRoom(), () => killed = true);
        link.Pass(bag);
        Assert.True(killed);
    }

    [Fact]
    public void DangerLink_Pass_SucceedsWithTalisman()
    {
        var killed = false;
        var bag = MakeBag();
        bag.Children.Add(new Item(new("Talismán de aire", "Air talisman"), new("", ""), 1));
        var link = new DangerLink(new("Traquea", "Trachea"), new("", ""),
            new("", ""), "", "Talismán de aire", MakeRoom(), () => killed = true);
        link.Pass(bag);
        Assert.False(killed);
    }

    // Review Focus: DangerLink2 item-must-be-absent
    [Fact]
    public void DangerLink2_Pass_KillsPlayerWithForbiddenItem()
    {
        var killed = false;
        var bag = MakeBag();
        bag.Children.Add(new Item(new("Talismán de Nieve", "Snow talisman"), new("", ""), 1));
        var link = new DangerLink2(new("Camino", "Path"), new("", ""),
            new("", ""), "", "Talismán de Nieve", MakeRoom(), () => killed = true);
        link.Pass(bag);
        Assert.True(killed);
    }

    [Fact]
    public void DangerLink2_Pass_SucceedsWithoutForbiddenItem()
    {
        var killed = false;
        var bag = MakeBag();
        var link = new DangerLink2(new("Camino", "Path"), new("", ""),
            new("", ""), "", "Talismán de Nieve", MakeRoom(), () => killed = true);
        link.Pass(bag);
        Assert.False(killed);
    }
}
