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
}
