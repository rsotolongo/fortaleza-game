using Fortaleza.Localization;

namespace Fortaleza.Engine;

public sealed class Item(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight)
{
}
