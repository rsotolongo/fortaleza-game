using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Hidden(
    LocalizedText name, LocalizedText description,
    string breakerName, Thing hiddenChild)
    : Thing(name, description, Inventory.LWeight + 1)
{
    public string BreakerName { get; } = breakerName;
    public Thing HiddenChild { get; } = hiddenChild;

    public bool TryBreak(string weaponName) =>
        BreakerName == ""
        || StringHelper.ContainsAllWords(
            StringHelper.Normalize(BreakerName), StringHelper.Normalize(weaponName));
}
