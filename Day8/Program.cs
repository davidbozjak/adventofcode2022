using System.Drawing;

var treeMap = new StringInputProvider("Input.txt").ToList();

var tiles = new List<TreeTile>();

int mapHeight = treeMap.Count;
int mapWidth = treeMap[0].Length;

for (int y = 0; y < mapHeight; y++)
{
    var row = treeMap[y];
    for (int x = 0; x < mapWidth; x++)
    {
        int treeHeight = row[x] - '0';

        var cell = new TreeTile(x, y, treeHeight);
        tiles.Add(cell);
    }
}

foreach (var tile in tiles)
{
    tile.IsVisibleFromBottom = IsVisibleFromBottom(tile, tiles);
    tile.IsVisibleFromTop = IsVisibleFromTop(tile, tiles);
    tile.IsVisibleFromLeft = IsVisibleFromLeft(tile, tiles);
    tile.IsVisibleFromRight = IsVisibleFromRight(tile, tiles);

    tile.ViewDistanceBottom = GetViewDistanceBottom(tile, tiles);
    tile.ViewDistanceTop = GetViewDistanceTop(tile, tiles);
    tile.ViewDistanceLeft = GetViewDistanceLeft(tile, tiles);
    tile.ViewDistanceRight = GetViewDistanceRight(tile, tiles);
}

var world = new SimpleWorld<TreeTile>(tiles);
var printer = new WorldPrinter();
//printer.Print(world);

Console.WriteLine($"Part 1: {tiles.Count(w => w.IsVisibleFromAnywhere)}");
Console.WriteLine($"Part 2: {tiles.Max(w => w.ScenicScore)}");

bool IsVisibleFromTop(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.Y  == 0 ||
        tile.Position.Y == mapHeight - 1) return true;

    for (int y = tile.Position.Y - 1; y >= 0; y--)
    {
        var t = GetTileAt(tile.Position.X, y, allTiles);

        if (t.TreeHeight >= tile.TreeHeight)
            return false;
    }

    return true;
}

bool IsVisibleFromBottom(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.Y == 0 ||
        tile.Position.Y == mapHeight - 1) return true;

    for (int y = tile.Position.Y + 1; y < mapHeight; y++)
    {
        var t = GetTileAt(tile.Position.X, y, allTiles);

        if (t.TreeHeight >= tile.TreeHeight)
            return false;
    }

    return true;
}

bool IsVisibleFromRight(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.X == 0 ||
        tile.Position.X == mapWidth - 1) return true;

    for (int x = tile.Position.X + 1; x < mapWidth; x++)
    {
        var t = GetTileAt(x, tile.Position.Y, allTiles);

        if (t.TreeHeight >= tile.TreeHeight)
            return false;
    }

    return true;
}

bool IsVisibleFromLeft(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.X == 0 ||
        tile.Position.X == mapWidth) return true;

    for (int x = tile.Position.X - 1; x >= 0; x--)
    {
        var t = GetTileAt(x, tile.Position.Y, allTiles);

        if (t.TreeHeight >= tile.TreeHeight)
            return false;
    }

    return true;
}

int GetViewDistanceTop(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.Y == 0 ||
        tile.Position.Y == mapHeight - 1) return 0;

    int distnace = 0;

    for (int y = tile.Position.Y - 1; y >= 0; y--)
    {
        var t = GetTileAt(tile.Position.X, y, allTiles);

        distnace++;

        if (t.TreeHeight >= tile.TreeHeight)
        {
            return distnace;
        }
    }

    return distnace;
}

int GetViewDistanceBottom(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.Y == 0 ||
        tile.Position.Y == mapHeight - 1) return 0;

    int distnace = 0;

    for (int y = tile.Position.Y + 1; y < mapHeight; y++)
    {
        var t = GetTileAt(tile.Position.X, y, allTiles);

        distnace++;

        if (t.TreeHeight >= tile.TreeHeight)
        {
            return distnace;
        }
    }

    return distnace;
}

int GetViewDistanceRight(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.X == 0 ||
        tile.Position.X == mapWidth - 1) return 0;

    int distnace = 0;

    for (int x = tile.Position.X + 1; x < mapWidth; x++)
    {
        var t = GetTileAt(x, tile.Position.Y, allTiles);

        distnace++;

        if (t.TreeHeight >= tile.TreeHeight)
        {
            return distnace;
        }
    }

    return distnace;
}

int GetViewDistanceLeft(TreeTile tile, IEnumerable<TreeTile> allTiles)
{
    if (tile.Position.X == 0 ||
        tile.Position.X == mapWidth) return 0;

    int distnace = 0;

    for (int x = tile.Position.X - 1; x >= 0; x--)
    {
        var t = GetTileAt(x, tile.Position.Y, allTiles);

        distnace++;

        if (t.TreeHeight >= tile.TreeHeight)
        {
            return distnace;
        }
    }

    return distnace;
}

TreeTile GetTileAt(int x, int y, IEnumerable<TreeTile> allTiles) =>
    allTiles.First(w => w.Position.X == x && w.Position.Y == y);

class TreeTile : IWorldObject, INode, IEquatable<Tile>
{
    public Point Position { get; }

    //public virtual char CharRepresentation => this.TreeHeight.ToString()[0];
    public virtual char CharRepresentation => this.IsVisibleFromAnywhere ? '#' : '.'; 

    public int Z => 0;

    public int ViewDistanceTop { get; set; }
    public bool IsVisibleFromTop { get; set; }

    public int ViewDistanceBottom { get; set; }
    public bool IsVisibleFromBottom { get; set; }

    public int ViewDistanceLeft { get; set; }
    public bool IsVisibleFromLeft { get; set; }

    public int ViewDistanceRight { get; set; }
    public bool IsVisibleFromRight { get; set; }

    public bool IsVisibleFromAnywhere => IsVisibleFromTop || IsVisibleFromBottom || IsVisibleFromLeft || IsVisibleFromRight;

    public int ScenicScore => ViewDistanceRight * ViewDistanceLeft * ViewDistanceBottom * ViewDistanceTop;

    public int TreeHeight { get; }

    public int Cost => 1;

    public TreeTile(int x, int y, int height)
    {
        Position = new Point(x, y);
        this.TreeHeight = height;
    }

    public bool Equals(Tile? other)
    {
        if (other == null) return false;

        return this.Position.Equals(other.Position);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Tile);
    }

    public override int GetHashCode()
    {
        return this.Position.GetHashCode();
    }
}