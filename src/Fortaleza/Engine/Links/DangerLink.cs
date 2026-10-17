using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class DangerLink(
    LocalizedText name, LocalizedText description,
    LocalizedText passComment, string key,
    string talismanName, Room destination, Action killPlayer)
    : Link(name, description, passComment, key, destination)
{
    public string TalismanName { get; } = talismanName;

    public override void Pass(Inventory bag)
    {
        var hasTalisman = bag.Get(TalismanName) is not null;

        if (hasTalisman)
            Output(PassComment.Get());
        else
        {
            Output(Localizer.Msg("NoProtection"));
            killPlayer();
        }
    }
}
