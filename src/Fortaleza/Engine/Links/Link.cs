using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class Link(
    LocalizedText name, LocalizedText description,
    LocalizedText passComment, string key, Room destination)
    : Thing(name, description, 0)
{
    public Room Destination { get; } = destination;
    public string Key { get; } = key;
    public LocalizedText PassComment { get; } = passComment;
    public bool IsOpen { get; protected set; } = string.IsNullOrEmpty(key);

    public virtual bool TryOpen(string key)
    {
        IsOpen = string.IsNullOrEmpty(Key)
            || StringHelper.Normalize(key) == StringHelper.Normalize(Key);
        return IsOpen;
    }

    public virtual Room? GetDest() => IsOpen ? Destination : null;

    public virtual void Pass(Inventory bag) => Output(PassComment.Get());

    public void Close() => IsOpen = false;
}
