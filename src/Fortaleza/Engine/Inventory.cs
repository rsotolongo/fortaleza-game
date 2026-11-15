using System.Globalization;
using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal sealed class Inventory()
    : Room(new("maleta", "suitcase"), new(string.Empty, string.Empty), LWeight + 1)
{
    public const int LWeight = 40;

    public bool IsEmpty => Children.Count is 0;
    public int Heaviness => Children.Sum(static t => t.Weight);
    public bool CanAdd(int weight)
    {
        return Heaviness + weight <= LWeight;
    }

    public override void Describe()
    {
        foreach (Thing child in Children)
        {
            Output(string.Create(CultureInfo.InvariantCulture, $"{child.Name.Get()} ({child.Weight})"));
        }

        if (IsEmpty)
        {
            return;
        }

        int remaining = LWeight - Heaviness;
        if (remaining is 0)
        {
            Output(Localizer.Msg("CannotCarryMore"));
        }
        else if (remaining is 1)
        {
            Output(Localizer.Msg("AlmostFull"));
        }
        else
        {
            Output(Localizer.Msg("CouldCarryMore") + remaining.ToString(CultureInfo.InvariantCulture) + Localizer.Msg("MoreBags"));
        }
    }
}
