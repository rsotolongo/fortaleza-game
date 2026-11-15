namespace Fortaleza.Localization;

internal readonly record struct LocalizedText(string Es, string En)
{
    public static implicit operator LocalizedText(string both) => new(both, both);

    public string Get()
    {
        return Localizer.Current is Language.English ? En : Es;
    }
}
