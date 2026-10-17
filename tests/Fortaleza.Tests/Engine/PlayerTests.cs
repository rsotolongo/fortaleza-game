using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Xunit;

namespace Fortaleza.Tests.Engine;

public class PlayerTests
{
    private static Room MakeRoom() =>
        new(new("sala", "room"), new("", ""), Inventory.LWeight + 1);

    private static (Player Player, Room Room) SetupPlayerInRoom()
    {
        var room = MakeRoom();
        return (new Player(room), room);
    }

    [Fact]
    public void Take_ItemInRoom_AddsItemToBag()
    {
        var (player, room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));

        player.Take("Maza");

        Assert.Single(player.Bag.Children);
        Assert.Empty(room.Children);
    }

    [Fact]
    public void Take_ItemTooHeavy_LeavesItemInRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Cedro", "Cedar"), new("", ""), Inventory.LWeight + 1));

        player.Take("Cedro");

        Assert.Empty(player.Bag.Children);
        Assert.Single(room.Children);
    }

    [Fact]
    public void Take_BagWouldOverflow_LeavesItemInRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Yunque", "Anvil"), new("", ""), Inventory.LWeight - 1));
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));

        player.Take("Maza");

        Assert.Single(player.Bag.Children);
        Assert.Single(room.Children);
    }

    [Fact]
    public void LeaveItem_ItemInBag_MovesItemToRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));

        player.LeaveItem("Maza");

        Assert.Single(room.Children);
        Assert.Empty(player.Bag.Children);
    }

    [Fact]
    public void LeaveAll_BagWithItems_MovesEverythingToRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));
        player.Bag.Children.Add(new Item(new("Soga", "Rope"), new("", ""), 1));

        player.LeaveAll();

        Assert.Equal(2, room.Children.Count);
        Assert.Empty(player.Bag.Children);
    }

    [Fact]
    public void Kill_GuardWithCorrectWeapon_RemovesGuardFromRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);
        player.Bag.Children.Add(new Item(new("Látigo", "Whip"), new("", ""), 2));

        player.Kill("Lobo", "Látigo");

        Assert.DoesNotContain(guard, room.Children);
    }

    [Fact]
    public void Kill_GuardWithWrongWeapon_KeepsGuardInRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 2));

        player.Kill("Lobo", "Maza");

        Assert.Contains(guard, room.Children);
    }

    [Fact]
    public void Kill_WeaponNotInBag_KeepsGuardInRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        var guard = new Guard(new("Lobo", "Wolf"), new("", ""), new("I die", "I die"), "Látigo");
        room.Children.Add(guard);

        player.Kill("Lobo", "Látigo");

        Assert.Contains(guard, room.Children);
        Assert.False(guard.Dead);
    }

    [Fact]
    public void Give_ToTroll_RemovesItemFromBagAndMakesTrollHappy()
    {
        var (player, room) = SetupPlayerInRoom();
        var troll = new Troll(new("Bruja", "Witch"), new("", ""), "Escoba",
            new("Gracias", "Thanks"), new("Dame la escoba", "Give broom"));
        room.Children.Add(troll);
        player.Bag.Children.Add(new Item(new("Escoba", "Broom"), new("", ""), 1));

        player.Give("Escoba", "Bruja");

        Assert.Empty(player.Bag.Children);
        Assert.True(troll.Happy);
    }

    [Fact]
    public void Walk_OpenLink_MovesPlayerToDestination()
    {
        var (player, room) = SetupPlayerInRoom();
        var next = MakeRoom();
        room.Children.Add(new OpenLink(new("Puerta", "Door"), new("", ""), new("pasas", "you pass"), next));

        player.Walk("Puerta");

        Assert.Same(next, player.CurrentRoom);
    }

    [Fact]
    public void Walk_ClosedLink_KeepsPlayerInRoom()
    {
        var (player, room) = SetupPlayerInRoom();
        room.Children.Add(new Link(new("Puerta", "Door"), new("", ""), new("", ""), "clave", MakeRoom()));

        player.Walk("Puerta");

        Assert.Same(room, player.CurrentRoom);
    }

    [Fact]
    public void Walk_DangerLinkWithoutTalisman_KillsPlayer()
    {
        var (player, room) = SetupPlayerInRoom();
        room.Children.Add(new DangerLink(new("Traquea", "Trachea"), new("", ""), new("", ""), "",
            "Talismán de aire", MakeRoom(), () => player.Die("")));

        player.Walk("Traquea");

        Assert.True(player.Dead);
    }

    [Fact]
    public void OpenDoor_CorrectKey_OpensLinkSoPlayerCanWalk()
    {
        var (player, room) = SetupPlayerInRoom();
        var next = MakeRoom();
        room.Children.Add(new Link(new("Puerta", "Door"), new("", ""), new("", ""), "clave", next));

        player.OpenDoor("Puerta", "clave");
        player.Walk("Puerta");

        Assert.Same(next, player.CurrentRoom);
    }

    [Fact]
    public void Go_FirstVisitOnly_IncrementsVisitCount()
    {
        var (player, _) = SetupPlayerInRoom();
        var room = MakeRoom();

        player.Go(room);
        player.Go(room);

        Assert.Equal(1, player.VisitCount);
    }

    [Fact]
    public void Die_Player_SetsDeadAndBlocksFurtherActions()
    {
        var (player, room) = SetupPlayerInRoom();
        room.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 5));

        player.Die("");
        player.Take("Maza");

        Assert.True(player.Dead);
        Assert.Empty(player.Bag.Children);
    }

    [Fact]
    public void Break_WithoutWeaponInBag_DoesNotBreakHidden()
    {
        var (player, room) = SetupPlayerInRoom();
        var inner = new Item(new("Llave", "Key"), new("", ""), 1);
        var hidden = new Hidden(new("Jarrón", "Vase"), new("", ""), "Maza", inner);
        room.Children.Add(hidden);

        player.Break("Jarrón", "Maza");

        Assert.Contains(hidden, room.Children);
    }

    [Fact]
    public void Break_WithWeaponInBag_ReplacesHiddenWithChild()
    {
        var (player, room) = SetupPlayerInRoom();
        var inner = new Item(new("Llave", "Key"), new("", ""), 1);
        room.Children.Add(new Hidden(new("Jarrón", "Vase"), new("", ""), "Maza", inner));
        player.Bag.Children.Add(new Item(new("Maza", "Mace"), new("", ""), 2));

        player.Break("Jarrón", "Maza");

        Assert.Contains(inner, room.Children);
    }
}
