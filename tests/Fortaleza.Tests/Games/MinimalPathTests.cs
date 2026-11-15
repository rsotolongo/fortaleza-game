using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Games;

public sealed class MinimalPathTests
{
    [Theory]
    [InlineData(1, nameof(Language.Spanish), "FORT1_ES.TRK")]
    [InlineData(1, nameof(Language.English), "FORT1_EN.TRK")]
    [InlineData(2, nameof(Language.Spanish), "FORT2_ES.TRK")]
    [InlineData(2, nameof(Language.English), "FORT2_EN.TRK")]
    public void TrackPlayedFromTheStartReachesTheEndingOnItsLastCommand(int game, string language, string file)
    {
        Localizer.Current = Enum.Parse<Language>(language);
        GameEngine engine = game is 1 ? Fort1Builder.Build() : Fort2Builder.Build();
        engine.Start();
        var commands = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "saves", file))
            .Where(static line => !string.IsNullOrEmpty(line))
            .ToList();
        int reachedAt = -1;

        TextWriter original = Console.Out;
        Console.SetOut(TextWriter.Null);
        try
        {
            for (int i = 0; i < commands.Count && reachedAt < 0 && !engine.Player.Dead; i++)
            {
                engine.ProcessCommand(commands[i]);
                if (!engine.Player.Dead && engine.CheckGoal())
                {
                    reachedAt = i;
                }
            }
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.False(engine.Player.Dead);
        Assert.Equal(commands.Count - 1, reachedAt);
    }
}
