namespace Fortaleza.Engine;

public static class StringHelper
{
    private static readonly (char from, char to)[] Diacritics =
    [
        ('á','a'),('é','e'),('í','i'),('ó','o'),('ú','u'),('ü','u'),('ñ','n'),
        ('Á','A'),('É','E'),('Í','I'),('Ó','O'),('Ú','U'),('Ü','U'),('Ñ','N'),
    ];

    public static string Normalize(string s)
    {
        var chars = s.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            foreach (var (from, to) in Diacritics)
            {
                if (chars[i] == from) { chars[i] = to; break; }
            }
        }
        return new string(chars).ToUpperInvariant();
    }

    public static bool ContainsAllWords(string name, string query)
    {
        var nameWords = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var match = 0;
        var i = 0;
        while (match < queryWords.Length && i < nameWords.Length)
        {
            var j = 0;
            while (match < queryWords.Length && j < queryWords.Length)
            {
                if (nameWords[i] == queryWords[j])
                    match++;
                j++;
            }
            i++;
        }
        return queryWords.Length > 0 && match >= queryWords.Length;
    }
}
