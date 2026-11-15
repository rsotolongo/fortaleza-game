using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal sealed class Item(LocalizedText name, LocalizedText description, int weight)
    : Thing(name, description, weight);
