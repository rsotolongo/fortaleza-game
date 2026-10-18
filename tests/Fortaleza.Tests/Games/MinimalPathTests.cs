using Fortaleza.Engine;
using Fortaleza.Games;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Games;

public class MinimalPathTests
{
    [Theory]
    [InlineData(1, Language.Spanish, "FORT1_ES.TRK")]
    [InlineData(1, Language.English, "FORT1_EN.TRK")]
    [InlineData(2, Language.Spanish, "FORT2_ES.TRK")]
    [InlineData(2, Language.English, "FORT2_EN.TRK")]
    public void Track_PlayedFromTheStart_ReachesTheEndingOnItsLastCommand(int game, Language language, string file)
    {
        Localizer.Current = language;
        GameEngine engine = game == 1 ? Fort1Builder.Build() : Fort2Builder.Build();
        engine.Start();
        var commands = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "saves", file))
            .Where(line => line != "")
            .ToList();
        var reachedAt = -1;

        var original = Console.Out;
        Console.SetOut(TextWriter.Null);
        try
        {
            for (var i = 0; i < commands.Count && reachedAt < 0; i++)
            {
                engine.ProcessCommand(commands[i]);
                if (engine.Player.Dead)
                    break;
                if (engine.CheckGoal())
                    reachedAt = i;
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
