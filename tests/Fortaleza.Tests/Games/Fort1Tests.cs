using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Games;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Games;

public sealed class Fort1Tests
{
    [Fact]
    public void BuildRegistersThirtyThreeRoomsStartingAtTheExterior()
    {
        Localizer.Current = Language.Spanish;

        Fort1 game = Fort1Builder.Build();

        Assert.Equal("el exterior de la fortaleza", game.GetRoom(0).Name.Es);
        Assert.Equal("el Jardín", game.GetRoom(12).Name.Es);
        Assert.Equal("un pasillo del Laberinto", game.GetRoom(30).Name.Es);
        Assert.Throws<ArgumentOutOfRangeException>(() => game.GetRoom(33));
    }

    [Fact]
    public void BuildAllFiftyRoomsAreReachableFromTheStart()
    {
        Fort1 game = Fort1Builder.Build();

        HashSet<Room> reachable = ReachableRooms(game.GetRoom(0));

        Assert.Equal(50, reachable.Count);
    }

    [Fact]
    public void CheckGoalAtStartIsFalse()
    {
        Fort1 game = Fort1Builder.Build();

        Assert.False(game.CheckGoal());
    }

    [Fact]
    public void CheckGoalAfterTheFiveFoesAreGoneIsTrue()
    {
        Fort1 game = Fort1Builder.Build();
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
    public void CheckGoalWithOneFoeLeftIsFalse(int survivorRoom)
    {
        Fort1 game = Fort1Builder.Build();
        var foes = new (int Room, string Name)[]
        {
            (21, "Centro del cerebro"), (20, "Centro del corazon"), (19, "Centro del estomago"),
            (18, "Centro de los pulmones"), (11, "Troll"),
        };
        foreach ((int room, string? name) in foes.Where(f => f.Room != survivorRoom))
        {
            RemoveFrom(game, room, name);
        }

        Assert.False(game.CheckGoal());
    }

    [Fact]
    public void BuildEveryHiddenAndGuardWeaponMatchesAnItemOfTheSameEnglishName()
    {
        Fort1 game = Fort1Builder.Build();
        HashSet<Room> rooms = ReachableRooms(game.GetRoom(0));
        var itemNames = rooms.SelectMany(static r => Flatten(r.Children)).OfType<Item>().Select(static i => i.Name).ToList();
        var weapons = rooms.SelectMany(static r => Flatten(r.Children))
            .Select(static t => t switch { Guard g => g.LethalWeapon, Hidden h => h.BreakerName, _ => default })
            .Where(static w => w.Es is not null && !string.IsNullOrEmpty(w.Es))
            .ToList();

        foreach (LocalizedText weapon in weapons)
        {
            Assert.Contains(itemNames, name =>
                StringHelper.Normalize(name.Es).StartsWith(StringHelper.Normalize(weapon.Es), StringComparison.Ordinal)
                && StringHelper.Normalize(name.En).StartsWith(StringHelper.Normalize(weapon.En), StringComparison.Ordinal));
        }
    }

    private static HashSet<Room> ReachableRooms(Room start)
    {
        var seen = new HashSet<Room>();
        var pending = new Stack<Room>([start]);
        while (pending.Count > 0)
        {
            Room room = pending.Pop();
            if (!seen.Add(room))
            {
                continue;
            }

            foreach (Thing thing in room.Children)
            {
                foreach (Link link in LinksOf(thing))
                {
                    pending.Push(link.Destination);
                }
            }
        }
        return seen;
    }

    private static IEnumerable<Link> LinksOf(Thing thing)
    {
        if (thing is Link link)
        {
            yield return link;
            yield break;
        }

        if (thing is not Hidden { HiddenChild: { } child })
        {
            yield break;
        }

        foreach (Link inner in LinksOf(child))
        {
            yield return inner;
        }
    }

    private static IEnumerable<Thing> Flatten(IEnumerable<Thing> things)
    {
        foreach (Thing thing in things)
        {
            yield return thing;
            if (thing is Hidden { HiddenChild: { } child })
            {
                foreach (Thing inner in Flatten([child]))
                {
                    yield return inner;
                }
            }
        }
    }

    private static void RemoveFrom(Fort1 game, int index, string name)
    {
        Room room = game.GetRoom(index);
        room.Take(name);
    }
}
