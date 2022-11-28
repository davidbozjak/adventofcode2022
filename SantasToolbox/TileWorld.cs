using System.Drawing;

namespace SantasToolbox
{
    [System.Diagnostics.DebuggerDisplay("({Position.X}, {Position.Y})")]
    public class Tile : IWorldObject, INode, IEquatable<Tile>
    {
        public Point Position { get; }
        
        public virtual char CharRepresentation => this.IsTraversable ? '.' : '#';

        public int Z => 0;

        public bool IsTraversable { get; }

        private readonly Cached<IEnumerable<Tile>> cachedNeighbours;

        public IEnumerable<Tile> TraversibleNeighbours => this.cachedNeighbours.Value;

        public int Cost => 1;

        public Tile(int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc)
        {
            Position = new Point(x, y);
            this.IsTraversable = isTraversable;
            this.cachedNeighbours = new Cached<IEnumerable<Tile>>(() => fillTraversibleNeighboursFunc(this).ToList());
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

    public class TileWorld : IWorld
    {
        private readonly List<Tile> allTiles = new();
        private readonly bool allowDiagnoalNeighbours;

        public IEnumerable<IWorldObject> WorldObjects => this.allTiles;

        public TileWorld(IEnumerable<string> map, bool allowDiagnoalNeighbours, Func<int, int, char, Func<Tile, IEnumerable<Tile>>, Tile> tileCreatingFunc)
        {
            this.allowDiagnoalNeighbours = allowDiagnoalNeighbours;

            int y = 0;
            foreach (var line in map)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    allTiles.Add(tileCreatingFunc(x, y, c, GetTraversibleNeighboursOfTile));
                }
                y++;
            }
        }

        public Tile GetTileAt(int x, int y)
            => GetTileAt(new Point(x, y));

        public Tile GetTileAt(Point point) =>
            this.allTiles.First(w => w.Position == point);

        private IEnumerable<Tile> GetTraversibleNeighboursOfTile(Tile tile)
        {
            Func<Point, Point, bool> neighbourFunc = this.allowDiagnoalNeighbours ?
                (p1, p2) => p1.IsNeighbourWithDiagnoals(p2) :
                (p1, p2) => p1.IsNeighbour(p2);

            return this.allTiles.Where(w => w.IsTraversable &&
                neighbourFunc(w.Position, tile.Position));
        }
    }
}
