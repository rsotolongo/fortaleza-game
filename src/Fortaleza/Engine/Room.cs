using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Room(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight)
{
    public bool Visited { get; set; }

    public Thing? Get(string name)
    {
        var query = StringHelper.Normalize(name);
        return Children.FirstOrDefault(
                t => StringHelper.Normalize(t.Name.Es) == query
                  || StringHelper.Normalize(t.Name.En) == query)
            ?? Children.FirstOrDefault(
                t => StringHelper.ContainsAllWords(StringHelper.Normalize(t.Name.Es), query)
                  || StringHelper.ContainsAllWords(StringHelper.Normalize(t.Name.En), query));
    }

    public Thing? Take(string name)
    {
        var thing = Get(name);
        if (thing is null) return null;
        Children.Remove(thing);
        return thing;
    }

    public virtual void Describe()
    {
        foreach (var child in Children)
            Output(child.Name.Get());
    }

    public virtual void OpenDoor(string name, string key)
    {
        var thing = Get(name);
        if (thing is null)
        {
            Output(name != ""
                ? Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + name + Localizer.Msg("InTheRoom")
                : Localizer.Msg("Complete"));
            return;
        }
        if (thing is not Link link)
        {
            Output(Localizer.Msg("CantDo"));
            return;
        }
        if (link.IsOpen)
        {
            Output(Localizer.Msg("AlreadyOpen"));
            return;
        }
        if (link.TryOpen(key))
        {
            if (link.Destination.GetType() == typeof(Room))
            {
                var twin = link.Destination.Get(name);
                if (twin is null)
                    Output(Localizer.Msg("DoorWarning"));
                else if (twin is Link twinLink)
                    twinLink.TryOpen(key);
            }
            Output(Localizer.Msg("Ok"));
        }
        else if (link is not RiddleLink)
        {
            Output(Localizer.Msg("TryOpenFail"));
            Output(key == ""
                ? Localizer.Msg("NeedPassword")
                : "\"" + key + "\"" + Localizer.Msg("WrongPassword"));
        }
    }

    public virtual void BreakThing(string name, string weapon)
    {
        var thing = Get(name);
        if (thing is null)
        {
            Output(name != ""
                ? Localizer.Msg("CantDo") + " " + Localizer.Msg("ThereIsNo") + name + Localizer.Msg("InTheRoom")
                : Localizer.Msg("Complete"));
            return;
        }
        if (thing is not Hidden hidden)
        {
            Output(Localizer.Msg("NotBreakable"));
            return;
        }
        if (!hidden.TryBreak(weapon))
        {
            Output(Localizer.Msg("CantBreak"));
            return;
        }
        var idx = Children.IndexOf(hidden);
        Children.RemoveAt(idx);
        Children.Insert(idx, hidden.HiddenChild);
        Output(Localizer.Msg("Crash"));
        Output(Localizer.Msg("Surprise"));
    }
}
