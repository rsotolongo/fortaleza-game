using System.Text;

namespace Fortaleza.Api.Games;

public static class TrackFile
{
    public const int MaxBytes = 64 * 1024;
    public const int MaxLines = 1000;
    public const int MaxLineLength = 200;

    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    static TrackFile() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static bool TryParse(byte[] bytes, out IReadOnlyList<string> commands, out string error)
    {
        var lines = Decode(bytes)
            .Split(['\r', '\n', '\x1a'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => line != "")
            .ToList();

        commands = lines;
        error = Validate(lines);
        return error == "";
    }

    public static string Validate(IReadOnlyList<string> commands)
    {
        if (commands.Count > MaxLines)
            return $"The track has more than {MaxLines} commands.";
        if (commands.Any(line => line.Length > MaxLineLength || line.Any(char.IsControl)))
            return $"A command is longer than {MaxLineLength} characters or has control characters.";
        if (commands.Any(line => !GameRunner.IsPlayable(line)))
            return "The track contains commands that are not allowed.";
        return "";
    }

    private static string Decode(byte[] bytes)
    {
        try
        {
            return StrictUtf8.GetString(bytes).TrimStart('﻿');
        }
        catch (DecoderFallbackException)
        {
            return Encoding.GetEncoding(850).GetString(bytes);
        }
    }
}
