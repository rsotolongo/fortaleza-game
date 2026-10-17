using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class OpenLink(
    LocalizedText name, LocalizedText description,
    LocalizedText passComment, Room destination)
    : Link(name, description, passComment, OpenDoor, destination)
{
    public override bool TryOpen(string key) => true;
    public override Room? GetDest() => Destination;
}
