using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for bilingual puzzle.
/// </summary>
public sealed class BilingualPuzzleTests
{
    /// <summary>
    /// Guard die accepts weapon name in either language.
    /// </summary>
    [Theory]
    [InlineData("látigo")]
    [InlineData("whip")]
    public void GuardDieAcceptsWeaponNameInEitherLanguage(string weapon)
    {
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), new("Látigo", "Whip"));

        bool died = guard.Die(weapon);

        Assert.True(died);
    }

    /// <summary>
    /// Hidden try break accepts breaker name in either language.
    /// </summary>
    [Theory]
    [InlineData("hueso")]
    [InlineData("cat bone")]
    public void HiddenTryBreakAcceptsBreakerNameInEitherLanguage(string weapon)
    {
        var hidden = new Hidden(
            new("Espejo", "Mirror"),
            new(string.Empty, string.Empty),
            new("Hueso de gato", "Cat bone"),
            new Item(new("a", "a"), new(string.Empty, string.Empty), 1));

        Assert.True(hidden.TryBreak(weapon));
    }

    /// <summary>
    /// Daughter die accepts weapon name in either language.
    /// </summary>
    [Theory]
    [InlineData("daga")]
    [InlineData("dagger")]
    public void DaughterDieAcceptsWeaponNameInEitherLanguage(string weapon)
    {
        bool killed = false;
        var daughter = new Daughter(
            new("Hija", "Daughter"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            new("Daga", "Dagger"),
            () => killed = true);

        bool died = daughter.Die(weapon);

        Assert.True(died);
        Assert.False(killed);
    }

    /// <summary>
    /// Link try open accepts alternative key.
    /// </summary>
    [Theory]
    [InlineData("Abrete Sesamo")]
    [InlineData("open sesame")]
    public void LinkTryOpenAcceptsAlternativeKey(string typed)
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Abrete Sesamo", MakeRoom())
        {
            AltKey = "Open Sesame",
        };

        Assert.True(link.TryOpen(typed));
    }

    /// <summary>
    /// Link try open alternative key does not open wrong word.
    /// </summary>
    [Fact]
    public void LinkTryOpenAlternativeKeyDoesNotOpenWrongWord()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Abrete Sesamo", MakeRoom())
        {
            AltKey = "Open Sesame",
        };

        Assert.False(link.TryOpen("sesame"));
    }

    /// <summary>
    /// Riddle link try open accepts alternative answer.
    /// </summary>
    [Theory]
    [InlineData("seis")]
    [InlineData("SIX")]
    public void RiddleLinkTryOpenAcceptsAlternativeAnswer(string typed)
    {
        var link = new RiddleLink(
            new("Puerta", "Door"),
            new(string.Empty, string.Empty),
            new("¿Cuántas?", "How many?"),
            "Seis",
            new(string.Empty, string.Empty),
            MakeRoom())
        {
            AltAnswer = "Six",
        };

        Assert.True(link.TryOpen(typed));
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
    }
}
