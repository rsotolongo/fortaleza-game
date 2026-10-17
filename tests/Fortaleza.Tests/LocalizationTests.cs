using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests;

public class LocalizationTests
{
    [Fact]
    public void LocalizedText_Get_ReturnsSpanishWhenCurrentIsSpanish()
    {
        Localizer.Current = Language.Spanish;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hola", t.Get());
    }

    [Fact]
    public void LocalizedText_Get_ReturnsEnglishWhenCurrentIsEnglish()
    {
        Localizer.Current = Language.English;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hello", t.Get());
    }

    [Fact]
    public void Localizer_Msg_ReturnsCorrectSpanishString()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    [Fact]
    public void Localizer_Msg_ReturnsCorrectEnglishString()
    {
        Localizer.Current = Language.English;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    [Fact]
    public void Localizer_Msg_ReturnsSpanishMessageForKnownKey()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Usted ve:", Localizer.Msg("RoomContains"));
    }

    [Fact]
    public void Localizer_Msg_ReturnsEnglishMessageForKnownKey()
    {
        Localizer.Current = Language.English;
        Assert.Equal("You see:", Localizer.Msg("RoomContains"));
    }
}
