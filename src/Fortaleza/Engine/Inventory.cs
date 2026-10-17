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
        if (IsEmpty)
        {
            Output(Localizer.Msg("BagEmpty"));
            return;
        }
        Output(Localizer.Msg("BagContains"));
        foreach (var child in Children)
            Output($"  {child.Name.Get()} ({child.Weight})");

        var remaining = LWeight - Heaviness;
        if (remaining == 0)
            Output(Localizer.Msg("CannotCarryMore"));
        else if (remaining <= 5)
            Output(Localizer.Msg("AlmostFull"));
        else
            Output(Localizer.Msg("CouldCarryMore") + remaining
                + (remaining == 1 ? Localizer.Msg("BagUnit") : Localizer.Msg("BagUnits")));
    }
}
