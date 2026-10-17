using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

public class DangerLink2(
    LocalizedText name, LocalizedText description,
    LocalizedText passComment, string key,
    string forbiddenItem, Room destination, Action killPlayer)
    : Link(name, description, passComment, key, destination)
{
    public string ForbiddenItem { get; } = forbiddenItem;

    public override void Pass(Inventory bag)
    {
        var normalized = StringHelper.Normalize(ForbiddenItem);
        var hasForbidden = bag.Children.Any(
            t => StringHelper.Normalize(t.Name.Es) == normalized
              || StringHelper.Normalize(t.Name.En) == normalized);

        if (hasForbidden)
        {
            Output(Localizer.Msg("UndesirableObject"));
            killPlayer();
        }
        else
            Output(PassComment.Get());
    }
}
