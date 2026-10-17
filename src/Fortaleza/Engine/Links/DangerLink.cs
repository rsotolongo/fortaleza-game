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
        var normalized = StringHelper.Normalize(TalismanName);
        var hasTalisman = bag.Children.Any(
            t => StringHelper.Normalize(t.Name.Es) == normalized
              || StringHelper.Normalize(t.Name.En) == normalized);

        if (hasTalisman)
            Output(PassComment.Get());
        else
        {
            Output(Localizer.Msg("NoProtection"));
            killPlayer();
        }
    }
}
