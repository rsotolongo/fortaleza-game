using Fortaleza.Engine;
using Fortaleza.Engine.Links;

namespace Fortaleza.Tests.Engine;

/// <summary>
/// Tests for player.
/// </summary>
public sealed class PlayerTests
{
    /// <summary>
    /// Take item in room adds item to bag.
    /// </summary>
    [Fact]
    public void TakeItemInRoomAddsItemToBag()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));

        player.Take("Maza");

        Assert.Single(player.Bag.Children);
        Assert.Empty(room.Children);
    }

    /// <summary>
    /// Take item too heavy leaves item in room.
    /// </summary>
    [Fact]
    public void TakeItemTooHeavyLeavesItemInRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Cedro", "Cedar"), new(string.Empty, string.Empty), Inventory.LWeight + 1));

        player.Take("Cedro");

        Assert.Empty(player.Bag.Children);
        Assert.Single(room.Children);
    }

    /// <summary>
    /// Take bag would overflow leaves item in room.
    /// </summary>
    [Fact]
    public void TakeBagWouldOverflowLeavesItemInRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Yunque", "Anvil"), new(string.Empty, string.Empty), Inventory.LWeight - 1));
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));

        player.Take("Maza");

        Assert.Single(player.Bag.Children);
        Assert.Single(room.Children);
    }

    /// <summary>
    /// Leave item in bag moves item to room.
    /// </summary>
    [Fact]
    public void LeaveItemItemInBagMovesItemToRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));

        player.LeaveItem("Maza");

        Assert.Single(room.Children);
        Assert.Empty(player.Bag.Children);
    }

    /// <summary>
    /// Leave all bag with items moves everything to room.
    /// </summary>
    [Fact]
    public void LeaveAllBagWithItemsMovesEverythingToRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));
        player.Bag.Children.Add(new Item(new("Soga", "Rope"), new(string.Empty, string.Empty), 1));

        player.LeaveAll();

        Assert.Equal(2, room.Children.Count);
        Assert.Empty(player.Bag.Children);
    }

    /// <summary>
    /// Kill guard with correct weapon removes guard from room.
    /// </summary>
    [Fact]
    public void KillGuardWithCorrectWeaponRemovesGuardFromRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);
        player.Bag.Children.Add(new Item(new("Látigo", "Whip"), new(string.Empty, string.Empty), 2));

        player.Kill("Lobo", "Látigo");

        Assert.DoesNotContain(guard, room.Children);
    }

    /// <summary>
    /// Kill guard with wrong weapon keeps guard in room.
    /// </summary>
    [Fact]
    public void KillGuardWithWrongWeaponKeepsGuardInRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 2));

        player.Kill("Lobo", "Maza");

        Assert.Contains(guard, room.Children);
    }

    /// <summary>
    /// Kill weapon not in bag keeps guard in room.
    /// </summary>
    [Fact]
    public void KillWeaponNotInBagKeepsGuardInRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new(string.Empty, string.Empty), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);

        player.Kill("Lobo", "Látigo");

        Assert.Contains(guard, room.Children);
        Assert.False(guard.Dead);
    }

    /// <summary>
    /// Give to troll removes item from bag and makes troll happy.
    /// </summary>
    [Fact]
    public void GiveToTrollRemovesItemFromBagAndMakesTrollHappy()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var troll = new Troll(
            new("Bruja", "Witch"),
            new(string.Empty, string.Empty),
            "Escoba",
            new("Gracias", "Thanks"),
            new("Dame la escoba", "Give broom"));
        room.Children.Add(troll);
        player.Bag.Children.Add(new Item(new("Escoba", "Broom"), new(string.Empty, string.Empty), 1));

        player.Give("Escoba", "Bruja");

        Assert.Empty(player.Bag.Children);
        Assert.True(troll.Happy);
    }

    /// <summary>
    /// Walk open link moves player to destination.
    /// </summary>
    [Fact]
    public void WalkOpenLinkMovesPlayerToDestination()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        Room next = MakeRoom();
        room.Children.Add(new OpenLink(new("Puerta", "Door"), new(string.Empty, string.Empty), new("pasas", "you pass"), next));

        player.Walk("Puerta");

        Assert.Same(next, player.CurrentRoom);
    }

    /// <summary>
    /// Walk closed link keeps player in room.
    /// </summary>
    [Fact]
    public void WalkClosedLinkKeepsPlayerInRoom()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        room.Children.Add(new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "clave", MakeRoom()));

        player.Walk("Puerta");

        Assert.Same(room, player.CurrentRoom);
    }

    /// <summary>
    /// Walk danger link without talisman kills player.
    /// </summary>
    [Fact]
    public void WalkDangerLinkWithoutTalismanKillsPlayer()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        room.Children.Add(new DangerLink(
            new("Traquea", "Trachea"),
            new(string.Empty, string.Empty),
            new(string.Empty, string.Empty),
            Link.OpenDoor,
            "Talismán de aire",
            MakeRoom(),
            () => player.Die(string.Empty)));

        player.Walk("Traquea");

        Assert.True(player.Dead);
    }

    /// <summary>
    /// Open door correct key opens link so player can walk.
    /// </summary>
    [Fact]
    public void OpenDoorCorrectKeyOpensLinkSoPlayerCanWalk()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        Room next = MakeRoom();
        room.Children.Add(new Link(new("Puerta", "Door"), new(string.Empty, string.Empty), new(string.Empty, string.Empty), "clave", next));

        player.OpenDoor("Puerta", "clave");
        player.Walk("Puerta");

        Assert.Same(next, player.CurrentRoom);
    }

    /// <summary>
    /// Go first visit only increments visit count.
    /// </summary>
    [Fact]
    public void GoFirstVisitOnlyIncrementsVisitCount()
    {
        (Player? player, Room _) = SetupPlayerInRoom();
        Room room = MakeRoom();

        player.Go(room);
        player.Go(room);

        Assert.Equal(1, player.VisitCount);
    }

    /// <summary>
    /// Die player sets dead and blocks further actions.
    /// </summary>
    [Fact]
    public void DiePlayerSetsDeadAndBlocksFurtherActions()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 5));

        player.Die(string.Empty);
        player.Take("Maza");

        Assert.True(player.Dead);
        Assert.Empty(player.Bag.Children);
    }

    /// <summary>
    /// Break without weapon in bag does not break hidden.
    /// </summary>
    [Fact]
    public void BreakWithoutWeaponInBagDoesNotBreakHidden()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var inner = new Item(new("Llave", "Key"), new(string.Empty, string.Empty), 1);
        var hidden = new Hidden(new("Jarrón", "Vase"), new(string.Empty, string.Empty), "Maza", inner);
        room.Children.Add(hidden);

        player.Break("Jarrón", "Maza");

        Assert.Contains(hidden, room.Children);
    }

    /// <summary>
    /// Break with weapon in bag replaces hidden with child.
    /// </summary>
    [Fact]
    public void BreakWithWeaponInBagReplacesHiddenWithChild()
    {
        (Player? player, Room? room) = SetupPlayerInRoom();
        var inner = new Item(new("Llave", "Key"), new(string.Empty, string.Empty), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new(string.Empty, string.Empty), "Maza", inner));
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new(string.Empty, string.Empty), 2));

        player.Break("Jarrón", "Maza");

        Assert.Contains(inner, room.Children);
    }

    private static Room MakeRoom()
    {
        return new(new("sala", "room"), new(string.Empty, string.Empty), Inventory.LWeight + 1);
    }

    private static (Player Player, Room Room) SetupPlayerInRoom()
    {
        Room room = MakeRoom();
        return (new Player(room), room);
    }
}
