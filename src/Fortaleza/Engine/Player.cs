using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Player(Room start) : LivingThing(new("jugador", "player"), new("", ""), 0)
{
    public Room CurrentRoom { get; private set; } = start;
    public Inventory Bag { get; } = new();
    public int VisitCount { get; private set; }

    public override bool Die(string weapon)
    {
        if (Dead) return false;
        base.Die(weapon);
        Output(Localizer.Msg("Dying"));
        return true;
    }

    public void Go(Room next)
    {
        if (Dead) return;
        CurrentRoom = next;
        if (next.Visited) return;
        next.Visited = true;
        VisitCount++;
    }

    public void Look()
    {
        if (Dead || !Visible) return;
        Output(Localizer.Msg("YouAreIn") + CurrentRoom.Name.Get() + ".");
        Output(CurrentRoom.Description.Get());
        Output(Localizer.Msg("RoomContains"));
        CurrentRoom.Describe();
    }

    public void See(string what)
    {
        if (Dead) return;
        if (what == "")
        {
            Output(Localizer.Msg("WhatSee"));
            return;
        }
        var thing = CurrentRoom.Get(what) ?? Bag.Get(what);
        Output(thing is not null
            ? thing.Image()
            : Localizer.Msg("ThereIsNo") + what + Localizer.Msg("InTheRoom") + Localizer.Msg("AlsoNotInBag"));
    }

    public void Take(string what)
    {
        if (Dead) return;
        var thing = CurrentRoom.Get(what);
        if (thing is null)
        {
            Output(what != ""
                ? Localizer.Msg("ThereIsNo") + what + Localizer.Msg("InTheRoom")
                : Localizer.Msg("Complete"));
            return;
        }
        if (!Bag.CanAdd(thing.Weight))
        {
            Output(Localizer.Msg(thing.Weight > Engine.Inventory.LWeight ? "CantCarry" : "TooHeavy"));
            return;
        }
        Bag.Children.Add(CurrentRoom.Take(what)!);
        Output(Localizer.Msg("Ok"));
    }

    public void LeaveItem(string what)
    {
        if (Dead) return;
        var thing = Bag.Take(what);
        if (thing is null)
        {
            Output(what != ""
                ? Quoted(what) + Localizer.Msg("NotInBag")
                : Localizer.Msg("Complete"));
            return;
        }
        CurrentRoom.Children.Add(thing);
        Output(Localizer.Msg("Ok"));
    }

    public void LeaveAll()
    {
        if (Dead) return;
        CurrentRoom.Children.AddRange(Bag.Children);
        Bag.Children.Clear();
        Output(Localizer.Msg("DropAll"));
    }

    public void Walk(string door)
    {
        if (Dead) return;
        var thing = CurrentRoom.Get(door);
        if (thing is null)
        {
            Output(door != ""
                ? Localizer.Msg("ThereIsNo") + door + Localizer.Msg("InTheRoom")
                : Localizer.Msg("Complete"));
            return;
        }
        if (thing is not Link link)
        {
            Output(Localizer.Msg("CantOpenThis"));
            return;
        }
        var next = link.GetDest();
        if (next is null)
        {
            Output(Localizer.Msg("Ouch"));
            Output(Localizer.Msg("TryOpenFirst"));
            return;
        }
        Go(next);
        link.Pass(Bag);
    }

    public void OpenDoor(string door, string key)
    {
        if (Dead) return;
        CurrentRoom.OpenDoor(door, key);
    }

    public void Kill(string who, string weapon)
    {
        if (Dead) return;
        var target = CurrentRoom.Get(who);
        if (target is null)
        {
            Output(who != ""
                ? Quoted(who) + Localizer.Msg("NotHere")
                : Localizer.Msg("Complete"));
            return;
        }
        if (target is not (Troll or Guard))
        {
            Output(Localizer.Msg("CantKillInanimate"));
            return;
        }
        if (weapon != "" && Bag.Get(weapon) is null)
        {
            Output(Quoted(weapon) + Localizer.Msg("NotInBag"));
            return;
        }
        if (((LivingThing)target).Die(weapon))
            CurrentRoom.Children.Remove(target);
    }

    public void Give(string what, string who)
    {
        if (Bag.IsEmpty)
        {
            Output(Localizer.Msg("BagEmpty"));
            return;
        }
        var target = CurrentRoom.Get(who);
        if (target is null)
        {
            Output(who != ""
                ? Quoted(who) + Localizer.Msg("NotHere")
                : Localizer.Msg("WhosGift"));
            return;
        }
        if (target is not Troll troll)
        {
            Output(Localizer.Msg("GiftRejected"));
            return;
        }
        var gift = Bag.Take(what);
        if (gift is null)
        {
            Output(Quoted(what) + Localizer.Msg("NotInBag"));
            return;
        }
        troll.Accept((Item)gift);
    }

    public void Ask(string who)
    {
        if (Dead) return;
        var target = CurrentRoom.Get(who);
        if (target is null)
        {
            Output(who != ""
                ? Quoted(who) + Localizer.Msg("NotHere")
                : Localizer.Msg("Complete"));
            return;
        }
        if (target is Troll or Guard)
            ((LivingThing)target).Speak();
        else
            Output(Localizer.Msg("TalkingAlone"));
    }

    public void Break(string what, string with)
    {
        if (with == "" || Bag.Get(with) is not null)
            CurrentRoom.BreakThing(what, with);
        else
            Output(Quoted(with) + Localizer.Msg("NotInBag"));
    }

    public void Weigh(string what)
    {
        if (Bag.Get("Balanza") is null)
        {
            Output(Localizer.Msg("CantDo"));
            return;
        }
        var thing = Bag.Get(what) ?? CurrentRoom.Get(what);
        if (thing is not Item item || item.Weight > Engine.Inventory.LWeight
            || StringHelper.Normalize(item.Name.Es) == "balanza")
        {
            Output(Localizer.Msg("CantDo"));
            return;
        }
        Output(Localizer.Msg("YouWeigh") + item.Weight
            + (item.Weight == 1 ? Localizer.Msg("BagUnit") : Localizer.Msg("BagUnits")));
    }

    public void Inventory()
    {
        if (Dead) return;
        if (Bag.IsEmpty)
        {
            Output(Localizer.Msg("BagEmpty"));
            return;
        }
        Output(Localizer.Msg("BagContains"));
        Bag.Describe();
    }

    private static string Quoted(string s) => $"\"{s}\"";
}
