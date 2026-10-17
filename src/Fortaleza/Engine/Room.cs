using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Room(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight)
{
    public bool Visited { get; set; }

    public Thing? Get(string name)
    {
        var normalized = StringHelper.Normalize(name);
        return Children.FirstOrDefault(
            t => StringHelper.Normalize(t.Name.Es) == normalized
              || StringHelper.Normalize(t.Name.En) == normalized);
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

    public virtual void OpenDoor(string name, string key) { }

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
