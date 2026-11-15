using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal sealed class Daughter(
    LocalizedText name,
    LocalizedText description,
    LocalizedText confession,
    LocalizedText weapon,
    Action killPlayer)
    : Guard(name, description, confession, weapon)
{
    public override bool Die(string weapon)
    {
        if (StringHelper.EqualsName(LethalWeapon, weapon))
        {
            Output(Confession.Get());
            Dead = true;
        }
        else
        {
            Output(Localizer.Msg("DaughterFail"));
            killPlayer();
        }
        return Dead;
    }
}
