using Fortaleza.Engine;

namespace Fortaleza.Games;

internal sealed class Fort1(Room start) : GameEngine(start)
{
    private static readonly (int Room, string Name)[] Targets =
    [
        (21, "Centro del cerebro"),
        (20, "Centro del corazon"),
        (19, "Centro del estomago"),
        (18, "Centro de los pulmones"),
        (11, "Troll"),
    ];

    public override bool CheckGoal()
    {
        return Array.TrueForAll(Targets, target => GetRoom(target.Room).Get(target.Name) is null);
    }
}
