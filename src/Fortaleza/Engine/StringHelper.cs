using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal static class StringHelper
{
    private static readonly Dictionary<char, char> Diacritics = new()
    {
        ['á'] = 'a',
        ['é'] = 'e',
        ['í'] = 'i',
        ['ó'] = 'o',
        ['ú'] = 'u',
        ['ü'] = 'u',
        ['ñ'] = 'n',
        ['Á'] = 'A',
        ['É'] = 'E',
        ['Í'] = 'I',
        ['Ó'] = 'O',
        ['Ú'] = 'U',
        ['Ü'] = 'U',
        ['Ñ'] = 'N',
    };

    public static string Normalize(string s)
    {
        char[] chars = s.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = Diacritics.GetValueOrDefault(chars[i], chars[i]);
        }

        return new string(chars).ToUpperInvariant();
    }

    public static bool ContainsAllWords(LocalizedText name, string query)
    {
        return ContainsAllWords(Normalize(name.Es), query) || ContainsAllWords(Normalize(name.En), query);
    }

    public static bool ContainsAllWords(string name, string query)
    {
        string[] nameWords = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string[] queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int match = 0;
        for (int i = 0; match < queryWords.Length && i < nameWords.Length; i++)
        {
            for (int j = 0; match < queryWords.Length && j < queryWords.Length; j++)
            {
                if (nameWords[i] == queryWords[j])
                {
                    match++;
                }
            }
        }
        return queryWords.Length > 0 && match >= queryWords.Length;
    }

    public static bool EqualsName(LocalizedText name, string typed)
    {
        return Normalize(name.Es) == Normalize(typed) || Normalize(name.En) == Normalize(typed);
    }
}
