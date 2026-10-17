using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class Link(
    LocalizedText name, LocalizedText description,
    LocalizedText passComment, string key, Room destination)
    : Thing(name, description, Inventory.LWeight + 1)
{
    public const string OpenDoor = "\0\0\0";

    public Room Destination { get; } = destination;
    public string Key { get; } = key == OpenDoor ? "" : key;
    public LocalizedText PassComment { get; } = passComment;
    public bool IsOpen { get; protected set; } = key == OpenDoor;
    public string AltKey { get; init; } = "";

    public virtual bool TryOpen(string key)
    {
        var typed = StringHelper.Normalize(key);
        IsOpen = string.IsNullOrEmpty(Key)
            || typed == StringHelper.Normalize(Key)
            || (AltKey != "" && typed == StringHelper.Normalize(AltKey));
        return IsOpen;
    }

    public virtual Room? GetDest() => IsOpen ? Destination : null;

    public virtual void Pass(Inventory bag) => Output(PassComment.Get());

    public void Close() => IsOpen = false;
}
