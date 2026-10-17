using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Games;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Games;

public class Fort1Tests
{
    private static HashSet<Room> ReachableRooms(Room start)
    {
        var seen = new HashSet<Room>();
        var pending = new Stack<Room>([start]);
        while (pending.Count > 0)
        {
            var room = pending.Pop();
            if (!seen.Add(room)) continue;
            foreach (var thing in room.Children)
                foreach (var link in LinksOf(thing))
                    pending.Push(link.Destination);
        }
        return seen;
    }

    private static IEnumerable<Link> LinksOf(Thing thing)
    {
        switch (thing)
        {
            case Link link:
                yield return link;
                break;
            case Hidden hidden:
                foreach (var inner in LinksOf(hidden.HiddenChild))
                    yield return inner;
                break;
        }
    }

    [Fact]
    public void Build_RegistersThirtyThreeRoomsStartingAtTheExterior()
    {
        Localizer.Current = Language.Spanish;

        var game = Fort1Builder.Build();

        Assert.Equal("el exterior de la fortaleza", game.GetRoom(0).Name.Es);
        Assert.Equal("el Jardín", game.GetRoom(12).Name.Es);
        Assert.Equal("un pasillo del Laberinto", game.GetRoom(30).Name.Es);
        Assert.Throws<ArgumentOutOfRangeException>(() => game.GetRoom(33));
    }

    [Fact]
    public void Build_AllFiftyRoomsAreReachableFromTheStart()
    {
        var game = Fort1Builder.Build();

        var reachable = ReachableRooms(game.GetRoom(0));

        Assert.Equal(50, reachable.Count);
    }

    [Fact]
    public void CheckGoal_AtStart_IsFalse()
    {
        var game = Fort1Builder.Build();

        Assert.False(game.CheckGoal());
    }

    [Fact]
    public void CheckGoal_AfterTheFiveFoesAreGone_IsTrue()
    {
        var game = Fort1Builder.Build();
        RemoveFrom(game, 21, "Centro del cerebro");
        RemoveFrom(game, 20, "Centro del corazon");
        RemoveFrom(game, 19, "Centro del estomago");
        RemoveFrom(game, 18, "Centro de los pulmones");
        RemoveFrom(game, 11, "Troll");

        Assert.True(game.CheckGoal());
    }

    [Theory]
    [InlineData(21)]
    [InlineData(20)]
    [InlineData(19)]
    [InlineData(18)]
    [InlineData(11)]
    public void CheckGoal_WithOneFoeLeft_IsFalse(int survivorRoom)
    {
        var game = Fort1Builder.Build();
        var foes = new (int Room, string Name)[]
        {
            (21, "Centro del cerebro"), (20, "Centro del corazon"), (19, "Centro del estomago"),
            (18, "Centro de los pulmones"), (11, "Troll")
        };
        foreach (var (room, name) in foes.Where(f => f.Room != survivorRoom))
            RemoveFrom(game, room, name);

        Assert.False(game.CheckGoal());
    }

    [Fact]
    public void Build_EveryHiddenAndGuardWeaponMatchesAnItemOfTheSameEnglishName()
    {
        var game = Fort1Builder.Build();
        var rooms = ReachableRooms(game.GetRoom(0));
        var itemNames = rooms.SelectMany(r => Flatten(r.Children)).OfType<Item>().Select(i => i.Name).ToList();
        var weapons = rooms.SelectMany(r => Flatten(r.Children))
            .Select(t => t switch { Guard g => g.LethalWeapon, Hidden h => h.BreakerName, _ => default })
            .Where(w => w.Es != null && w.Es != "")
            .ToList();

        foreach (var weapon in weapons)
            Assert.Contains(itemNames, name =>
                StringHelper.Normalize(name.Es).StartsWith(StringHelper.Normalize(weapon.Es))
                && StringHelper.Normalize(name.En).StartsWith(StringHelper.Normalize(weapon.En)));
    }

    private static IEnumerable<Thing> Flatten(IEnumerable<Thing> things)
    {
        foreach (var thing in things)
        {
            yield return thing;
            if (thing is Hidden hidden)
                foreach (var inner in Flatten([hidden.HiddenChild]))
                    yield return inner;
        }
    }

    private static void RemoveFrom(Fort1 game, int index, string name)
    {
        var room = game.GetRoom(index);
        room.Take(name);
    }
}
