using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Guard(
    LocalizedText name, LocalizedText description,
    LocalizedText confession, string lethalWeapon)
    : LivingThing(name, description, Inventory.LWeight + 1)
{
    public LocalizedText Confession { get; } = confession;
    public string LethalWeapon { get; } = lethalWeapon;

    public override bool Die(string weapon)
    {
        if (StringHelper.ContainsAllWords(
                StringHelper.Normalize(LethalWeapon), StringHelper.Normalize(weapon)))
        {
            Output(Confession.Get());
            base.Die(weapon);
        }
        else
        {
            Output(Localizer.Msg("Useless"));
        }
        return Dead;
    }
}

public class Daughter(
    LocalizedText name, LocalizedText description,
    LocalizedText confession, string weapon, Action killPlayer)
    : Guard(name, description, confession, weapon)
{
    public override bool Die(string weapon)
    {
        if (StringHelper.Normalize(weapon) == StringHelper.Normalize(LethalWeapon))
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
