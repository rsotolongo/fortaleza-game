using Fortaleza.Engine;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for inventory.
/// </summary>
public sealed class InventoryTests
{
    /// <summary>
    /// Inventory heaviness sums weights.
    /// </summary>
    [Fact]
    public void InventoryHeavinessSumsWeights()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 10));
        inv.Children.Add(new Item(new("B", "B"), new(string.Empty, string.Empty), 15));
        Assert.Equal(25, inv.Heaviness);
    }

    /// <summary>
    /// Inventory can add false when over limit.
    /// </summary>
    [Fact]
    public void InventoryCanAddFalseWhenOverLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 35));
        Assert.False(inv.CanAdd(6));
    }

    /// <summary>
    /// Inventory can add true at exact limit.
    /// </summary>
    [Fact]
    public void InventoryCanAddTrueAtExactLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 35));
        Assert.True(inv.CanAdd(5));
    }

    /// <summary>
    /// Inventory is empty true when no children.
    /// </summary>
    [Fact]
    public void InventoryIsEmptyTrueWhenNoChildren()
    {
        var inv = new Inventory();
        Assert.True(inv.IsEmpty);
    }

    /// <summary>
    /// Inventory is empty false when has children.
    /// </summary>
    [Fact]
    public void InventoryIsEmptyFalseWhenHasChildren()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new(string.Empty, string.Empty), 1));
        Assert.False(inv.IsEmpty);
    }

    /// <summary>
    /// Inventory l weight is40.
    /// </summary>
    [Fact]
    public void InventoryLWeightIs40()
    {
        Assert.Equal(40, Inventory.LWeight);
    }
}
