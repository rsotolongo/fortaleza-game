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
        if (Get(name) is not Link link)
        {
            Output(Localizer.Msg("CantDo"));
            return;
        }
        if (!string.IsNullOrEmpty(key))
            Output(Localizer.Msg("MagicWords") + " " + key);
        if (link.TryOpen(key))
            Output(Localizer.Msg("Ok"));
        else if (string.IsNullOrEmpty(key))
            Output(Localizer.Msg("NeedPassword"));
        else
            Output(key + Localizer.Msg("WrongPassword"));
    }

    public virtual void BreakThing(string name, string weapon)
    {
        if (Get(name) is not Hidden hidden)
        {
            Output(Localizer.Msg("NotBreakable"));
            return;
        }
        if (!hidden.TryBreak(weapon))
        {
            Output(Localizer.Msg("Useless"));
            return;
        }
        Output(Localizer.Msg("Crash"));
        var idx = Children.IndexOf(hidden);
        Children.RemoveAt(idx);
        Children.Insert(idx, hidden.HiddenChild);
    }
}
