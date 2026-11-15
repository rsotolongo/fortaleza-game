using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

internal class Link(
    LocalizedText name,
    LocalizedText description,
    LocalizedText passComment,
    string key,
    Room destination)
    : Thing(name, description, Inventory.LWeight + 1)
{
    public const string OpenDoor = "\0\0\0";

    public Room Destination { get; } = destination;
    public string Key { get; } = key == OpenDoor ? string.Empty : key;
    public LocalizedText PassComment { get; } = passComment;
    public bool IsOpen { get; protected set; } = key == OpenDoor;
    public string AltKey { get; init; } = string.Empty;

    public virtual bool TryOpen(string key)
    {
        string typed = StringHelper.Normalize(key);
        IsOpen = string.IsNullOrEmpty(Key)
            || typed == StringHelper.Normalize(Key)
            || (!string.IsNullOrEmpty(AltKey) && typed == StringHelper.Normalize(AltKey));
        return IsOpen;
    }

    public virtual Room? GetDest()
    {
        return IsOpen ? Destination : null;
    }

    public virtual void Pass(Inventory bag)
    {
        Output(PassComment.Get());
    }

    public void Close()
    {
        IsOpen = false;
    }
}
