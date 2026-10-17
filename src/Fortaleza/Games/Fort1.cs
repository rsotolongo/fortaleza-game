using Fortaleza.Engine;

namespace Fortaleza.Games;

public class Fort1(Room start) : GameEngine(start)
{
    public override bool CheckGoal() =>
        GetRoom(21).Get("Centro del cerebro") is null
        && GetRoom(20).Get("Centro del corazon") is null
        && GetRoom(19).Get("Centro del estomago") is null
        && GetRoom(18).Get("Centro de los pulmones") is null
        && GetRoom(11).Get("Troll") is null;
}
