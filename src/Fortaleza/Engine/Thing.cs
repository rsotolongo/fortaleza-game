using Fortaleza.Localization;

namespace Fortaleza.Engine;

internal abstract class Thing(LocalizedText name, LocalizedText description, int weight)
{
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
        get => Width.Value is 0 ? 77 : Width.Value;
        set => Width.Value = value;
    }

    public static TextWriter? OutputWriter
    {
        get => Sink.Value;
        set => Sink.Value = value;
    }

    public LocalizedText Name { get; } = name;
    public LocalizedText Description { get; } = description;
    public int Weight { get; } = weight;
    public List<Thing> Children { get; } = [];

    public static void Output(string s)
    {
        if (!Visible)
        {
            return;
        }

        TextWriter writer = OutputWriter ?? Console.Out;
        int width = LineWidth;
        foreach (string line in s.Split('\n'))
        {
            string remaining = line;
            while (remaining.Length > width)
            {
                int split = remaining.LastIndexOf(' ', width);
                if (split <= 0)
                {
                    split = width;
                }

                writer.WriteLine(remaining[..split]);
                remaining = remaining[split..].TrimStart();
            }
            writer.WriteLine(remaining);
        }
    }

    public virtual string Image()
    {
        return Description.Get();
    }
}
