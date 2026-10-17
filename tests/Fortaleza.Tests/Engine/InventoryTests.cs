using Fortaleza.Engine;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class InventoryTests
{
    [Fact]
    public void Inventory_Heaviness_SumsWeights()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new("", ""), 10));
        inv.Children.Add(new Item(new("B", "B"), new("", ""), 15));
        Assert.Equal(25, inv.Heaviness);
    }

    [Fact]
    public void Inventory_CanAdd_FalseWhenOverLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new("", ""), 35));
        Assert.False(inv.CanAdd(6));
    }

    [Fact]
    public void Inventory_CanAdd_TrueAtExactLimit()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new("", ""), 35));
        Assert.True(inv.CanAdd(5));
    }

    [Fact]
    public void Inventory_IsEmpty_TrueWhenNoChildren()
    {
        var inv = new Inventory();
        Assert.True(inv.IsEmpty);
    }

    [Fact]
    public void Inventory_IsEmpty_FalseWhenHasChildren()
    {
        var inv = new Inventory();
        inv.Children.Add(new Item(new("A", "A"), new("", ""), 1));
        Assert.False(inv.IsEmpty);
    }

    [Fact]
    public void Inventory_LWeight_Is40()
    {
        Assert.Equal(40, Inventory.LWeight);
    }
}
