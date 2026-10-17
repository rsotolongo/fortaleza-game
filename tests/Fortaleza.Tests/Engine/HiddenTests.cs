using Fortaleza.Engine;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class HiddenTests
{
    private static Troll MakeTroll(string likeness) =>
        new(new("Bruja", "Witch"), new("", ""), likeness,
            new("Gracias", "Thank you"), new("Dame la escoba", "Give me the broom"));

    [Fact]
    public void Troll_Accept_SetsHappyOnCorrectGift()
    {
        var troll = new Troll(new("Bruja", "Witch"), new("", ""), "Escoba",
            new("Gracias", "Thank you"), new("Dame mi escoba", "Give me my broom"));
        troll.Accept(new Item(new("Escoba", "Broom"), new("", ""), 1));
        Assert.True(troll.Happy);
    }

    [Fact]
    public void Troll_Accept_IgnoresWrongGift()
    {
        var troll = MakeTroll("Escoba");
        troll.Accept(new Item(new("Maza", "Mace"), new("", ""), 1));
        Assert.False(troll.Happy);
    }

    [Fact]
    public void Guard_Die_ReturnsTrueWithCorrectWeapon()
    {
        var guard = new Guard(new("Ciclope", "Cyclops"), new("", ""),
            new("Dying words", "Dying words"), "Maza");
        Assert.True(guard.Die("Maza"));
    }

    [Fact]
    public void Guard_Die_ReturnsFalseWithWrongWeapon()
    {
        var guard = new Guard(new("Ciclope", "Cyclops"), new("", ""),
            new("Dying", "Dying"), "Maza");
        Assert.False(guard.Die("Espada"));
    }

    [Fact]
    public void Guard_Die_SetsDeadOnCorrectWeapon()
    {
        var guard = new Guard(new("Ciclope", "Cyclops"), new("", ""),
            new("Dying", "Dying"), "Maza");
        guard.Die("Maza");
        Assert.True(guard.Dead);
    }

    [Fact]
    public void Guard_Die_DoesNotSetDeadOnWrongWeapon()
    {
        var guard = new Guard(new("Ciclope", "Cyclops"), new("", ""),
            new("Dying", "Dying"), "Maza");
        guard.Die("Espada");
        Assert.False(guard.Dead);
    }

    [Fact]
    public void Hidden_TryBreak_ReturnsTrueOnMatch()
    {
        var child = new Item(new("Puerta oculta", "Hidden door"), new("", ""), 100);
        var hidden = new Hidden(new("Espejo opaco", "Opaque mirror"), new("", ""), "Hueso de gato", child);
        Assert.True(hidden.TryBreak("Hueso de gato"));
    }

    [Fact]
    public void Hidden_TryBreak_ReturnsFalseOnMismatch()
    {
        var child = new Item(new("Tesoro", "Treasure"), new("", ""), 1);
        var hidden = new Hidden(new("Cofre", "Chest"), new("", ""), "Llave", child);
        Assert.False(hidden.TryBreak("Espada"));
    }

    [Fact]
    public void Room_Break_InsertHiddenChildAtSameIndex()
    {
        var room = new Room(new("", ""), new("", ""), 100);
        var before = new Item(new("A", "A"), new("", ""), 1);
        var hiddenChild = new Item(new("C", "C"), new("", ""), 1);
        var hidden = new Hidden(new("B", "B"), new("", ""), "Hacha", hiddenChild);
        var after = new Item(new("D", "D"), new("", ""), 1);
        room.Children.AddRange([before, hidden, after]);
        room.BreakThing("B", "Hacha");
        Assert.Equal(1, room.Children.IndexOf(hiddenChild));
    }

    [Fact]
    public void Room_Break_RemovesHiddenContainerOnSuccess()
    {
        var room = new Room(new("", ""), new("", ""), 100);
        var hiddenChild = new Item(new("C", "C"), new("", ""), 1);
        var hidden = new Hidden(new("B", "B"), new("", ""), "Hacha", hiddenChild);
        room.Children.Add(hidden);
        room.BreakThing("B", "Hacha");
        Assert.DoesNotContain(hidden, room.Children);
        Assert.Contains(hiddenChild, room.Children);
    }
}
