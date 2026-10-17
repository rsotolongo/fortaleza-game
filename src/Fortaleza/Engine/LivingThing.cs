using Fortaleza.Localization;

namespace Fortaleza.Engine;

public abstract class LivingThing(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight)
{
    public bool Dead { get; protected set; }

    public virtual bool Die(string weapon)
    {
        Dead = true;
        return true;
    }

    public virtual void Speak() => Output(Localizer.Msg("Silence"));

    public virtual void Accept(Item gift) { }
}
