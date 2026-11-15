using Fortaleza.Engine;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

public sealed class ThingTests
{
    [Fact]
    public void ThingNameReturnsLocalizedName()
    {
        Localizer.Current = Language.English;
        var item = new Item(new("Maza", "Mace"), new("Es muy pesada.", "It is very heavy."), 39);
        Assert.Equal("Mace", item.Name.Get());
    }

    [Fact]
    public void ItemWeightStoresCorrectly()
    {
        var item = new Item(new("Maza", "Mace"), new("desc", "desc"), 39);
        Assert.Equal(39, item.Weight);
    }

    [Fact]
    public void LivingThingDieSetsDead()
    {
        var npc = new TestLivingThing();
        npc.Die(string.Empty);
        Assert.True(npc.Dead);
    }

    [Fact]
    public void ThingChildrenStartsEmpty()
    {
        var item = new Item(new("A", "A"), new(string.Empty, string.Empty), 1);
        Assert.Empty(item.Children);
    }

    [Fact]
    public void ThingImageReturnsDescription()
    {
        Localizer.Current = Language.English;
        var item = new Item(new("Maza", "Mace"), new("Pesada", "Heavy"), 1);
        Assert.Equal("Heavy", item.Image());
    }
}

file sealed class TestLivingThing()
    : LivingThing(new("Test", "Test"), new(string.Empty, string.Empty), 0);
