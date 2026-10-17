using Fortaleza.Engine;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class RoomTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    [Fact]
    public void Room_Get_FindsItemByNormalizedName()
    {
        var room = new Room(new("pasillo", "corridor"), new("", ""), Inventory.LWeight + 1);
        room.Children.Add(new Item(new("Antorcha", "Torch"), new("", ""), 1));
        Assert.NotNull(room.Get("antorcha"));
        Assert.NotNull(room.Get("ANTORCHA"));
    }

    [Fact]
    public void Room_Get_MatchesWithoutDiacritics()
    {
        var room = MakeRoom();
        room.Children.Add(new Item(new("Trébol", "Clover"), new("", ""), 1));
        Assert.NotNull(room.Get("Trebol"));
    }

    [Fact]
    public void Room_Get_ReturnsNullWhenNotFound()
    {
        var room = MakeRoom();
        Assert.Null(room.Get("Fantasma"));
    }

    [Fact]
    public void Room_Take_RemovesItemFromRoom()
    {
        var room = MakeRoom();
        var item = new Item(new("Maza", "Mace"), new("", ""), 1);
        room.Children.Add(item);
        var taken = room.Take("Maza");
        Assert.Same(item, taken);
        Assert.Empty(room.Children);
    }

    [Fact]
    public void Room_Take_ReturnsNullWhenNotFound()
    {
        var room = MakeRoom();
        Assert.Null(room.Take("Fantasma"));
    }

    [Fact]
    public void Room_Visited_DefaultsFalse()
    {
        var room = MakeRoom();
        Assert.False(room.Visited);
    }
}
