using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Inventory()
    : Room(new("maleta", "suitcase"), new("", ""), LWeight + 1)
{
    public const int LWeight = 40;

    public bool IsEmpty => Children.Count == 0;
    public int Heaviness => Children.Sum(t => t.Weight);
    public bool CanAdd(int weight) => Heaviness + weight <= LWeight;

    public override void Describe()
    {
        foreach (var child in Children)
            Output($"{child.Name.Get()} ({child.Weight})");

        if (IsEmpty) return;

        var remaining = LWeight - Heaviness;
        if (remaining == 0)
            Output(Localizer.Msg("CannotCarryMore"));
        else if (remaining == 1)
            Output(Localizer.Msg("AlmostFull"));
        else
            Output(Localizer.Msg("CouldCarryMore") + remaining + Localizer.Msg("MoreBags"));
    }
}
