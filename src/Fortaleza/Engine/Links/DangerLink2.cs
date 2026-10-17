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
        var hasForbidden = bag.Get(ForbiddenItem) is not null;

        if (hasForbidden)
        {
            Output(Localizer.Msg("UndesirableObject"));
            killPlayer();
        }
        else
            Output(PassComment.Get());
    }
}
