using System.Runtime.CompilerServices;

var directionsProvider = new CyclicalElementProvider<PushDirection>(new StringInputProvider("Input.txt").First().ToCharArray()
    .Select(GetDirectionFromChar)
    .Select(w => new Func<PushDirection>(() => w))
    .ToArray()
);

var orderedPieceProvider = new CyclicalElementProvider<TetrisPiece>(new Func<TetrisPiece>[]
{
    () => new MinusTetrisPiece(),
    () => new PlusTetrisPiece(),
    () => new LTetrisPiece(),
    () => new VerticalLineTetrisPiece(),
    () => new SquareTetrisPiece()
});

var tetrisWorld = new TetrisWorld(7, orderedPieceProvider);

var printer = new WorldPrinter(skipEmptyLines: false);

int step = 0;

while (tetrisWorld.PlacedPiecesCount < 1000000000000)
{
    step++;
    tetrisWorld.MakeStep(directionsProvider.Current);
    
    //printer.Print(tetrisWorld, 0, 7, 0, tetrisWorld.Height + 5);
    //Console.WriteLine($"Step {step} Direction: {directionsProvider.Current}");
    //Console.WriteLine($"Current height: {tetrisWorld.Height} Total Placed Pieces {tetrisWorld.PlacedPiecesCount}");
    //Console.WriteLine($"Press any key to continue");
    //Console.ReadKey();

    directionsProvider.MoveNext();
}

Console.WriteLine($"Current height: {tetrisWorld.Height} Total Placed Pieces {tetrisWorld.PlacedPiecesCount}");

static PushDirection GetDirectionFromChar(char c) =>
    c switch {
        '>' => PushDirection.Right,
        '<' => PushDirection.Left,
        _ => throw new Exception()
    };
enum PushDirection { Left, Right };

class TetrisWorld : IWorld
{
    public int Width { get; }

    public int Height { get; private set; }

    public int PlacedPiecesCount => this.placedPieces.Count;

    TetrisPiece CurrentPiece => PieceEnumerator.Current;

    int CurrentX = 0;
    int CurrentY = 0;

    private readonly HashSet<(int x, int y)> occupiedPoints = new();

    public IEnumerator<TetrisPiece> PieceEnumerator { get; }

    public IEnumerable<IWorldObject> WorldObjects => 
        this.placedPieces.SelectMany(w => w.Piece.RenderAtLocation(w.X, w.Y, '#'))
        .Concat(this.CurrentPiece.RenderAtLocation(this.CurrentX, this.CurrentY, '@'))
        .Concat(this.placedPieces.Select(w => w.Y).SelectMany(w => new[] { new SimplePointWorldObject(-1, w, '|'), new SimplePointWorldObject(this.Width + 1, w, '|') }))
        .Concat(Enumerable.Range(0, this.Width).Select(w => new SimplePointWorldObject(w, 0, '-')));

    private readonly List<PlacedPiece> placedPieces = new();

    public TetrisWorld(int width, IEnumerator<TetrisPiece> getNextPieceEnumerator)
    {
        this.Width = width;
        this.PieceEnumerator = getNextPieceEnumerator;
        this.CurrentX = 2;
        this.CurrentY = 4;
    }

    public void MakeStep(PushDirection direction)
    {
        int potentialX = this.CurrentX + (direction == PushDirection.Left ? -1 : 1);

        bool canMoveHorizontally = this.CurrentPiece.Points.Select(w => (potentialX + w.diffX, CurrentY + w.diffY))
            .All(w => IsCoordinateAvaliable(w.Item1, w.Item2));

        if (canMoveHorizontally)
        {
            this.CurrentX = potentialX;
        }

        bool canMoveDown = this.CurrentPiece.Points.Select(w => (this.CurrentX + w.diffX, this.CurrentY - 1 + w.diffY))
            .All(w => IsCoordinateAvaliable(w.Item1, w.Item2));

        if (canMoveDown)
        {
            this.CurrentY--;
        }
        else
        {
            PlaceCurrentPieceAndGenerateNewOne();
        }
    }

    private void PlaceCurrentPieceAndGenerateNewOne()
    {
        this.placedPieces.Add(new PlacedPiece(this.CurrentX, this.CurrentY, this.CurrentPiece));

        foreach ((int diffX, int diffY) in this.CurrentPiece.Points)
        {
            var x = this.CurrentX + diffX;
            var y = this.CurrentY + diffY;

            this.occupiedPoints.Add((x, y));

            if (y > this.Height)
            {
                this.Height = y;
            }
        }

        this.PieceEnumerator.MoveNext();

        this.CurrentX = 2;
        this.CurrentY = this.Height + 4;
    }

    private bool IsCoordinateAvaliable(int x, int y)
    {
        if (x < 0 || x >= this.Width) return false;

        if (y <= 0) return false;

        return !this.occupiedPoints.Contains((x, y));
    }

    class PlacedPiece
    {
        public TetrisPiece Piece { get; }
        public int X { get; }
        public int Y { get; }

        public PlacedPiece(int x, int y, TetrisPiece piece)
        {
            this.X = x;
            this.Y = y;
            this.Piece = piece;
        }
    }
}
