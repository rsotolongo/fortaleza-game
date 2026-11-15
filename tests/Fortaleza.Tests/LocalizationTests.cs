using Fortaleza.Localization;

namespace Fortaleza.Tests;

public sealed class LocalizationTests
{
    [Fact]
    public void LocalizedTextGetReturnsSpanishWhenCurrentIsSpanish()
    {
        Localizer.Current = Language.Spanish;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hola", t.Get());
    }

    [Fact]
    public void LocalizedTextGetReturnsEnglishWhenCurrentIsEnglish()
    {
        Localizer.Current = Language.English;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hello", t.Get());
    }

    [Fact]
    public void LocalizerMsgReturnsCorrectSpanishString()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    [Fact]
    public void LocalizerMsgReturnsCorrectEnglishString()
    {
        Localizer.Current = Language.English;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    [Fact]
    public void LocalizerMsgReturnsSpanishMessageForKnownKey()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Usted ve:", Localizer.Msg("RoomContains"));
    }

    [Fact]
    public void LocalizerMsgReturnsEnglishMessageForKnownKey()
    {
        Localizer.Current = Language.English;
        Assert.Equal("You see:", Localizer.Msg("RoomContains"));
    }
}
