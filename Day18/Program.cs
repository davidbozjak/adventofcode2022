using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;

var cubeFactory = new UniqueFactory<(int, int, int), Cube>(w => new Cube(w.Item1, w.Item2, w.Item3));

var cubes = new InputProvider<Cube?>("Input.txt", GetCube).Where(w => w != null).Cast<Cube>().ToList();

foreach (var cube in cubes)
{
    cube.SetNeighbours(cubeFactory);
}

var totalExposedSides = cubes.Sum(w => w.ExposedSides);
Console.WriteLine($"Part 1: {totalExposedSides}");

int maxX = cubes.Select(W => W.X).Max();
int minX = cubes.Select(W => W.X).Min();
int maxY = cubes.Select(W => W.Y).Max();
int minY = cubes.Select(W => W.Y).Min();
int maxZ = cubes.Select(W => W.Z).Max();
int minZ = cubes.Select(W => W.Z).Min();

int edgeMaxX = maxX + 1;
int edgeMinX = minX - 1;
int edgeMaxY = maxY + 1;
int edgeMinY = minY - 1;
int edgeMaxZ = maxZ + 1;
int edgeMinZ = minZ- 1;

var totalVoxels = (edgeMaxX - edgeMinX + 1) * (edgeMaxY - edgeMinY + 1) * (edgeMaxZ - edgeMinZ + 1);

var visitedLocations = new HashSet<(int, int, int)>();
IterativeFill(edgeMinX, edgeMinY, edgeMinZ, visitedLocations);

var airPockets = new List<Cube>();

for (int x = minX; x <= maxX; x++)
{
    for (int y = minY; y <= maxY; y++)
    {
        for (int z = minZ; z <= maxZ; z++)
        {
            if (!visitedLocations.Contains((x, y, z)))
            {
                if (!cubeFactory.InstanceForIdentifierExists((x, y, z)))
                {
                    var airCube = new Cube(x, y, z);
                    airCube.SetNeighbours(cubeFactory);
                    airPockets.Add(airCube);
                }
            }
        }
    }
}

// Count all rechable tiles from the neighbours, and count them mutlipel times if they are reachable multiple times
// - that gives us the number of sides which is exactly what we need
var airPocketNeighbours = airPockets.SelectMany(w => w.Neighbours).ToList();

Console.WriteLine($"Part 2: {totalExposedSides - airPocketNeighbours.Count}");

void IterativeFill(int startX, int startY, int startZ, HashSet<(int, int, int)> visited)
{
    var reachable = new Queue<(int, int, int)>();
    reachable.Enqueue((startX, startY, startZ));

    while (reachable.Count > 0)
    {
        (int x, int y, int z) = reachable.Dequeue();

        if (x > edgeMaxX) continue;
        if (y > edgeMaxY) continue;
        if (z > edgeMaxZ) continue;
                          
        if (x < edgeMinX) continue;
        if (y < edgeMinY) continue;
        if (z < edgeMinZ) continue;

        if (visited.Contains((x, y, z))) continue;

        visited.Add((x, y, z));

        if (cubeFactory.InstanceForIdentifierExists((x, y, z)))
        {
            cubeFactory.GetOrCreateInstance((x, y, z)).CanBeReachedFromEdge = true;
        }
        else
        {
            reachable.Enqueue((x + 1, y, z));
            reachable.Enqueue((x - 1, y, z));

            reachable.Enqueue((x, y + 1, z));
            reachable.Enqueue((x, y - 1, z));

            reachable.Enqueue((x, y, z + 1));
            reachable.Enqueue((x, y, z - 1));
        }
    }
}

bool GetCube(string? input, out Cube? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 3) throw new Exception();

    value = cubeFactory.GetOrCreateInstance((numbers[0], numbers[1], numbers[2]));
    
    return true;
}

class Cube : IWorldObject
{
    public Point Position { get; }

    public char CharRepresentation => 'x';

    public int Z { get; }

    public int X => this.Position.X;

    public int Y => this.Position.Y;

    public Cube(int x, int y, int z)
    {
        this.Position = new Point(x, y);
        this.Z = z;
    }

    public bool CanBeReachedFromEdge { get; set; } = false;

    private readonly List<Cube> neighbours = new();
    public IEnumerable<Cube> Neighbours => this.neighbours;

    public void SetNeighbours(UniqueFactory<(int, int, int), Cube> cubeFactory)
    {
        this.neighbours.Clear();

        AddNeighbourIfExists(-1, 0, 0);
        AddNeighbourIfExists(1, 0, 0);

        AddNeighbourIfExists(0, -1, 0);
        AddNeighbourIfExists(0, 1, 0);

        AddNeighbourIfExists(0, 0, -1);
        AddNeighbourIfExists(0, 0, 1);

        void AddNeighbourIfExists(int diffX, int diffY, int diffZ) 
        {
            if (cubeFactory.InstanceForIdentifierExists((this.X + diffX, this.Y + diffY, this.Z + diffZ)))
            {
                this.neighbours.Add(cubeFactory.GetOrCreateInstance((this.X + diffX, this.Y + diffY, this.Z + diffZ)));
            } 
        }
    }

    public int ExposedSides => 6 - this.neighbours.Count;
}