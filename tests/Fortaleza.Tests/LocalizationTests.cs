using Fortaleza.Localization;

namespace Fortaleza.Tests;

/// <summary>
/// Tests for localization.
/// </summary>
public sealed class LocalizationTests
{
    /// <summary>
    /// Localized text get returns spanish when current is spanish.
    /// </summary>
    [Fact]
    public void LocalizedTextGetReturnsSpanishWhenCurrentIsSpanish()
    {
        Localizer.Current = Language.Spanish;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hola", t.Get());
    }

    /// <summary>
    /// Localized text get returns english when current is english.
    /// </summary>
    [Fact]
    public void LocalizedTextGetReturnsEnglishWhenCurrentIsEnglish()
    {
        Localizer.Current = Language.English;
        var t = new LocalizedText("Hola", "Hello");
        Assert.Equal("Hello", t.Get());
    }

    /// <summary>
    /// Localizer msg returns correct spanish string.
    /// </summary>
    [Fact]
    public void LocalizerMsgReturnsCorrectSpanishString()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    /// <summary>
    /// Localizer msg returns correct english string.
    /// </summary>
    [Fact]
    public void LocalizerMsgReturnsCorrectEnglishString()
    {
        Localizer.Current = Language.English;
        Assert.Equal("Ok.", Localizer.Msg("Ok"));
    }

    /// <summary>
    /// Localizer msg returns spanish message for known key.
    /// </summary>
    [Fact]
    public void LocalizerMsgReturnsSpanishMessageForKnownKey()
    {
        Localizer.Current = Language.Spanish;
        Assert.Equal("Usted ve:", Localizer.Msg("RoomContains"));
    }

    /// <summary>
    /// Localizer msg returns english message for known key.
    /// </summary>
    [Fact]
    public void LocalizerMsgReturnsEnglishMessageForKnownKey()
    {
        Localizer.Current = Language.English;
        Assert.Equal("You see:", Localizer.Msg("RoomContains"));
    }
}
