using System.Drawing;
using System.Text.RegularExpressions;

var tilesFactory = new UniqueFactory<(int x, int y), Tile>(w => new Tile(w.x, w.y));

var rockLines = new StringInputProvider("Input.txt").ToList();

foreach (var line in rockLines)
{
    CreateTilesForLine(line);
}

var world = new SimpleWorld<Tile>(tilesFactory.AllCreatedInstances);
var printer = new WorldPrinter();
printer.PrintToFile(world, "Output_Clean.txt");

var absoluteMaxY = tilesFactory.AllCreatedInstances.Max(w => w.Position.Y);
int absoluteMinY = tilesFactory.AllCreatedInstances.Min(w => w.Position.Y);

bool simulateFloor = false;

while (PlaceSandBelow(500, 0))
{
}

world = new SimpleWorld<Tile>(tilesFactory.AllCreatedInstances);
printer.PrintToFile(world, "Output_Part1.txt");

Console.WriteLine($"Part 1: {tilesFactory.AllCreatedInstances.Count(w => w.State == GroundType.Sand)}");

simulateFloor = true;

while (GetTileAt(500, 0).State != GroundType.Sand)
{
    if (!PlaceSandBelow(500, 0))
        throw new Exception();
}

world = new SimpleWorld<Tile>(tilesFactory.AllCreatedInstances);
printer.PrintToFile(world, "Output_Part2.txt");

Console.WriteLine($"Part 2: {tilesFactory.AllCreatedInstances.Count(w => w.State == GroundType.Sand)}");

bool PlaceSandBelow(int StartX, int StartY)
{
    var currentTile = GetTileAt(StartX, StartY);
    //go down as far as it goes
    for (int y = StartY; true; y++)
    {
        if (!simulateFloor && y > absoluteMaxY)
            return false;

        currentTile.State = GroundType.SandyAir;

        var tileBelow = GetTileAt(StartX, y + 1);

        if (tileBelow.State == GroundType.Rock)
            break;

        if (tileBelow.State == GroundType.Sand)
            break;

        currentTile = tileBelow;
    }

    var leftBelow = GetTileAt(currentTile.Position.X - 1, currentTile.Position.Y + 1);
    
    if (leftBelow.State != GroundType.Sand && leftBelow.State != GroundType.Rock)
        return PlaceSandBelow(currentTile.Position.X - 1, currentTile.Position.Y + 1);

    var rightBelow = GetTileAt(currentTile.Position.X + 1, currentTile.Position.Y + 1);

    if (rightBelow.State != GroundType.Sand && rightBelow.State != GroundType.Rock)
        return PlaceSandBelow(currentTile.Position.X + 1, currentTile.Position.Y + 1);

    // both left and right are blocked, stay at rest here
    currentTile.State = GroundType.Sand;
    return true;
}

Tile GetTileAt(int x, int y)
{
    if (!simulateFloor) 
        return tilesFactory.GetOrCreateInstance((x, y));
    else
    {
        var floorY = absoluteMaxY + 2;

        if (y >= floorY) 
            return new Tile(x, y) { State = GroundType.Rock };
        else return tilesFactory.GetOrCreateInstance((x, y));
    }
}
    

void CreateTilesForLine(string line)
{
    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(line).Select(w => int.Parse(w.Value)).ToArray();

    var points = new List<(int x, int y)>();

    for (int i = 0; i < numbers.Length - 1; i += 2)
    {
        points.Add((numbers[i], numbers[i + 1]));
    }

    for (int i = 1; i < points.Count; i++)
    {
        var start = points[i - 1];
        var end = points[i];

        if (start.x == end.x)
        {
            CreateVerticalLineOfPoints(start.x, start.y, end.y);
        }
        else if (start.y == end.y)
        {
            CreateHorizontalLineOfPoints(start.y, start.x, end.x);
        }
        else throw new Exception();
    }

    void CreateVerticalLineOfPoints(int x, int y1, int y2)
    {
        var minY = Math.Min(y1, y2);
        var maxY = Math.Max(y1, y2);

        CreateLineOfPoints(x, minY, (x, y) => (x, y + 1), (x, y) => y <= maxY);
    }

    void CreateHorizontalLineOfPoints(int y, int x1, int x2)
    {
        var minX = Math.Min(x1, x2);
        var maxX = Math.Max(x1, x2);

        CreateLineOfPoints(minX, y, (x, y) => (x + 1, y), (x, y) => x <= maxX);
    }
    void CreateLineOfPoints(int x, int y, Func<int, int, (int, int)> iteratorFunc, Func<int, int, bool> conditionFunc)
    {
        while (conditionFunc(x, y))
        {
            var tile = tilesFactory.GetOrCreateInstance((x, y));
            tile.State = GroundType.Rock;

            (x, y) = iteratorFunc(x, y);
        }
    }
}

enum GroundType { Air, Sand, Rock, SandyAir }

class Tile : IWorldObject
{
    public Point Position { get; }

    public char CharRepresentation => this.State switch
    {
        GroundType.Air => '.',
        GroundType.Sand => 'o',
        GroundType.Rock => '#',
        GroundType.SandyAir => '~',
        _ => throw new Exception()
    };

    public GroundType State { get; set; }

    public int Z => 1;

    public Tile(int x, int y)
    {
        this.Position = new Point(x, y);
    }
}