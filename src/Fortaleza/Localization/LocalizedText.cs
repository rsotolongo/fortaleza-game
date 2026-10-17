namespace Fortaleza.Localization;

public readonly record struct LocalizedText(string Es, string En)
{
    public string Get() => Localizer.Current == Language.English ? En : Es;

    public static implicit operator LocalizedText(string both) => new(both, both);
}
