using Fortaleza.Engine;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for room.
/// </summary>
public sealed class RoomTests
{
    /// <summary>
    /// Room get finds item by normalized name.
    /// </summary>
    [Fact]
    public void RoomGetFindsItemByNormalizedName()
    {
        var room = new Room(new("pasillo", "corridor"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
        room.Children.Add(new Item(new("Antorcha", "Torch"), new(string.Empty, string.Empty), 1));
        Assert.NotNull(room.Get("antorcha"));
        Assert.NotNull(room.Get("ANTORCHA"));
    }

    /// <summary>
    /// Room get matches without diacritics.
    /// </summary>
    [Fact]
    public void RoomGetMatchesWithoutDiacritics()
    {
        Room room = MakeRoom();
        room.Children.Add(new Item(new("Trébol", "Clover"), new(string.Empty, string.Empty), 1));
        Assert.NotNull(room.Get("Trebol"));
    }

    /// <summary>
    /// Room get returns null when not found.
    /// </summary>
    [Fact]
    public void RoomGetReturnsNullWhenNotFound()
    {
        Room room = MakeRoom();
        Assert.Null(room.Get("Fantasma"));
    }

    /// <summary>
    /// Room take removes item from room.
    /// </summary>
    [Fact]
    public void RoomTakeRemovesItemFromRoom()
    {
        Room room = MakeRoom();
        var item = new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 1);
        room.Children.Add(item);
        Thing? taken = room.Take("Maza");
        Assert.Same(item, taken);
        Assert.Empty(room.Children);
    }

    /// <summary>
    /// Room take returns null when not found.
    /// </summary>
    [Fact]
    public void RoomTakeReturnsNullWhenNotFound()
    {
        Room room = MakeRoom();
        Assert.Null(room.Take("Fantasma"));
    }

    /// <summary>
    /// Room visited defaults false.
    /// </summary>
    [Fact]
    public void RoomVisitedDefaultsFalse()
    {
        Room room = MakeRoom();
        Assert.False(room.Visited);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
    }
}
