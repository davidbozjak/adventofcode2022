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

var maxX = tileWorld.WorldObjects.Select(w => w.Position.X).Max();
var maxY = tileWorld.WorldObjects.Select(w => w.Position.Y).Max();

var minX = tileWorld.WorldObjects.Select(w => w.Position.X).Min();
var minY = tileWorld.WorldObjects.Select(w => w.Position.Y).Min();

var startTile = tileWorld.WorldObjects.Cast<Tile>().First(w => w.Position.Y == 0 && w.IsTraversable == true);
var endTile = tileWorld.WorldObjects.Cast<Tile>().First(w => w.Position.Y == maxY && w.IsTraversable == true);

var startState = new State(startTile, tileWorld, initialBlizzards, "initialState", null);
var endState = new EndState(endTile, tileWorld);

int minDistance = int.MaxValue;

var path = AStarPathfinder.FindPath(startState, endState,
    s => 
    {
        int value = s.CurrentPlayerPosition.Position.Distance(endTile.Position);
        if (value < minDistance)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}: {value}");
            minDistance = value;
            
        }
        return value;
    },
    s => s.GetFollowingStates());

if (path == null)
    throw new Exception();

//var printer = new WorldPrinter();
//foreach (var state in path)
//{
//    var objects = tileWorld.WorldObjects.ToList();
//    objects.AddRange(state.Blizzards);
//    objects.Add(new SimplePointWorldObject(state.CurrentPlayerPosition.Position.X, state.CurrentPlayerPosition.Position.Y, 'E'));

//    var world = new SimpleWorld<IWorldObject>(objects);
//    printer.Print(world);

//    Console.WriteLine($"State action: {state.TransitionString}, previous {state.PreviousState?.CurrentPlayerPosition.Position}");

//    Console.ReadKey();
//}


Console.WriteLine($"Part 1: {path.Count}");

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

class EndState : State
{
    public EndState(Tile playerPosition, TileWorld world)
        : base(playerPosition, world, Enumerable.Empty<Blizzard>(), "end state", null)
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

    public Tile CurrentPlayerPosition { get; }

    public TileWorld World { get; }

    private readonly List<Blizzard> blizzards;
    public IReadOnlyCollection<Blizzard> Blizzards => this.blizzards;

    private readonly Cached<string> stringRepresentation;
    public string StringRepresentation => this.stringRepresentation.Value;

    public State? PreviousState { get; }

    public string TransitionString { get; }

    public State(Tile currentPosition, TileWorld world, IEnumerable<Blizzard> blizzards, string transitionString, State? previousState)
    {
        this.CurrentPlayerPosition = currentPosition;
        this.World = world;
        this.blizzards = blizzards.ToList();
        this.PreviousState = previousState;
        this.TransitionString = transitionString;
        
        this.stringRepresentation = new Cached<string>(GetStringRepresentation);
    }

    public IEnumerable<State> GetFollowingStates()
    {
        var newBlizzards = this.Blizzards.Select(w => w.MakeMove(this.World));

        var forbiddenPositions = newBlizzards.Select(w => w.Position).ToHashSet();

        foreach (var tile in this.CurrentPlayerPosition.TraversibleNeighbours)
        {
            if (forbiddenPositions.Contains(tile.Position))
                continue;

            yield return new State(tile, this.World, newBlizzards, "move", this);
        }

        if (!forbiddenPositions.Contains(this.CurrentPlayerPosition.Position))
            yield return new State(this.CurrentPlayerPosition, this.World, newBlizzards, "wait in place", this);
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
            return this.stringRepresentation.Value.Equals(other.StringRepresentation);
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
        return this.stringRepresentation.Value.GetHashCode();
    }

    public string GetStringRepresentation()
    {
        return $"[{this.CurrentPlayerPosition.Position.X},{this.CurrentPlayerPosition.Position.Y}]"
            + string.Join(',', this.Blizzards.OrderBy(w => w.Position.ReadingOrder()).Select(w => $"[{w.CharRepresentation},{w.Position.X},{w.Position.Y}]"));
    }
}