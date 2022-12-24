using System.Drawing;

var mapInput = new StringInputProvider("Input.txt");

var elfs = new List<Elf>();

var tileWorld = new TileWorld(mapInput, true, (x, y, c, fillFunc) =>
{
    var t = new Tile(x, y, true, fillFunc);

    if (c == '#')
    {
        elfs.Add(new Elf(t));
    }

    return t;
});

foreach (var elf in elfs)
    elf.World = tileWorld;

var printer = new WorldPrinter();

int part1MaxRounds = 10;

var cyclcicalOrderProvider = new CyclicalElementProvider<Direction>(new[] {
    () => Direction.East,
    () => Direction.North,
    () => Direction.South,
    () => Direction.West,
});

for (int round = 0; true; round++)
{
    if (round % 100 == 0)
        Console.WriteLine($"{DateTime.Now.TimeOfDay}: Round {round + 1}");

    var order = cyclcicalOrderProvider.Take(4).ToList();
    cyclcicalOrderProvider.MoveNext();

    var elfsLocations = elfs.ToDictionary(w => w.Position);

    if (round == part1MaxRounds)
    {
        int maxX = elfs.Select(w => w.Position.X).Max();
        int maxY = elfs.Select(w => w.Position.Y).Max();
        int minX = elfs.Select(w => w.Position.X).Min();
        int minY = elfs.Select(w => w.Position.Y).Min();

        int totalSurfaceArea = (maxX - minX + 1) * (maxY - minY + 1);

        Console.WriteLine($"{DateTime.Now.TimeOfDay}:Part 1: {totalSurfaceArea - elfs.Count}");
    }

    var elfsThatCanMoveThisRound = elfs.Where(w => w.HasNeighbouringElf(elfsLocations));

    if (!elfsThatCanMoveThisRound.Any())
    {
        Console.WriteLine($"{DateTime.Now.TimeOfDay}:Part 2: No elf moved in round {round + 1}");
        break;
    }

    var allTilesAndElfs = elfsThatCanMoveThisRound.Select(w => new { Elf = w, ChosenPosition = w.ConsiderNextPosition(order, elfsLocations) })
        .Where(w => w.ChosenPosition != null)
        .GroupBy(w => w.ChosenPosition)
        .ToDictionary(group => group.Key, elements => elements.ToArray());

    var elfsToMove = new List<Elf>();

    foreach (var keyValuePair in allTilesAndElfs)
    {
        if (keyValuePair.Value.Length == 1)
        {
            elfsToMove.Add(keyValuePair.Value[0].Elf);
        }
    }

    foreach (var elf in elfsToMove)
    {
        elf.MoveToNextPosition();
    }
}

enum Direction { North = 1, South = 2, East = 3, West = 4 };

class Elf : IWorldObject
{
    public TileWorld World { get; set;  }
    public Tile CurrentPosition { get; private set; }

    public Point Position => this.CurrentPosition.Position;

    public char CharRepresentation => '#';

    public int Z => 1;

    public Elf(Tile originalPosition)
    {
        this.CurrentPosition = originalPosition;
    }

    private Tile? proposedNextTile;

    public Tile? ConsiderNextPosition(IEnumerable<Direction> directionOrder, Dictionary<Point, Elf> elfsLocations)
    {
        proposedNextTile = null;

        if (!this.HasNeighbouringElf(elfsLocations))
            throw new Exception();

        foreach (var direction in directionOrder)
        {
            var tilesToConsider = (direction switch
            {
                Direction.North => GetNorthTiles(),
                Direction.East => GetEastTiles(),
                Direction.West => GetWestTiles(),
                Direction.South => GetSouthTiles(),
                _ => throw new Exception()
            }).ToList();

            if (!tilesToConsider.Any(w => elfsLocations.ContainsKey(w.Position)))
            {
                // Assumption: the "true" north/east/west/south tile will always be the on position 0
                proposedNextTile = tilesToConsider.First();
                break;
            }
        }

        return proposedNextTile;
    }

    public void MoveToNextPosition()
    {
        this.CurrentPosition = proposedNextTile ?? throw new Exception();
        proposedNextTile = null;
    }

    public bool HasNeighbouringElf(Dictionary<Point, Elf> elfsLocations)
    {
        return new[] { GetNorthTiles(), GetSouthTiles(), GetEastTiles(), GetWestTiles() }
            .SelectMany(w => w)
            .Any(w => elfsLocations.ContainsKey(w.Position));
    }

    private IEnumerable<Tile> GetNorthTiles()
    {
        return new[]
        {
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X, this.Position.Y - 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X - 1, this.Position.Y - 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X + 1, this.Position.Y - 1)
        };
    }

    private IEnumerable<Tile> GetSouthTiles()
    {
        return new[]
        {
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X, this.Position.Y + 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X - 1, this.Position.Y + 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X + 1, this.Position.Y + 1)
        };
    }

    private IEnumerable<Tile> GetEastTiles()
    {
        return new[]
        {
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X + 1, this.Position.Y),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X + 1, this.Position.Y - 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X + 1, this.Position.Y + 1)
        };
    }

    private IEnumerable<Tile> GetWestTiles()
    {
        return new[]
        {
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X - 1, this.Position.Y),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X - 1, this.Position.Y - 1),
            World.GetOrCreateTileAt(this.CurrentPosition.Position.X - 1, this.Position.Y + 1)
        };
    }
}