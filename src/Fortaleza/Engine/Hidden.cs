using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Hidden(
    LocalizedText name, LocalizedText description,
    LocalizedText breakerName, Thing hiddenChild)
    : Thing(name, description, Inventory.LWeight + 1)
{
    public LocalizedText BreakerName { get; } = breakerName;
    public Thing HiddenChild { get; } = hiddenChild;

    public bool TryBreak(string weaponName) =>
        BreakerName.Es == ""
        || StringHelper.ContainsAllWords(BreakerName, StringHelper.Normalize(weaponName));
}
