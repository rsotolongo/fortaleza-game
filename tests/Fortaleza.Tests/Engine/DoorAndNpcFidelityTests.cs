using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class DoorAndNpcFidelityTests
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

    private static (Room Room, Link Link) RoomWithDoor(string key, Room? destination = null)
    {
        var room = MakeRoom();
        var link = new Link(new("Puerta azul", "Blue door"), new("", ""), new("", ""), key, destination ?? MakeRoom());
        room.Children.Add(link);
        return (room, link);
    }

    [Fact]
    public void OpenDoor_ClosedLinkWithAnyWord_OpensAndSaysOk()
    {
        Localizer.Current = Language.Spanish;
        var (room, link) = RoomWithDoor("");

        var output = Capture(() => room.OpenDoor("puerta azul", ""));

        Assert.True(link.IsOpen);
        Assert.Contains(Localizer.Msg("Ok"), output);
    }

    [Fact]
    public void OpenDoor_AlreadyOpen_SaysNotNecessary()
    {
        Localizer.Current = Language.Spanish;
        var (room, _) = RoomWithDoor(Link.OpenDoor);

        var output = Capture(() => room.OpenDoor("puerta azul", ""));

        Assert.Contains(Localizer.Msg("AlreadyOpen"), output);
    }

    [Fact]
    public void OpenDoor_WrongKey_ReportsFailureAndQuotedKey()
    {
        Localizer.Current = Language.Spanish;
        var (room, link) = RoomWithDoor("secreto");

        var output = Capture(() => room.OpenDoor("puerta azul", "mal"));

        Assert.False(link.IsOpen);
        Assert.Contains(Localizer.Msg("TryOpenFail"), output);
        Assert.Contains("\"mal\"" + Localizer.Msg("WrongPassword"), output);
    }

    [Fact]
    public void OpenDoor_NoKeyGivenForLockedDoor_AsksForPassword()
    {
        Localizer.Current = Language.Spanish;
        var (room, _) = RoomWithDoor("secreto");

        var output = Capture(() => room.OpenDoor("puerta azul", ""));

        Assert.Contains(Localizer.Msg("TryOpenFail"), output);
        Assert.Contains(Localizer.Msg("NeedPassword"), output);
    }

    [Fact]
    public void OpenDoor_TwinDoorInDestination_IsOpenedToo()
    {
        var destination = MakeRoom();
        var twin = new Link(new("Puerta azul", "Blue door"), new("", ""), new("", ""), "", MakeRoom());
        destination.Children.Add(twin);
        var (room, _) = RoomWithDoor("", destination);

        room.OpenDoor("puerta azul", "");

        Assert.True(twin.IsOpen);
    }

    [Fact]
    public void OpenDoor_NoTwinDoor_WarnsItIsOneWay()
    {
        Localizer.Current = Language.Spanish;
        var (room, _) = RoomWithDoor("");

        var output = Capture(() => room.OpenDoor("puerta azul", ""));

        Assert.Contains(Localizer.Msg("DoorWarning"), output);
    }

    [Fact]
    public void OpenDoor_WrongRiddleAnswer_DoesNotAddGenericFailureText()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        room.Children.Add(new RiddleLink(new("Escalera", "Staircase"), new("", ""),
            new("¿Cuántos?", "How many?"), "treinta", new("", ""), MakeRoom()));

        var output = Capture(() => room.OpenDoor("escalera", "cuarenta"));

        Assert.Contains(Localizer.Msg("WrongAnswer"), output);
        Assert.DoesNotContain(Localizer.Msg("TryOpenFail"), output);
    }

    [Fact]
    public void OpenDoor_ThingIsNotALink_SaysCannotDoThat()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 1));

        var output = Capture(() => room.OpenDoor("maza", ""));

        Assert.Contains(Localizer.Msg("CantDo"), output);
    }

    [Fact]
    public void OpenDoor_NothingByThatName_SaysCannotDoThatAndNoSuchThing()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();

        var output = Capture(() => room.OpenDoor("puerta", ""));

        Assert.Contains(
            Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + "puerta" + Localizer.Msg("InTheRoom"),
            output);
    }

    [Fact]
    public void PlayerOpenDoor_WithKey_InvokesMagicWordsFirst()
    {
        Localizer.Current = Language.Spanish;
        var (room, _) = RoomWithDoor("secreto");
        var player = new Player(room);

        var output = Capture(() => player.OpenDoor("puerta azul", "SECRETO"));

        Assert.Contains(Localizer.Msg("MagicWords"), output);
        Assert.Contains("SECRETO!!!", output);
    }

    [Fact]
    public void BreakThing_Success_ShowsCrashAndSurprise()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        var inner = new Item(new("Llave", "Key"), new("", ""), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new("", ""), "Maza", inner));

        var output = Capture(() => room.BreakThing("jarron", "maza"));

        Assert.Contains(Localizer.Msg("Crash"), output);
        Assert.Contains(Localizer.Msg("Surprise"), output);
    }

    [Fact]
    public void BreakThing_WrongWeapon_SaysCannotBreak()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();
        var inner = new Item(new("Llave", "Key"), new("", ""), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new("", ""), "Maza", inner));

        var output = Capture(() => room.BreakThing("jarron", "soga"));

        Assert.Contains(Localizer.Msg("CantBreak"), output);
    }

    [Fact]
    public void BreakThing_NothingByThatName_SaysCannotDoThatAndNoSuchThing()
    {
        Localizer.Current = Language.Spanish;
        var room = MakeRoom();

        var output = Capture(() => room.BreakThing("jarron", "maza"));

        Assert.Contains(
            Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + "jarron" + Localizer.Msg("InTheRoom"),
            output);
    }

    [Fact]
    public void Hidden_TryBreak_AcceptsPartialWeaponName()
    {
        var hidden = new Hidden(new("Espejo", "Mirror"), new("", ""), "Hueso de gato",
            new Item(new("a", "a"), new("", ""), 1));

        Assert.True(hidden.TryBreak("hueso"));
        Assert.False(hidden.TryBreak(""));
    }

    [Fact]
    public void Hidden_EmptyBreaker_BreaksWithAnything()
    {
        var hidden = new Hidden(new("Pared", "Wall"), new("", ""), "",
            new Item(new("a", "a"), new("", ""), 1));

        Assert.True(hidden.TryBreak(""));
        Assert.True(hidden.TryBreak("cualquier cosa"));
    }

    [Fact]
    public void Troll_Die_ScreamsAndDies()
    {
        Localizer.Current = Language.Spanish;
        var troll = new Troll(new("Bruja", "Witch"), new("", ""), "Escoba", new("", ""), new("", ""));

        var output = Capture(() => troll.Die(""));

        Assert.True(troll.Dead);
        Assert.Contains(Localizer.Msg("Ahhhh"), output);
    }

    [Fact]
    public void Troll_Accept_WrongGiftAfterRightOne_MakesTrollUnhappyAgain()
    {
        var troll = new Troll(new("Bruja", "Witch"), new("", ""), "Escoba", new("", ""), new("", ""));

        troll.Accept(new Item(new("Escoba", "Broom"), new("", ""), 1));
        troll.Accept(new Item(new("Maza", "Mace"), new("", ""), 1));

        Assert.False(troll.Happy);
    }

    [Fact]
    public void Guard_Die_WrongWeapon_ShowsUselessAndSurvives()
    {
        Localizer.Current = Language.Spanish;
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("grr", "grr"), "Látigo");
        var died = true;

        var output = Capture(() => died = guard.Die("maza"));

        Assert.False(died);
        Assert.False(guard.Dead);
        Assert.Contains(Localizer.Msg("Useless"), output);
    }

    [Fact]
    public void Daughter_Die_RightWeapon_ConfessesAndDies()
    {
        var killed = false;
        var daughter = new Daughter(new("Hija", "Daughter"), new("", ""), new("confieso", "I confess"),
            "Daga", () => killed = true);

        var died = daughter.Die("daga");

        Assert.True(died);
        Assert.True(daughter.Dead);
        Assert.False(killed);
    }

    [Fact]
    public void Daughter_Die_WrongWeapon_KillsThePlayerInstead()
    {
        var killed = false;
        var daughter = new Daughter(new("Hija", "Daughter"), new("", ""), new("confieso", "I confess"),
            "Daga", () => killed = true);

        var died = daughter.Die("maza");

        Assert.False(died);
        Assert.False(daughter.Dead);
        Assert.True(killed);
    }
}
