using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Guard(
    LocalizedText name, LocalizedText description,
    LocalizedText confession, string lethalWeapon)
    : LivingThing(name, description, 0)
{
    public LocalizedText Confession { get; } = confession;
    public string LethalWeapon { get; } = lethalWeapon;

    public override bool Die(string weapon)
    {
        if (string.IsNullOrEmpty(weapon)
            || StringHelper.Normalize(weapon) != StringHelper.Normalize(LethalWeapon))
        {
            Output(Localizer.Msg("Useless"));
            return false;
        }
        Output(Confession.Get());
        Dead = true;
        return true;
    }
}

public class Daughter(
    LocalizedText name, LocalizedText description,
    LocalizedText confession, string weapon)
    : Guard(name, description, confession, weapon)
{
    public override bool Die(string _)
    {
        Dead = true;
        Output(Confession.Get());
        return true;
    }
}
