using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

public sealed class BilingualPuzzleTests
{
    [Theory]
    [InlineData("látigo")]
    [InlineData("whip")]
    public void GuardDieAcceptsWeaponNameInEitherLanguage(string weapon)
    {
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), new("Látigo", "Whip"));

        bool died = guard.Die(weapon);

        Assert.True(died);
    }

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

    [Fact]
    public void LinkTryOpenAlternativeKeyDoesNotOpenWrongWord()
    {
        var link = new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Abrete Sesamo", MakeRoom())
        {
            AltKey = "Open Sesame",
        };

        Assert.False(link.TryOpen("sesame"));
    }

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
