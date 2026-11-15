using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

internal sealed class OpenLink(
    LocalizedText name,
    LocalizedText description,
    LocalizedText passComment,
    Room destination)
    : Link(name, description, passComment, OpenDoor, destination)
{
    public override bool TryOpen(string key)
    {
        return true;
    }

    public override Room? GetDest()
    {
        return Destination;
    }
}
