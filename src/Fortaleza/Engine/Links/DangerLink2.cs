using Fortaleza.Localization;

namespace Fortaleza.Engine.Links;

internal sealed class DangerLink2(
    LocalizedText name,
    LocalizedText description,
    LocalizedText passComment,
    string key,
    string forbiddenItem,
    Room destination,
    Action killPlayer)
    : Link(name, description, passComment, key, destination)
{
    public string ForbiddenItem { get; } = forbiddenItem;

    public override void Pass(Inventory bag)
    {
        bool hasForbidden = bag.Get(ForbiddenItem) is not null;

        if (hasForbidden)
        {
            Output(Localizer.Msg("UndesirableObject"));
            killPlayer();
        }
        else
        {
            Output(PassComment.Get());
        }
    }
}
