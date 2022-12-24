using System.Drawing;
using System.Runtime.Remoting;
using System.Text;

var mapInput = new StringInputProvider("Input.txt");

List<Blizzard> initialBlizzards = new();

var tileWorld = new TileWorld(mapInput, false, (x, y, c, fillFunc) =>
{
    var t = new Tile(x, y, c != '#', fillFunc);

    if (c == '<' || c == '>' || c == '^' || c == 'v')
    {
        initialBlizzards.Add(new Blizzard() { CharRepresentation = c, CurrentTile = t, DirectionOfTravel = c switch
        {
            '>' => Direction.Right,
            '<' => Direction.Left,
            '^' => Direction.Up,
            'v' => Direction.Down,
            _ => throw new Exception()
        }});
    }

    return t;
});

var blizzardServices = new BlizzardServices(initialBlizzards, tileWorld);

var startTile = tileWorld.WorldObjects.Cast<Tile>().First(w => w.Position.Y == 0 && w.IsTraversable == true);
var endTile = tileWorld.WorldObjects.Cast<Tile>().First(w => w.Position.Y == tileWorld.MaxY && w.IsTraversable == true);

var startState = new State(0, startTile, tileWorld, blizzardServices, "initialState", null);
var endState = new EndState(endTile, blizzardServices, tileWorld);

var path = AStarPathfinder.FindPath(startState, endState,
    s => s.CurrentPlayerPosition.Position.Distance(endTile.Position),
    s => s.GetFollowingStates());

if (path == null)
    throw new Exception();


int totalPath = path.Count - 1;

Console.WriteLine($"Part 1: {totalPath}");

var startState2 = path.Last();
var endState2 = new EndState(startTile, blizzardServices, tileWorld);

path = AStarPathfinder.FindPath(startState2, endState2,
    s => s.CurrentPlayerPosition.Position.Distance(endTile.Position),
    s => s.GetFollowingStates());

if (path == null)
    throw new Exception();

totalPath += path.Count - 1;

var startState3 = path.Last();
var endState3 = new EndState(endTile, blizzardServices, tileWorld);

path = AStarPathfinder.FindPath(startState3, endState3,
    s => s.CurrentPlayerPosition.Position.Distance(endTile.Position),
    s => s.GetFollowingStates());

totalPath += path.Count - 1;
Console.WriteLine($"Part 2: {totalPath}");


enum Direction { Up, Down, Left, Right };

class Blizzard : IWorldObject
{
    public Tile CurrentTile { get; init; }

    public Direction DirectionOfTravel { get; init; }

    public Point Position => this.CurrentTile.Position;

    public char CharRepresentation { get; init; }

    public int Z => 1;

    public Blizzard MakeMove(TileWorld world)
    {
        (int newX, int newY) = this.DirectionOfTravel switch
        {
            Direction.Left => (this.CurrentTile.Position.X - 1, this.CurrentTile.Position.Y),
            Direction.Right => (this.CurrentTile.Position.X + 1, this.CurrentTile.Position.Y),
            Direction.Up => (this.CurrentTile.Position.X, this.CurrentTile.Position.Y - 1),
            Direction.Down => (this.CurrentTile.Position.X, this.CurrentTile.Position.Y + 1),
            _ => throw new Exception()
        };

        // hardcoded values, assuming we never hit the corner case where we are exactly at entrances

        if (newX == 0) newX = world.MaxX - 1;
        if (newY == 0) newY = world.MaxY - 1;

        if (newX == world.MaxX) newX = 1;
        if (newY == world.MaxY) newY = 1;

        return new Blizzard()
            {
                CharRepresentation = this.CharRepresentation,
                DirectionOfTravel = this.DirectionOfTravel,
                CurrentTile = world.GetTileAt(newX, newY)
            };
    }
}

class BlizzardServices
{
    private readonly Dictionary<int, HashSet<Point>> BlizzardsForMinute = new();
    private readonly TileWorld world;

    private List<Blizzard> blizzardsAtLastGeneratedPoint;
    private int lastGeneratedMinute = 0;

    public BlizzardServices(List<Blizzard> initialBlizzards, TileWorld world)
    {
        this.world = world;
        this.blizzardsAtLastGeneratedPoint = initialBlizzards.ToList();
    }

    public HashSet<Point> GetForbiddenPointsForMinute(int minute)
    {
        if (!BlizzardsForMinute.ContainsKey(minute))
        {
            GenerateMoreBlizzards();
        }

        return this.BlizzardsForMinute[minute];
    }

    private void GenerateMoreBlizzards()
    {
        for (int i = 1; i <= 10; i++)
        {
            this.blizzardsAtLastGeneratedPoint = this.blizzardsAtLastGeneratedPoint.Select(w => w.MakeMove(this.world)).ToList();

            this.lastGeneratedMinute++;
            this.BlizzardsForMinute[this.lastGeneratedMinute] = this.blizzardsAtLastGeneratedPoint.Select(w => w.Position).ToHashSet();
        }
    }
}

class EndState : State
{
    public EndState(Tile playerPosition, BlizzardServices services, TileWorld world)
        : base(int.MaxValue, playerPosition, world, services, "end state", null)
    {
    }

    public override bool Equals(State? other)
    {
        if (other == null) return false;
        else return this.CurrentPlayerPosition.Position.Equals(other.CurrentPlayerPosition.Position);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        else if (obj is EndState endstate)
        {
            return true;
        }
        else if (obj is State state)
        {
            return this.Equals(state);
        }
        else return false;
    }
}

class State: INode, IEquatable<State>
{
    public int Cost => 1;

    public int Minute { get; }

    public Tile CurrentPlayerPosition { get; }

    public TileWorld World { get; }

    private readonly BlizzardServices blizzardServices;

    public State? PreviousState { get; }

    public string TransitionString { get; }

    public State(int minute, Tile currentPosition, TileWorld world, BlizzardServices blizzardServices, string transitionString, State? previousState)
    {
        this.Minute = minute;
        this.CurrentPlayerPosition = currentPosition;
        this.World = world;
        this.blizzardServices = blizzardServices;
        this.PreviousState = previousState;
        this.TransitionString = transitionString;
    }

    public IEnumerable<State> GetFollowingStates()
    {
        var forbiddenPositions = this.blizzardServices.GetForbiddenPointsForMinute(this.Minute + 1);

        foreach (var tile in this.CurrentPlayerPosition.TraversibleNeighbours)
        {
            if (forbiddenPositions.Contains(tile.Position))
                continue;

            yield return new State(this.Minute + 1, tile, this.World, blizzardServices, "move", this);
        }

        if (!forbiddenPositions.Contains(this.CurrentPlayerPosition.Position))
            yield return new State(this.Minute + 1, this.CurrentPlayerPosition, this.World, blizzardServices, "wait in place", this);
    }

    public virtual bool Equals(State? other)
    {
        if (other == null) return false;
        else if (other is EndState endstate)
        {
            return endstate.Equals(this);
        }
        else
        {
            return this.Minute == other.Minute && this.CurrentPlayerPosition.Equals(other.CurrentPlayerPosition);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        else if (obj is State state)
        {
            return this.Equals(state);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return this.CurrentPlayerPosition.GetHashCode() * this.Minute;
    }
}