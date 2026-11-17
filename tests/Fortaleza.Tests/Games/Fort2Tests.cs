using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Games;
using Fortaleza.Localization;

namespace Fortaleza.Tests.Games;

/// <summary>
/// Tests for fort2.
/// </summary>
public sealed class Fort2Tests
{
    /// <summary>
    /// Build registers fifty five rooms starting at the guest room.
    /// </summary>
    [Fact]
    public void BuildRegistersFiftyFiveRoomsStartingAtTheGuestRoom()
    {
        Localizer.Current = Language.Spanish;

        Fort2 game = Fort2Builder.Build();

        Assert.Equal("una habitación para huéspedes", game.GetRoom(0).Name.Es);
        Assert.Equal("las Habitaciones del Hechicero", game.GetRoom(54).Name.Es);
        Assert.Throws<ArgumentOutOfRangeException>(() => game.GetRoom(55));
    }

    /// <summary>
    /// Build every room has an english name and description.
    /// </summary>
    [Fact]
    public void BuildEveryRoomHasAnEnglishNameAndDescription()
    {
        Fort2 game = Fort2Builder.Build();

        for (int i = 0; i < 55; i++)
        {
            Assert.False(string.IsNullOrWhiteSpace(game.GetRoom(i).Name.En));
            Assert.False(string.IsNullOrWhiteSpace(game.GetRoom(i).Description.En));
        }
    }

    /// <summary>
    /// Build rooms reachable from the start are all but the originals unused room.
    /// </summary>
    [Fact]
    public void BuildRoomsReachableFromTheStartAreAllButTheOriginalsUnusedRoom()
    {
        Fort2 game = Fort2Builder.Build();

        HashSet<Room> reachable = ReachableRooms(game.GetRoom(0));

        Assert.Equal(54, reachable.Count);
        Assert.DoesNotContain(game.GetRoom(29), reachable);
    }

    /// <summary>
    /// Check goal at start is false.
    /// </summary>
    [Fact]
    public void CheckGoalAtStartIsFalse()
    {
        Fort2 game = Fort2Builder.Build();

        Assert.False(game.CheckGoal());
    }

    /// <summary>
    /// Check goal when the daughter is gone leaves a chalice in her room.
    /// </summary>
    [Fact]
    public void CheckGoalWhenTheDaughterIsGoneLeavesAChaliceInHerRoom()
    {
        Fort2 game = Fort2Builder.Build();
        game.GetRoom(45).Take("Hija del Hechicero");

        game.CheckGoal();
        game.CheckGoal();

        Assert.NotNull(game.GetRoom(45).Get("Cáliz"));
        Assert.Single(game.GetRoom(45).Children, static t => t.Name.Es is "Cáliz");
    }

    /// <summary>
    /// Check goal with the handkerchief left in the kitchen kills the player.
    /// </summary>
    [Fact]
    public void CheckGoalWithTheHandkerchiefLeftInTheKitchenKillsThePlayer()
    {
        Fort2 game = Fort2Builder.Build();
        game.GetRoom(6).Children.Add(new Item(new("Pañuelo", "Handkerchief"), new(string.Empty, string.Empty), 1));

        game.CheckGoal();

        Assert.True(game.Player.Dead);
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

    private static IEnumerable<Link> LinksOf(Thing? thing)
    {
        if (thing is Link link)
        {
            yield return link;
            yield break;
        }

        if (thing is not Hidden hidden)
        {
            yield break;
        }

        foreach (Link inner in LinksOf(hidden.HiddenChild))
        {
            yield return inner;
        }
    }
}
