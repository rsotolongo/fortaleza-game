using Fortaleza.Engine;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class WeighTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    private static string Capture(Action action)
    {
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try { action(); }
        finally { Console.SetOut(original); }
        return writer.ToString().Replace(Environment.NewLine, " ");
    }

    private static Player PlayerWithScale(Room room)
    {
        var player = new Player(room);
        player.Bag.Children.Add(new Item(new("Balanza", "Scale"), new("", ""), 2));
        return player;
    }

    [Fact]
    public void Weigh_WithoutScale_SaysSo()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        var output = Capture(() => player.Weigh("maza"));

        Assert.Contains(Localizer.Msg("NoScale"), output);
    }

    [Theory]
    [InlineData(1, "BagUnit")]
    [InlineData(5, "BagUnits")]
    public void Weigh_ItemInRoom_ShowsWeightWithSingularOrPlural(int weight, string unitKey)
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), weight));
        var player = PlayerWithScale(room);

        var output = Capture(() => player.Weigh("maza"));

        Assert.Contains(weight + Localizer.Msg(unitKey), output);
    }

    [Fact]
    public void Weigh_ItemInBag_IsFoundInBag()
    {
        Localizer.Current = Language.Spanish;
        var player = PlayerWithScale(MakeRoom());
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 7));

        var output = Capture(() => player.Weigh("maza"));

        Assert.Contains("7" + Localizer.Msg("BagUnits"), output);
    }

    [Fact]
    public void Weigh_TheScaleItself_TriesInVain()
    {
        Localizer.Current = Language.Spanish;
        var player = PlayerWithScale(MakeRoom());

        var output = Capture(() => player.Weigh("balanza"));

        Assert.Contains(Localizer.Msg("WeighSelf"), output);
    }

    [Fact]
    public void Weigh_NonItem_CallsPlayerMad()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        room.Children.Add(new Guard(new("Lobo", "Wolf"), new("", ""), new("", ""), "Látigo"));
        var player = PlayerWithScale(room);

        var output = Capture(() => player.Weigh("lobo"));

        Assert.Contains(Localizer.Msg("WeighCrazy"), output);
    }

    [Theory]
    [InlineData("", "Complete")]
    [InlineData("yunque", "WeighNotHere")]
    public void Weigh_NothingFound_ExplainsWhy(string what, string messageKey)
    {
        Localizer.Current = Language.Spanish;
        var player = PlayerWithScale(MakeRoom());

        var output = Capture(() => player.Weigh(what));

        Assert.Contains(Localizer.Msg(messageKey), output);
    }

    [Fact]
    public void LeaveAll_EmptyBag_SaysNothingIsCarried()
    {
        Localizer.Current = Language.Spanish;
        var player = new Player(MakeRoom());

        var output = Capture(player.LeaveAll);

        Assert.Contains(Localizer.Msg("BagEmpty"), output);
        Assert.DoesNotContain(Localizer.Msg("DropAll"), output);
    }
}
