using System.Globalization;
using System.Text;
using Fortaleza.Parser;

namespace Fortaleza.Api.Games;

public static class TrackFile
{
    public const int MaxBytes = 64 * 1024;
    public const int MaxLines = 2000;
    public const int MaxLineLength = 255;

    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    static TrackFile() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static bool TryParse(byte[] bytes, out IReadOnlyList<string> commands, out string error)
    {
        commands = [];
        error = "";

        var lines = Decode(bytes)
            .Split(['\r', '\n', '\x1a'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => line != "")
            .ToList();

        if (lines.Count == 0)
            error = "The track file is empty.";
        else if (lines.Count > MaxLines)
            error = $"The track file has more than {MaxLines} commands.";
        else if (lines.Any(line => line.Length > MaxLineLength))
            error = $"A command is longer than {MaxLineLength} characters.";
        else if (lines.Any(line => !GameSession.IsPlayable(line)))
            error = "The track file contains commands that are not allowed.";
        else
            commands = lines;

        return error == "";
    }

    public static byte[] Encode(IEnumerable<string> commands)
    {
        var text = string.Concat(commands.Select(command => StripAccents(command) + "\r\n"));
        return Encoding.ASCII.GetBytes(text);
    }

    public static string StripAccents(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var c in text.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        return builder.ToString().Normalize(NormalizationForm.FormC);
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
