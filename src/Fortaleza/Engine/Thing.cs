using Fortaleza.Localization;

namespace Fortaleza.Engine;

public abstract class Thing
{
    public LocalizedText Name { get; }
    public LocalizedText Description { get; }
    public int Weight { get; }
    public List<Thing> Children { get; } = new();
    public static bool Visible { get; set; } = true;

    protected Thing(LocalizedText name, LocalizedText description, int weight)
    {
        Name = name;
        Description = description;
        Weight = weight;
    }

    public virtual string Image() => Description.Get();

    public void Output(string s)
    {
        if (!Visible) return;
        foreach (var line in s.Split('\n'))
        {
            var remaining = line;
            while (remaining.Length > 77)
            {
                var split = remaining.LastIndexOf(' ', 77);
                if (split <= 0) split = 77;
                Console.WriteLine(remaining[..split]);
                remaining = remaining[split..].TrimStart();
            }
            Console.WriteLine(remaining);
        }
    }
}
