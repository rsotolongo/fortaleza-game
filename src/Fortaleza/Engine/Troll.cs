using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Troll(
    LocalizedText name, LocalizedText description, string likeness,
    LocalizedText hiData, LocalizedText loData)
    : LivingThing(name, description, 100)
{
    public string Likeness { get; } = likeness;
    public LocalizedText HiData { get; } = hiData;
    public LocalizedText LoData { get; } = loData;
    public bool Happy { get; private set; }

    public override void Speak() => Output(Happy ? HiData.Get() : LoData.Get());

    public override void Accept(Item gift)
    {
        Happy = StringHelper.Normalize(gift.Name.Es) == StringHelper.Normalize(Likeness);
        Output(Localizer.Msg(Happy ? "VeryThanks" : "CoolThanks"));
    }

    public override bool Die(string weapon)
    {
        Output(Localizer.Msg("Ahhhh"));
        return base.Die(weapon);
    }
}
