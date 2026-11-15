using Fortaleza.Engine;

namespace Fortaleza.Tests.Engine;

public sealed class InventoryTests
{
    [Fact]
    public void InventoryHeavinessSumsWeights()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 10));
        inv.Children.Add(new Item(new("B", "B"), new(string.Empty, string.Empty), 15));
        Assert.Equal(25, inv.Heaviness);
    }

    [Fact]
    public void InventoryCanAddFalseWhenOverLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 35));
        Assert.False(inv.CanAdd(6));
    }

    [Fact]
    public void InventoryCanAddTrueAtExactLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 35));
        Assert.True(inv.CanAdd(5));
    }

    [Fact]
    public void InventoryIsEmptyTrueWhenNoChildren()
    {
        var inv = new Inventory();
        Assert.True(inv.IsEmpty);
    }

    [Fact]
    public void InventoryIsEmptyFalseWhenHasChildren()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 1));
        Assert.False(inv.IsEmpty);
    }

    [Fact]
    public void InventoryLWeightIs40()
    {
        Assert.Equal(40, Inventory.LWeight);
    }
}
