using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class BilingualPuzzleTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    [Theory]
    [InlineData("látigo")]
    [InlineData("whip")]
    public void Guard_Die_AcceptsWeaponNameInEitherLanguage(string weapon)
    {
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("", ""), new("Látigo", "Whip"));

        var died = guard.Die(weapon);

        Assert.True(died);
    }

    [Theory]
    [InlineData("hueso")]
    [InlineData("cat bone")]
    public void Hidden_TryBreak_AcceptsBreakerNameInEitherLanguage(string weapon)
    {
        var hidden = new Hidden(new("Espejo", "Mirror"), new("", ""), new("Hueso de gato", "Cat bone"),
            new Item(new("a", "a"), new("", ""), 1));

        Assert.True(hidden.TryBreak(weapon));
    }

    [Theory]
    [InlineData("daga")]
    [InlineData("dagger")]
    public void Daughter_Die_AcceptsWeaponNameInEitherLanguage(string weapon)
    {
        var killed = false;
        var daughter = new Daughter(new("Hija", "Daughter"), new("", ""), new("", ""),
            new("Daga", "Dagger"), () => killed = true);

        var died = daughter.Die(weapon);

        Assert.True(died);
        Assert.False(killed);
    }

    [Theory]
    [InlineData("Abrete Sesamo")]
    [InlineData("open sesame")]
    public void Link_TryOpen_AcceptsAlternativeKey(string typed)
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "Abrete Sesamo", MakeRoom())
        {
            AltKey = "Open Sesame"
        };

        Assert.True(link.TryOpen(typed));
    }

    [Fact]
    public void Link_TryOpen_AlternativeKeyDoesNotOpenWrongWord()
    {
        var link = new Link(new("Puerta", "Door"), new("", ""), new("", ""), "Abrete Sesamo", MakeRoom())
        {
            AltKey = "Open Sesame"
        };

        Assert.False(link.TryOpen("sesame"));
    }

    [Theory]
    [InlineData("seis")]
    [InlineData("SIX")]
    public void RiddleLink_TryOpen_AcceptsAlternativeAnswer(string typed)
    {
        var link = new RiddleLink(new("Puerta", "Door"), new("", ""), new("¿Cuántas?", "How many?"),
            "Seis", new("", ""), MakeRoom())
        {
            AltAnswer = "Six"
        };

        Assert.True(link.TryOpen(typed));
    }
}
