using Fortaleza.Engine;
using Fortaleza.Engine.Links;
using Fortaleza.Games;
using Fortaleza.Localization;
using Xunit;

namespace Fortaleza.Tests.Games;

public class Fort2Tests
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

    private static IEnumerable<Link> LinksOf(Thing? thing)
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
    public void Build_RegistersFiftyFiveRoomsStartingAtTheGuestRoom()
    {
        Localizer.Current = Language.Spanish;

        var game = Fort2Builder.Build();

        Assert.Equal("una habitación para huéspedes", game.GetRoom(0).Name.Es);
        Assert.Equal("las Habitaciones del Hechicero", game.GetRoom(54).Name.Es);
        Assert.Throws<ArgumentOutOfRangeException>(() => game.GetRoom(55));
    }

    [Fact]
    public void Build_EveryRoomHasAnEnglishNameAndDescription()
    {
        var game = Fort2Builder.Build();

        for (var i = 0; i < 55; i++)
        {
            Assert.False(string.IsNullOrWhiteSpace(game.GetRoom(i).Name.En));
            Assert.False(string.IsNullOrWhiteSpace(game.GetRoom(i).Description.En));
        }
    }

    [Fact]
    public void Build_RoomsReachableFromTheStart_AreAllButTheOriginalsUnusedRoom()
    {
        var game = Fort2Builder.Build();

        var reachable = ReachableRooms(game.GetRoom(0));

        Assert.Equal(54, reachable.Count);
        Assert.DoesNotContain(game.GetRoom(29), reachable);
    }

    [Fact]
    public void CheckGoal_AtStart_IsFalse()
    {
        var game = Fort2Builder.Build();

        Assert.False(game.CheckGoal());
    }

    [Fact]
    public void CheckGoal_WhenTheDaughterIsGone_LeavesAChaliceInHerRoom()
    {
        var game = Fort2Builder.Build();
        game.GetRoom(45).Take("Hija del Hechicero");

        game.CheckGoal();
        game.CheckGoal();

        Assert.NotNull(game.GetRoom(45).Get("Cáliz"));
        Assert.Single(game.GetRoom(45).Children, t => t.Name.Es == "Cáliz");
    }

    [Fact]
    public void CheckGoal_WithTheHandkerchiefLeftInTheKitchen_KillsThePlayer()
    {
        var game = Fort2Builder.Build();
        game.GetRoom(6).Children.Add(new Item(new("Pañuelo", "Handkerchief"), new("", ""), 1));

        game.CheckGoal();

        Assert.True(game.Player.Dead);
    }
}
