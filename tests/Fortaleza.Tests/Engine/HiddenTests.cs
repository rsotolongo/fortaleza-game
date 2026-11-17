using Fortaleza.Engine;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for hidden.
/// </summary>
public sealed class HiddenTests
{
    /// <summary>
    /// Troll accept sets happy on correct gift.
    /// </summary>
    [Fact]
    public void TrollAcceptSetsHappyOnCorrectGift()
    {
        var troll = new Troll(
            new("Bruja", "Witch"),
            new(string.Empty, string.Empty),
            "Escoba",
            new("Gracias", "Thank you"),
            new("Dame mi escoba", "Give me my broom"));
        troll.Accept(new Item(new("Escoba", "Broom"), new(string.Empty, string.Empty), 1));
        Assert.True(troll.Happy);
    }

    /// <summary>
    /// Troll accept ignores wrong gift.
    /// </summary>
    [Fact]
    public void TrollAcceptIgnoresWrongGift()
    {
        Troll troll = MakeTroll("Escoba");
        troll.Accept(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 1));
        Assert.False(troll.Happy);
    }

    /// <summary>
    /// Guard die returns true with correct weapon.
    /// </summary>
    [Fact]
    public void GuardDieReturnsTrueWithCorrectWeapon()
    {
        var guard = new Guard(
            new("Ciclope", "Cyclops"),
            new(string.Empty, string.Empty),
            new("Dying words", "Dying words"),
            "Maza");
        Assert.True(guard.Die("Maza"));
    }

    /// <summary>
    /// Guard die returns false with wrong weapon.
    /// </summary>
    [Fact]
    public void GuardDieReturnsFalseWithWrongWeapon()
    {
        var guard = new Guard(
            new("Ciclope", "Cyclops"),
            new(string.Empty, string.Empty),
            new("Dying", "Dying"),
            "Maza");
        Assert.False(guard.Die("Espada"));
    }

    /// <summary>
    /// Guard die sets dead on correct weapon.
    /// </summary>
    [Fact]
    public void GuardDieSetsDeadOnCorrectWeapon()
    {
        var guard = new Guard(
            new("Ciclope", "Cyclops"),
            new(string.Empty, string.Empty),
            new("Dying", "Dying"),
            "Maza");
        guard.Die("Maza");
        Assert.True(guard.Dead);
    }

    /// <summary>
    /// Guard die does not set dead on wrong weapon.
    /// </summary>
    [Fact]
    public void GuardDieDoesNotSetDeadOnWrongWeapon()
    {
        var guard = new Guard(
            new("Ciclope", "Cyclops"),
            new(string.Empty, string.Empty),
            new("Dying", "Dying"),
            "Maza");
        guard.Die("Espada");
        Assert.False(guard.Dead);
    }

    /// <summary>
    /// Hidden try break returns true on match.
    /// </summary>
    [Fact]
    public void HiddenTryBreakReturnsTrueOnMatch()
    {
        var child = new Item(new("Puerta oculta", "Hidden door"), new(string.Empty, string.Empty), 100);
        var hidden = new Hidden(new("Espejo opaco", "Opaque mirror"), new(string.Empty, string.Empty), "Hueso de gato", child);
        Assert.True(hidden.TryBreak("Hueso de gato"));
    }

    /// <summary>
    /// Hidden try break returns false on mismatch.
    /// </summary>
    [Fact]
    public void HiddenTryBreakReturnsFalseOnMismatch()
    {
        var child = new Item(new("Tesoro", "Treasure"), new(string.Empty, string.Empty), 1);
        var hidden = new Hidden(new("Cofre", "Chest"), new(string.Empty, string.Empty), "Llave", child);
        Assert.False(hidden.TryBreak("Espada"));
    }

    /// <summary>
    /// Room break insert hidden child at same index.
    /// </summary>
    [Fact]
    public void RoomBreakInsertHiddenChildAtSameIndex()
    {
        var room = new Room(new(string.Empty, string.Empty), new(string.Empty, string.Empty), 100);
        var before = new Item(new("A", "A"), new(string.Empty, string.Empty), 1);
        var hiddenChild = new Item(new("C", "C"), new(string.Empty, string.Empty), 1);
        var hidden = new Hidden(new("B", "B"), new(string.Empty, string.Empty), "Hacha", hiddenChild);
        var after = new Item(new("D", "D"), new(string.Empty, string.Empty), 1);
        room.Children.AddRange([before, hidden, after]);
        room.BreakThing("B", "Hacha");
        Assert.Equal(1, room.Children.IndexOf(hiddenChild));
    }

    /// <summary>
    /// Room break removes hidden container on success.
    /// </summary>
    [Fact]
    public void RoomBreakRemovesHiddenContainerOnSuccess()
    {
        var room = new Room(new(string.Empty, string.Empty), new(string.Empty, string.Empty), 100);
        var hiddenChild = new Item(new("C", "C"), new(string.Empty, string.Empty), 1);
        var hidden = new Hidden(new("B", "B"), new(string.Empty, string.Empty), "Hacha", hiddenChild);
        room.Children.Add(hidden);
        room.BreakThing("B", "Hacha");
        Assert.DoesNotContain(hidden, room.Children);
        Assert.Contains(hiddenChild, room.Children);
    }

    private static Troll MakeTroll(string likeness)
    {
        return new(
            new("Bruja", "Witch"),
            new(string.Empty, string.Empty),
            likeness,
            new("Gracias", "Thank you"),
            new("Dame la escoba", "Give me the broom"));
    }
}
