using Fortaleza.Localization;

namespace Fortaleza.Engine;

public class Hidden(
    LocalizedText name, LocalizedText description,
    string breakerName, Thing hiddenChild)
    : Thing(name, description, 0)
{
    public string BreakerName { get; } = breakerName;
    public Thing HiddenChild { get; } = hiddenChild;

    public bool TryBreak(string weaponName) =>
        StringHelper.Normalize(weaponName) == StringHelper.Normalize(BreakerName);
}
