using Fortaleza.Localization;

namespace Fortaleza.Engine;

public abstract class Thing
{
    public LocalizedText Name { get; }
    public LocalizedText Description { get; }
    public int Weight { get; }
    public List<Thing> Children { get; } = new();
    private static readonly AsyncLocal<bool> Muted = new();
    private static readonly AsyncLocal<TextWriter?> Sink = new();
    private static readonly AsyncLocal<int> Width = new();

    public static bool Visible
    {
        get => !Muted.Value;
        set => Muted.Value = !value;
    }

    public static int LineWidth
    {
        get => Width.Value == 0 ? 77 : Width.Value;
        set => Width.Value = value;
    }

    public static TextWriter? OutputWriter
    {
        get => Sink.Value;
        set => Sink.Value = value;
    }

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
        var writer = OutputWriter ?? Console.Out;
        var width = LineWidth;
        foreach (var line in s.Split('\n'))
        {
            var remaining = line;
            while (remaining.Length > width)
            {
                var split = remaining.LastIndexOf(' ', width);
                if (split <= 0) split = width;
                writer.WriteLine(remaining[..split]);
                remaining = remaining[split..].TrimStart();
            }
            writer.WriteLine(remaining);
        }
    }
}
