using System.Globalization;
using Fortaleza.Engine;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for weigh.
/// </summary>
public sealed class WeighTests
{
    /// <summary>
    /// Weigh without scale says so.
    /// </summary>
    [Fact]
    public void WeighWithoutScaleSaysSo()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        string output = Capture(() => player.Weigh("maza"));

        Assert.Contains(Localizer.Msg("NoScale"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Weigh item in room shows weight with singular or plural.
    /// </summary>
    [Theory]
    [InlineData(1, "BagUnit")]
    [InlineData(5, "BagUnits")]
    public void WeighItemInRoomShowsWeightWithSingularOrPlural(int weight, string unitKey)
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), weight));
        Player player = PlayerWithScale(room);

        string output = Capture(() => player.Weigh("maza"));

        Assert.Contains(weight.ToString(CultureInfo.InvariantCulture) + Localizer.Msg(unitKey), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Weigh item in bag is found in bag.
    /// </summary>
    [Fact]
    public void WeighItemInBagIsFoundInBag()
    {
        Localizer.Current = Language.Spanish;
        Player player = PlayerWithScale(MakeRoom());
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 7));

        string output = Capture(() => player.Weigh("maza"));

        Assert.Contains("7" + Localizer.Msg("BagUnits"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Weigh the scale itself tries in vain.
    /// </summary>
    [Fact]
    public void WeighTheScaleItselfTriesInVain()
    {
        Localizer.Current = Language.Spanish;
        Player player = PlayerWithScale(MakeRoom());

        string output = Capture(() => player.Weigh("balanza"));

        Assert.Contains(Localizer.Msg("WeighSelf"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Weigh non item calls player mad.
    /// </summary>
    [Fact]
    public void WeighNonItemCallsPlayerMad()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        room.Children.Add(new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "Látigo"));
        Player player = PlayerWithScale(room);

        string output = Capture(() => player.Weigh("lobo"));

        Assert.Contains(Localizer.Msg("WeighCrazy"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Weigh nothing found explains why.
    /// </summary>
    [Theory]
    [InlineData("", "Complete")]
    [InlineData("yunque", "WeighNotHere")]
    public void WeighNothingFoundExplainsWhy(string what, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        Player player = PlayerWithScale(MakeRoom());

        string output = Capture(() => player.Weigh(what));

        Assert.Contains(Localizer.Msg(messageKey), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Leave all empty bag says nothing is carried.
    /// </summary>
    [Fact]
    public void LeaveAllEmptyBagSaysNothingIsCarried()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        string output = Capture(player.LeaveAll);

        Assert.Contains(Localizer.Msg("BagEmpty"), output, StringComparison.Ordinal);
        Assert.DoesNotContain(Localizer.Msg("DropAll"), output, StringComparison.Ordinal);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
    }

    private static string Capture(Action action)
    {
        var writer = new StringWriter();
        TextWriter original = Console.Out;
        Console.SetOut(writer);
        try
        { action(); }
        finally { Console.SetOut(original); }
        return writer.ToString().Replace(Environment.NewLine, " ", StringComparison.Ordinal);
    }

    private static Player PlayerWithScale(Room room)
    {
        var player = new Player(room);
        player.Bag.Children.Add(new Item(new("Balanza", "Scale"), new(string.Empty, string.Empty), 2));
        return player;
    }
}
