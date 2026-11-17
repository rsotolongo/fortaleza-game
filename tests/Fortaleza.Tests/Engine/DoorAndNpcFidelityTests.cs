using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for door and npc fidelity.
/// </summary>
public sealed class DoorAndNpcFidelityTests
{
    /// <summary>
    /// Open door closed link with any word opens and says ok.
    /// </summary>
    [Fact]
    public void OpenDoorClosedLinkWithAnyWordOpensAndSaysOk()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link? link) = RoomWithDoor(string.Empty);

        string output = Capture(() => room.OpenDoor("puerta azul", string.Empty));

        Assert.True(link.IsOpen);
        Assert.Contains(Localizer.Msg("Ok"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door already open says not necessary.
    /// </summary>
    [Fact]
    public void OpenDoorAlreadyOpenSaysNotNecessary()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link _) = RoomWithDoor(Link.OpenDoor);

        string output = Capture(() => room.OpenDoor("puerta azul", string.Empty));

        Assert.Contains(Localizer.Msg("AlreadyOpen"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door wrong key reports failure and quoted key.
    /// </summary>
    [Fact]
    public void OpenDoorWrongKeyReportsFailureAndQuotedKey()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link? link) = RoomWithDoor("secreto");

        string output = Capture(() => room.OpenDoor("puerta azul", "mal"));

        Assert.False(link.IsOpen);
        Assert.Contains(Localizer.Msg("TryOpenFail"), output, StringComparison.Ordinal);
        Assert.Contains("\"mal\"" + Localizer.Msg("WrongPassword"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door no key given for locked door asks for password.
    /// </summary>
    [Fact]
    public void OpenDoorNoKeyGivenForLockedDoorAsksForPassword()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link _) = RoomWithDoor("secreto");

        string output = Capture(() => room.OpenDoor("puerta azul", string.Empty));

        Assert.Contains(Localizer.Msg("TryOpenFail"), output, StringComparison.Ordinal);
        Assert.Contains(Localizer.Msg("NeedPassword"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door twin door in destination is opened too.
    /// </summary>
    [Fact]
    public void OpenDoorTwinDoorInDestinationIsOpenedToo()
    {
        Room destination = MakeRoom();
        var twin = new Link(new("Puerta azul", "Blue door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), string.Empty, MakeRoom());
        destination.Children.Add(twin);
        (Room? room, Link _) = RoomWithDoor(string.Empty, destination);

        room.OpenDoor("puerta azul", string.Empty);

        Assert.True(twin.IsOpen);
    }

    /// <summary>
    /// Open door no twin door warns it is one way.
    /// </summary>
    [Fact]
    public void OpenDoorNoTwinDoorWarnsItIsOneWay()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link _) = RoomWithDoor(string.Empty);

        string output = Capture(() => room.OpenDoor("puerta azul", string.Empty));

        Assert.Contains(Localizer.Msg("DoorWarning"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door wrong riddle answer does not add generic failure text.
    /// </summary>
    [Fact]
    public void OpenDoorWrongRiddleAnswerDoesNotAddGenericFailureText()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        room.Children.Add(new RiddleLink(
            new("Escalera", "Staircase"),
            new(string.Empty, string.Empty),
            new("¿Cuántos?", "How many?"),
            "treinta",
            new(string.Empty, string.Empty),
            MakeRoom()));

        string output = Capture(() => room.OpenDoor("escalera", "cuarenta"));

        Assert.Contains(Localizer.Msg("WrongAnswer"), output, StringComparison.Ordinal);
        Assert.DoesNotContain(Localizer.Msg("TryOpenFail"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door thing is not a link says cannot do that.
    /// </summary>
    [Fact]
    public void OpenDoorThingIsNotALinkSaysCannotDoThat()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 1));

        string output = Capture(() => room.OpenDoor("maza", string.Empty));

        Assert.Contains(Localizer.Msg("CantDo"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Open door nothing by that name says cannot do that and no such thing.
    /// </summary>
    [Fact]
    public void OpenDoorNothingByThatNameSaysCannotDoThatAndNoSuchThing()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();

        string output = Capture(() => room.OpenDoor("puerta", string.Empty));

        Assert.Contains(
            Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + "puerta" + Localizer.Msg("InTheRoom"),
            output,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// Player open door with key invokes magic words first.
    /// </summary>
    [Fact]
    public void PlayerOpenDoorWithKeyInvokesMagicWordsFirst()
    {
        Localizer.Current = Language.Spanish;
        (Room? room, Link _) = RoomWithDoor("secreto");
        var player = new Player(room);

        string output = Capture(() => player.OpenDoor("puerta azul", "SECRETO"));

        Assert.Contains(Localizer.Msg("MagicWords"), output, StringComparison.Ordinal);
        Assert.Contains("SECRETO!!!", output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Break thing success shows crash and surprise.
    /// </summary>
    [Fact]
    public void BreakThingSuccessShowsCrashAndSurprise()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        var inner = new Item(new("Llave", "Key"), new(string.Empty, string.Empty), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new(string.Empty, string.Empty), "Maza", inner));

        string output = Capture(() => room.BreakThing("jarron", "maza"));

        Assert.Contains(Localizer.Msg("Crash"), output, StringComparison.Ordinal);
        Assert.Contains(Localizer.Msg("Surprise"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Break thing wrong weapon says cannot break.
    /// </summary>
    [Fact]
    public void BreakThingWrongWeaponSaysCannotBreak()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();
        var inner = new Item(new("Llave", "Key"), new(string.Empty, string.Empty), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new(string.Empty, string.Empty), "Maza", inner));

        string output = Capture(() => room.BreakThing("jarron", "soga"));

        Assert.Contains(Localizer.Msg("CantBreak"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Break thing nothing by that name says cannot do that and no such thing.
    /// </summary>
    [Fact]
    public void BreakThingNothingByThatNameSaysCannotDoThatAndNoSuchThing()
    {
        Localizer.Current = Language.Spanish;
        Room room = MakeRoom();

        string output = Capture(() => room.BreakThing("jarron", "maza"));

        Assert.Contains(
            Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + "jarron" + Localizer.Msg("InTheRoom"),
            output,
            StringComparison.Ordinal);
    }

    /// <summary>
    /// Hidden try break accepts partial weapon name.
    /// </summary>
    [Fact]
    public void HiddenTryBreakAcceptsPartialWeaponName()
    {
        var hidden = new Hidden(
            new("Espejo", "Mirror"),
            new(string.Empty, string.Empty),
            "Hueso de gato",
            new Item(new("a", "a"), new(string.Empty, string.Empty), 1));

        Assert.True(hidden.TryBreak("hueso"));
        Assert.False(hidden.TryBreak(string.Empty));
    }

    /// <summary>
    /// Hidden empty breaker breaks with anything.
    /// </summary>
    [Fact]
    public void HiddenEmptyBreakerBreaksWithAnything()
    {
        var hidden = new Hidden(
            new("Pared", "Wall"),
            new(string.Empty, string.Empty),
            string.Empty,
            new Item(new("a", "a"), new(string.Empty, string.Empty), 1));

        Assert.True(hidden.TryBreak(string.Empty));
        Assert.True(hidden.TryBreak("cualquier cosa"));
    }

    /// <summary>
    /// Troll die screams and dies.
    /// </summary>
    [Fact]
    public void TrollDieScreamsAndDies()
    {
        Localizer.Current = Language.Spanish;
        var troll = new Troll(new("Bruja", "Witch"), new(string.Empty, string.Empty), "Escoba", new(string.Empty, string.Empty), new(string.Empty, string.Empty));

        string output = Capture(() => troll.Die(string.Empty));

        Assert.True(troll.Dead);
        Assert.Contains(Localizer.Msg("Ahhhh"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Troll accept wrong gift after right one makes troll unhappy again.
    /// </summary>
    [Fact]
    public void TrollAcceptWrongGiftAfterRightOneMakesTrollUnhappyAgain()
    {
        var troll = new Troll(new("Bruja", "Witch"), new(string.Empty, string.Empty), "Escoba", new(string.Empty, string.Empty), new(string.Empty, string.Empty));

        troll.Accept(new Item(new("Escoba", "Broom"), new(string.Empty, string.Empty), 1));
        troll.Accept(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 1));

        Assert.False(troll.Happy);
    }

    /// <summary>
    /// Guard die wrong weapon shows useless and survives.
    /// </summary>
    [Fact]
    public void GuardDieWrongWeaponShowsUselessAndSurvives()
    {
        Localizer.Current = Language.Spanish;
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new("grr", "grr"), "Látigo");
        bool died = true;

        string output = Capture(() => died = guard.Die("maza"));

        Assert.False(died);
        Assert.False(guard.Dead);
        Assert.Contains(Localizer.Msg("Useless"), output, StringComparison.Ordinal);
    }

    /// <summary>
    /// Daughter die right weapon confesses and dies.
    /// </summary>
    [Fact]
    public void DaughterDieRightWeaponConfessesAndDies()
    {
        bool killed = false;
        var daughter = new Daughter(
            new("Hija", "Daughter"),
            new(string.Empty, string.Empty),
            new("confieso", "I confess"),
            "Daga",
            () => killed = true);

        bool died = daughter.Die("daga");

        Assert.True(died);
        Assert.True(daughter.Dead);
        Assert.False(killed);
    }

    /// <summary>
    /// Daughter die wrong weapon kills the player instead.
    /// </summary>
    [Fact]
    public void DaughterDieWrongWeaponKillsThePlayerInstead()
    {
        bool killed = false;
        var daughter = new Daughter(
            new("Hija", "Daughter"),
            new(string.Empty, string.Empty),
            new("confieso", "I confess"),
            "Daga",
            () => killed = true);

        bool died = daughter.Die("maza");

        Assert.False(died);
        Assert.False(daughter.Dead);
        Assert.True(killed);
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

    private static (Room Room, Link Link) RoomWithDoor(string key, Room? destination = null)
    {
        Room room = MakeRoom();
        var link = new Link(new("Puerta azul", "Blue door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), key, destination ?? MakeRoom());
        room.Children.Add(link);
        return (room, link);
    }
}
