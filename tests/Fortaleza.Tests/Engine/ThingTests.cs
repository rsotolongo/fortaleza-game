using Fortaleza.Engine;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class ThingTests
{
    [Fact]
    public void Thing_Name_ReturnsLocalizedName()
    {
        Localizer.Current = Language.English;
        var item = new Item(new("Maza", "Mace"), new("Es muy pesada.", "It is very heavy."), 39);
        Assert.Equal("Mace", item.Name.Get());
    }

    [Fact]
    public void Item_Weight_StoresCorrectly()
    {
        var item = new Item(new("Maza", "Mace"), new("desc", "desc"), 39);
        Assert.Equal(39, item.Weight);
    }

    [Fact]
    public void LivingThing_Die_SetsDead()
    {
        var npc = new TestLivingThing();
        npc.Die("");
        Assert.True(npc.Dead);
    }

    [Fact]
    public void Thing_Children_StartsEmpty()
    {
        var item = new Item(new("A", "A"), new("", ""), 1);
        Assert.Empty(item.Children);
    }

    [Fact]
    public void Thing_Image_ReturnsDescription()
    {
        Localizer.Current = Language.English;
        var item = new Item(new("Maza", "Mace"), new("Pesada", "Heavy"), 1);
        Assert.Equal("Heavy", item.Image());
    }
}

file sealed class TestLivingThing()
    : LivingThing(new("Test", "Test"), new("", ""), 0)
{
}
