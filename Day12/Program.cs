var input = new StringInputProvider("Input.txt").ToList();

var world = new TileWorld(input, false,
    (x, y, c, getNeighbours) => new HighTile(x, y, c, true, w => GetReachableNeighbours(w, getNeighbours(w))));

var printer = new WorldPrinter();
printer.Print(world);

var start = world.WorldObjects.Cast<HighTile>().Where(w => w.CharRepresentation == 'S').First();
var end = world.WorldObjects.Cast<HighTile>().Where(w => w.CharRepresentation == 'E').First();

var path = AStarPathfinder.FindPath<Tile>(start, end, w => w.Position.Distance(end.Position), w => w.TraversibleNeighbours);

var worldWithPath = new WorldWithPath<Tile>(world, path);
printer.Print(worldWithPath);

Console.WriteLine($"Part 1: {path.Count - 1}");

var tilesWithLowestElevation = world.WorldObjects.Cast<HighTile>().Where(w => w.Z == 0).ToList();

int min = int.MaxValue;
foreach (var potentialStart in tilesWithLowestElevation)
{
    var potentialPath = AStarPathfinder.FindPath<Tile>(potentialStart, end, w => w.Position.Distance(end.Position), w => w.TraversibleNeighbours);

    if (potentialPath == null)
        continue;

    var distance = potentialPath.Count - 1;

    if (distance < min)
    {
        min = distance;
    }
}

Console.WriteLine($"Part 2: {min}");

static IEnumerable<Tile> GetReachableNeighbours(Tile t, IEnumerable<Tile> tiles)
{
    foreach (var tile in tiles)
    {
        var diff = tile.Z - t.Z;
        if (diff <= 1)
        {
            yield return tile;
        }
    }
}

class HighTile : Tile
{
    public override int Z { get; }

    public override char CharRepresentation { get; }

    public HighTile(int x, int y, char c, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) :
        base(x, y, isTraversable, fillTraversibleNeighboursFunc)
    {
        this.CharRepresentation = c;

        if (c == 'S')
        {
            c = 'a';
        }
        else if (c == 'E')
        {
            c = 'z';
        }

        this.Z = c - 'a';
    }
}