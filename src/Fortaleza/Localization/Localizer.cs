using System.Reflection;
using System.Text.Json;

namespace Fortaleza.Localization;

internal static class Localizer
{
    private static readonly AsyncLocal<Language> CurrentLanguage = new();
    private static Dictionary<string, string>? _esMessages;
    private static Dictionary<string, string>? _enMessages;

    public static Language Current
    {
        get => CurrentLanguage.Value;
        set
        {
            CurrentLanguage.Value = value;
            EnsureLoaded();
        }
    }

    public static string Msg(string key)
    {
        return Msg(key, CurrentLanguage.Value);
    }

    public static string Msg(string key, Language language)
    {
        EnsureLoaded();
        Dictionary<string, string> dict = language is Language.English ? _enMessages! : _esMessages!;
        return dict.TryGetValue(key, out string? val) ? val : $"[{key}]";
    }

    private static void EnsureLoaded()
    {
        if (_esMessages is not null)
        {
            return;
        }

        _esMessages = LoadResource("messages.es.json");
        _enMessages = LoadResource("messages.en.json");
    }

    private static Dictionary<string, string> LoadResource(string filename)
    {
        Assembly assembly = typeof(Localizer).Assembly;
        string resourceName = $"Fortaleza.Localization.res.{filename}";
        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource not found: {resourceName}");
        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? throw new InvalidOperationException($"Failed to deserialize {resourceName}");
    }
}
