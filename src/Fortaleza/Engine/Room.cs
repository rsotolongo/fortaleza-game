using Fortaleza.Engine.Links;
using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal class Room(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight)
{
    public bool Visited { get; set; }

    public Thing? Get(string name)
    {
        string query = StringHelper.Normalize(name);
        return Children.Find(
                t => StringHelper.Normalize(t.Name.Es) == query
                  || StringHelper.Normalize(t.Name.En) == query)
            ?? Children.Find(
                t => StringHelper.ContainsAllWords(StringHelper.Normalize(t.Name.Es), query)
                  || StringHelper.ContainsAllWords(StringHelper.Normalize(t.Name.En), query));
    }

    public Thing? Take(string name)
    {
        Thing? thing = Get(name);
        if (thing is null)
        {
            return null;
        }

        Children.Remove(thing);
        return thing;
    }

    public virtual void Describe()
    {
        foreach (Thing child in Children)
        {
            Output(child.Name.Get());
        }
    }

    public virtual void OpenDoor(string name, string key)
    {
        Thing? thing = Get(name);
        if (thing is null)
        {
            Output(!string.IsNullOrEmpty(name)
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
        if (!link.TryOpen(key))
        {
            ReportFailedOpen(link, key);
            return;
        }

        if (link.Destination.GetType() == typeof(Room))
        {
            Thing? twin = link.Destination.Get(name);
            if (twin is null)
            {
                Output(Localizer.Msg("DoorWarning"));
            }
            else
            {
                (twin as Link)?.TryOpen(key);
            }
        }

        Output(Localizer.Msg("Ok"));
    }

    public virtual void BreakThing(string name, string weapon)
    {
        Thing? thing = Get(name);
        if (thing is null)
        {
            Output(!string.IsNullOrEmpty(name)
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
        int idx = Children.IndexOf(hidden);
        Children.RemoveAt(idx);
        if (hidden.HiddenChild is not null)
        {
            Children.Insert(idx, hidden.HiddenChild);
        }

        Output(Localizer.Msg("Crash"));
        Output(Localizer.Msg("Surprise"));
    }

    private static void ReportFailedOpen(Link link, string key)
    {
        if (link is RiddleLink)
        {
            return;
        }

        Output(Localizer.Msg("TryOpenFail"));
        Output(string.IsNullOrEmpty(key)
            ? Localizer.Msg("NeedPassword")
            : "\"" + key + "\"" + Localizer.Msg("WrongPassword"));
    }
}
