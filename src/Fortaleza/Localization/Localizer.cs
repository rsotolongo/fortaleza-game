using System.Reflection;
using System.Text.Json;

namespace Fortaleza.Localization;

public enum Language { Spanish, English }

public static class Localizer
{
    private static Language _current = Language.Spanish;
    private static Dictionary<string, string>? _esMessages;
    private static Dictionary<string, string>? _enMessages;

    public static Language Current
    {
        get => _current;
        set
        {
            _current = value;
            EnsureLoaded();
        }
    }

    public static string Msg(string key)
    {
        EnsureLoaded();
        var dict = _current == Language.English ? _enMessages! : _esMessages!;
        return dict.TryGetValue(key, out var val) ? val : $"[{key}]";
    }

    private static void EnsureLoaded()
    {
        if (_esMessages is not null) return;
        _esMessages = LoadResource("messages.es.json");
        _enMessages = LoadResource("messages.en.json");
    }

    private static Dictionary<string, string> LoadResource(string filename)
    {
        var assembly = typeof(Localizer).Assembly;
        var resourceName = $"Fortaleza.Localization.res.{filename}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource not found: {resourceName}");
        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? throw new InvalidOperationException($"Failed to deserialize {resourceName}");
    }
}
