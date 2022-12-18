using System.Drawing;

abstract class TetrisPiece
{
    protected abstract (int diffX, int diffY)[] Pieces { get; }

    public IEnumerable<(int diffX, int diffY)> Points => Pieces;

    public IEnumerable<IWorldObject> RenderAtLocation(int x, int y, char c)
    {
        return this.Points.Select(w => new SimplePointWorldObject(x + w.diffX, y + w.diffY, c));
    }
}

class MinusTetrisPiece : TetrisPiece
{
    static readonly (int diffX, int diffY)[] pieces = new[]
    {
        (0, 0),
        (1, 0),
        (2, 0),
        (3, 0),
    };
    protected override (int diffX, int diffY)[] Pieces => pieces;
}

class PlusTetrisPiece : TetrisPiece
{
    static readonly (int diffX, int diffY)[] pieces = new[]
    {
        (1, 0),
        (0, 1),
        (1, 1),
        (2, 1),
        (1, 2),
    };
    protected override (int diffX, int diffY)[] Pieces => pieces;
}

class SquareTetrisPiece : TetrisPiece
{
    static readonly (int diffX, int diffY)[] pieces = new[]
    {
        (0, 0),
        (1, 0),
        (0, 1),
        (1, 1),
    };
    protected override (int diffX, int diffY)[] Pieces => pieces;
}

class VerticalLineTetrisPiece : TetrisPiece
{
    static readonly (int diffX, int diffY)[] pieces = new[]
    {
        (0, 0),
        (0, 1),
        (0, 2),
        (0, 3),
    };
    protected override (int diffX, int diffY)[] Pieces => pieces;
}

class LTetrisPiece : TetrisPiece
{
    static readonly (int diffX, int diffY)[] pieces = new[]
    {
        (0, 0),
        (1, 0),
        (2, 0),
        (2, 1),
        (2, 2),
    };
    protected override (int diffX, int diffY)[] Pieces => pieces;
}