using System.Data;
using System.Drawing;

//var directionsInput = "10R5L5R10L4R5L5";
var directionsInput = "17L25L32R15L31R29R9L13R49L39R47L22L28R49L16L14L48R11R15L22L25L16R27R28R39L44L47R49L38R15L34L18L21R41R39R46L7R42R16L27L3L32L28R3R3L49L27L37R49L46R20L12R45L17L35L2L8L23R14R35L44L29R22R50L31L44L36R12R37R34R24R33R33L43L20R27L12L24L50L14R46L41R34R35R16R33L41R34L38R22L4L46L12R42R23R20L43L34R22R46L12R44R42R17R47R25L4R18R13R13R18R20R24R20L14L21R6L13L21L5L22L26R50R18L48R20L35L29L24R48L15R36R41L3L40R12R10R13L21L25R43R25L2R46R21L26L4R9L19R37R23L24R25R34L24L44R39R39L17R50R5R26L12R42R8R13L42R6R4R32R12L43L45R29R31L16R20R36R47R22L41L5L14L44R1R17L33L2L32R33L16R3R20L24L9R20L22R45R39R26L1R21R14L40L6R1R32R17R47L34L12L47L15R38L1R1L24L2L23L38L28R49L18R43R19L1R35R12R50L1R12R27R1L18L24R2L8R37R37L49R41R1L38L32R12R46L11L6R33L12R1R16R43R14R50R13L35L14L3L49R43R30R17R45R30R8L27R37R40L29R38R25R29R21R37L45R27R3R30L12L32R22L10R30R24L17L5R3L22R21L13L15L40L17L36L16R4R34R27L43L29R49L24R43L14R34L6L42R32R40L13L31R17R24R31R27L33L2L24R28L49L8R29L25R25R27L37R49R3L21L4R38L20R2R12R21L32L25R10L5R43L47R46L21R48R8L33L43L38L26L39L16L8L14R13R11R3L15L46L25R5R8L24L2L33L37L2R33R13R2R17R34L46L35L29L32L46L4L45L4L27L23L34R8L37L31L20L29R29L46R34L41L37L1R10R34L25R20R40L26L11L5L8R4L45R20L17L16L48L32L35R36L20L39R13L18L2L9R6L20R14R30R15R36R38L32L36R17L16R25L47L30R40L13L36R40R7R8R24L3R25R26R6L38R7R24L34L13R43R30L6L38L28R21L11R12L32R29L25R50R25L29L5L44R31R26R10R49L36R49R44R28R13L7L2L19L38R37L4L2R36R33L11L40L27L22R17L27R14R5L8R40L23L28R10L33L46L1L31L14L49R5L26L38L5R29R25L47R34L15R37L28L8R17L22L37R35L8L45R3L1R5R3R7L47L9L37R27L2R4L48L2R15R25L17L42R6R33L19L12L3R5L25L40L27L42L2L9L45L10L20L33L42L18R1R3L24L39R14L48R13L21R23R37L16L34L19L42L4L8R10R47R26R6R19L15R43R5L5R32L28L14R38R21L45L46L19L41R10L13L15L36R46L39L8L33R4L40L7L12L47L31R17L17L47L27L25L43L2L16L6L19R5R16L45L12L11L46L5R34R40L18R7L47R28L31L38L39L40R47L5R47L38R47L16L14L34R50R15R35R11L49R48L18R32L46R16L4L32L20R23L10L46R6L19R8R31R9R31R45R25R32R29L24L39R35R26L25L48L19R43L26R27L45R5L31R48R1L11L9L9R34L32L17L24R34L14L35L7L37L7R7R15R36R46R24L43R14L49R47R22R27L29L9R50R34R1L38R37L42R31R33R19R35R40L14R30L7L32R19L45R6R14L27L27L18L35R15R48L31L29L33R8R24R7R19L32R17R42R38L49R4L44R18R7R40L19R6L29L50L38R14R28L27L20R3L25L18L23L49R6L46L19L33L36L35R29R34R50L13R20R1L33R15L17L45R32L32R24R6R6L24R39R48L21L26L15L29L9L36L20L28L26L43R11R10L29L44L37L37R37L12R16L31R41L32L21L49L32L22L34L46R44R17L11R44R2R14L27R26R19R24L31L17R40R41L35R11R38R32R49R44L30L49L25L46L31R39R36R1L44R20L4L9R11L44L33R43R17R8L1R17L3L35R20L35L39R27L10R6L38R37R48R39R28R34L38L12L45R50L27L43L43R24R24R26R13L8L47L10R50L24R38L28R41R41R38R3L9R45L15R36R31R20R45L27R14L1L43L1L41L40L31R47R45L15L44R25L4L25L13L32R40L25R1R43L45R10L18L47R43L5L21L23R18L49L47R44L37R9R30R4L44R18R4L29L10R21R15L4L16L10R42L38R32R35R26R26L3L48L28R29R5R43L32L44R32R39L41L37L9R17L16L5R50L34R31L28R26L34L36R5L43R31R35L37R22L17L7R5R4L47L42L27L12L7R45R20L29R8R39R46R15R14R28L24R8R31R3L47L38R4L18L13R45L13R28R4R24L13L28L16L12L6R12L31L26R44R17R13R1L1R36L10L31L17R1L16L47L19R48R6L3L29R9L7L10R22R16L42R49L40L25L14R8R6L18R6L13L9R44R37R38L11L29R26R11L25L35L39R27L9R36L7L46R21L28R30R28L47R22R50R42L8L47R40R47R49R20R14L4R27R10L32R18L33L1R27L30L39L20R27R50L8L32R17L40R27L23L16L36R48L38R3L36L45R31L7R21L14L23R47L34L25L38L1L45R12R17L17L46R49L19L33L30R33R26L3L40L15R24R37L10L24R27L50L17R35L2R45L40R20R10R25L27R23L14L9R10R16R32L27R18R22L14L29L32L25R18R41R31L21L27R5R33L31L40R15L3R22L16L11L20R3L32L8R8L40L11L45R9L13R8L19R42R17L18L22R16L39R13R29R44R12R25R38L23L24L38L21L29L20L32L4L43L11L15R14R17L9L42R42R26R20R3R22R46L50R42L28R49R37L35R16L24R7R9R47L16R7L32L22R5L42R40R6L18L50L25R8L43L6R10L19R25L3R9L11R48R25L37L16L29L2L49R42R23R13L36R20R21L38L37L42R26L35L47R23L13L5L48R46L11R43R26R27L12L49R16R13L29R36L34R7L16L1R49L46L46L25L46R34R30L15L35R37L3L37L36L43R46L14L35R38L46L24L45L8R1L46R30R17R15L17R7R37L35L28R5R27R13L32R5L18L48L3R1R11R2R36R28R9L37R10L2R17R32R7R8L24R44R26R4R1R24L16L43R38L33L45R23L23R33L45L4R37R28R39L43R50L45R49L10R2R26L24R50L27R50L40R46R40R32L20R38R19L24R28L28R26L50L43R20R8R43R31L21L31L22L43R1R15R28R8L29L34R4L1L48L18L17R45R1R23R46R31R31L3R5L36L45L1R10L14L10L36R5L10L45L22L47L42L15L48L42R14R41L10R49L25L18R8R43R29R9R6R31R8L6R22L13L24R28R15R13L32L2R17R16L17L16R33L7L8R50L41L22L47L30L47L37R11R44R1R46R48L38L7L24L44R43L14L43L40R41R44L48L4L11L36L39L23L24L15L34L7R35R22R42R49L35R7R1L8R42L27R39L37R1L21R2R33L18L17R2L37L8L35L16R25R32R4L33L24R20R41R39L12L12L42R33L4R38L42L16L14L32R36L15L1L20R14R37R33L21L24R28L11L12L49L13R4R26R2L4L35L42R11R23R27L6R38R9R47L23L4R37R5L37L11R11L19L10L38L17R3L18R44L22R10R50L33L29L42R15L3L20L5R6R23R6R25L46L48L9L22R37L16R7L22L37R36R17R13L21L4L27L41R35L29R47R7R13R43R9L33L50R50R30L37R48R46R13R45L8L47R49R11R19L3R15R27R31R33L14R25R46R14R45R44L10L11L36R30L13L36L26L24R29R46L4R22R47R19L24L8R13R35L33L24R41R39L36L17L25L10L12R13L39R20R46L25L41R49L13L5R34L19R13R32L33L6L35R44R40R4R41L39L45R49R36L47R14R6L14R1L15L21R11R47R45R23R25R43L14L1R25R19L38L16R14L32R29L25L43L33L17L1R11L5R25L32R40L6R47R14L20L43R49R36R12R36R47L6R28R49R32R38R41R35L43L42R43R50R24R9L7R41L16R34R13R16L50R11L39L48L32R34L16L11R43R27R3R50R18L16L46L37L14L42R6R27L49L48L7L27L47R10L47R30R39L38L14R32L47L44L2L22L32L44L37L46R6R18R49L14R45R17L21R18R47L49R19R23L43R18R40L35L27L34R26R17L26R16L26R46L33R26L49L38R33R38R29R38R37R40L1L13L5R45L20L48R12L17L11R16R2R35R3R14L47L8R10R34L42R8R38R14L48L8L47R24L10L44L7R14L31R6R8L11L12R37R11R22L14L50L43R28L21R7R42L20L1L4L42L4R26R20R48R6R37R29L5L8R8R12R18L45L39R46R6R24R23R16R12R28L27R33R15R49R41L9L12L31R21L33R31R26R6R16R21L24L5R11L44L3L30L6L21R9L6L5R1L17L5R40L48L20R5R37R37L29R2R35L12R19R11L49L49L21R24R25L45R17L33L13R20L9R39L12L27L21R7L49R35L8L21L11R16R43L48L28L13L35L46L8R32R35R46R14L46L44L21R6R11L26R1L3R20L27R28L12R9R17R7L43L43L34R16L4L23L33L40L46L35R20L47R12L13L6R32L26L18L6L39L31L2L31L29R40R9";

var instructions = GetInstructions(directionsInput).ToArray();

var mapInput = new StringInputProvider("Input.txt");

var tileWorld = new TileWorld(mapInput, false, (x, y, c, fillFunc) => new TristateTile(x, y, c, fillFunc));

var printer = new WorldPrinter();
//printer.Print(tileWorld);

TristateTile currentPosition = tileWorld.WorldObjects.Cast<TristateTile>().Where(w => w.IsTraversable).OrderBy(w => w.Position.ReadingOrder()).First();
var currentOrientation = Orientation.Right;

var path = new List<TristateTile>() { currentPosition };

foreach (var instruction in instructions)
{
    for (int step = 0; step < instruction.NumberOfSteps; step++)
    {
        var tile = GetTile(currentPosition, currentOrientation, tileWorld);

        if (tile == null || tile.IsNonExistant)
        {
            (var newPosition, var newOrientation) = WrapUsingP2Rules(currentPosition, currentOrientation, tileWorld);

            if (newPosition.IsTraversable)
            {
                currentPosition = newPosition;
                currentOrientation = newOrientation;
            }
            else if (newPosition.IsNonExistant)
            {
                throw new Exception();
            }
            else
            {
                break;
            }
        }
        else if (tile.IsTraversable)
        {
            currentPosition = tile;
        }
        else
        {
            break;
        }

        path.Add(currentPosition);
    }

    currentOrientation = instruction.TurnAfterwards switch
    {
        Turn.None => currentOrientation,
        Turn.Clockwise => TurnClockwise(currentOrientation),
        Turn.Counterclockwise => TurnCounterClockwise(currentOrientation),
        _ => throw new Exception()
    };
}

var worldWithPath = new WorldWithPath<TristateTile>(tileWorld, path);
printer.PrintToFile(worldWithPath, "resultingPath.txt");

var password = (currentPosition.Position.Y + 1) * 1000 + (currentPosition.Position.X + 1)* 4 + 
    (currentOrientation switch
        {
            Orientation.Right => 0,
            Orientation.Down => 1,
            Orientation.Left => 2,
            Orientation.Up => 3,
            _ => throw new Exception()
        });

Console.WriteLine($"Password: {password}");

static TristateTile? GetTile(Tile currentPosition, Orientation orientation, TileWorld tileWorld)
{
    int potentialNewX = currentPosition.Position.X, potentialNewY = currentPosition.Position.Y;

    if (orientation == Orientation.Up)
        potentialNewY--;
    else if (orientation == Orientation.Down)
        potentialNewY++;
    else if (orientation == Orientation.Left)
        potentialNewX--;
    else if (orientation == Orientation.Right)
        potentialNewX++;
    else throw new Exception();

    return tileWorld.GetTileAtOrNull(potentialNewX, potentialNewY) as TristateTile;
}

static Orientation TurnClockwise(Orientation current)
{
    return current switch
    {
        Orientation.Up => Orientation.Right,
        Orientation.Right => Orientation.Down,
        Orientation.Down => Orientation.Left,
        Orientation.Left => Orientation.Up,
        _ => throw new Exception()
    };
}

static Orientation TurnCounterClockwise(Orientation current)
{
    return current switch
    {
        Orientation.Up => Orientation.Left,
        Orientation.Left => Orientation.Down,
        Orientation.Down => Orientation.Right,
        Orientation.Right => Orientation.Up,
        _ => throw new Exception()
    };
}

static IEnumerable<Instruction> GetInstructions(string input)
{
    int currentNumber = 0;

    if (!char.IsDigit(input[0]))
        throw new Exception();

    for (int index = 0; index < input.Length; index++)
    {
        char c = input[index];

        if (char.IsDigit(c))
        {
            currentNumber *= 10;
            currentNumber += c - '0';
        }
        else
        {
            Turn t = getTurn(c);
            yield return new Instruction(currentNumber, t);
            currentNumber = 0;
        }
    }

    Turn finalT = getTurn(input[^1]);
    yield return new Instruction(currentNumber, finalT);

    static Turn getTurn(char c)
    {
        if (char.IsDigit(c)) return Turn.None;
        else return c == 'R' ? Turn.Clockwise : Turn.Counterclockwise;
    }
}

static TristateTile WrapUsingP1Rules(TristateTile currentPosition, Orientation currentOrientation, TileWorld tileWorld)
{
    TristateTile? tile;
    var opositeOrientation = currentOrientation switch
    {
        Orientation.Left => Orientation.Right,
        Orientation.Right => Orientation.Left,
        Orientation.Up => Orientation.Down,
        Orientation.Down => Orientation.Up,
        _ => throw new Exception()
    };

    var tileAtWrapped = currentPosition;
    do
    {
        tile = tileAtWrapped;
        tileAtWrapped = GetTile(tileAtWrapped, opositeOrientation, tileWorld);
    } while (tileAtWrapped != null && !tileAtWrapped.IsNonExistant);

    if (tile == null)
        throw new Exception();

    return tile;
}

static (TristateTile, Orientation) WrapUsingP2Rules(TristateTile currentPosition, Orientation currentOrientation, TileWorld tileWorld)
{
    int cubeSize = 50;

    var cubes = new[] 
    {
        new CubeSide(1, new Point(50, 0), new Point(99, 49)),
        new CubeSide(2, new Point(100, 0), new Point(149, 49)),
        new CubeSide(3, new Point(50, 50), new Point(99, 99)),
        new CubeSide(4, new Point(0, 100), new Point(49, 149)),
        new CubeSide(5, new Point(50, 100), new Point(99, 149)),
        new CubeSide(6, new Point(0, 150), new Point(49, 199)),
    };

    var currentCubeFace = cubes.First(w => w.ContainsPoint(currentPosition.Position));
    (var relativeX, var relativeY) = currentCubeFace.GetRelativePositionWithinCube(currentPosition.Position);

    if (relativeX < 0 || relativeX > 49) throw new Exception();
    if (relativeY < 0 || relativeY > 49) throw new Exception();

    if (currentCubeFace.Id == 1)
    {
        if (currentOrientation == Orientation.Up)
        {
            //going into side 6
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [6]");
            
            var newOrientation = Orientation.Right;
            var tile = (TristateTile)tileWorld.GetTileAt(0, 150 + relativeX);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 6)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Down)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Left)
        {
            //going into side 4
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [4]");
            var newOrientation = Orientation.Right;
            var tile = (TristateTile)tileWorld.GetTileAt(0, 100 + cubeSize - relativeY - 1);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 4)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Right)
        {
            throw new Exception();
        }
        else throw new Exception();
    }
    else if (currentCubeFace.Id == 2)
    {
        if (currentOrientation == Orientation.Up)
        {
            //going into side 6
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [6]");
            var newOrientation = Orientation.Up;
            var tile = (TristateTile)tileWorld.GetTileAt(relativeX, 199);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 6)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Down)
        {
            //going into side 3
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [3]");
            var newOrientation = Orientation.Left;
            var tile = (TristateTile)tileWorld.GetTileAt(99, 50 + relativeX);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 3)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Left)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Right)
        {
            //going into side 5
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [5]");
            var newOrientation = Orientation.Left;
            var tile = (TristateTile)tileWorld.GetTileAt(99, 100 + cubeSize - relativeY - 1);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 5)
                throw new Exception();

            return (tile, newOrientation);
        }
        else throw new Exception();
    }
    else if (currentCubeFace.Id == 3)
    {
        if (currentOrientation == Orientation.Up)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Down)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Left)
        {
            //going into side 4
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [4]");
            var newOrientation = Orientation.Down;
            var tile = (TristateTile)tileWorld.GetTileAt(relativeY, 100);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 4)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Right)
        {
            //going into side 2
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [2]");
            var newOrientation = Orientation.Up;
            var tile = (TristateTile)tileWorld.GetTileAt(100 + relativeY, 49);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 2)
                throw new Exception();

            return (tile, newOrientation);
        }
        else throw new Exception();
    }
    else if (currentCubeFace.Id == 4)
    {
        if (currentOrientation == Orientation.Up)
        {
            //going into side 3
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [3]");
            var newOrientation = Orientation.Right;
            var tile = (TristateTile)tileWorld.GetTileAt(50, 50 + relativeX);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 3)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Down)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Left)
        {
            //going into side 1
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [1]");
            var newOrientation = Orientation.Right;
            var tile = (TristateTile)tileWorld.GetTileAt(50, cubeSize - relativeY - 1);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 1)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Right)
        {
            throw new Exception();
        }
        else throw new Exception();
    }
    else if (currentCubeFace.Id == 5)
    {
        if (currentOrientation == Orientation.Up)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Down)
        {
            //going into side 6
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [6]");
            var newOrientation = Orientation.Left;
            var tile = (TristateTile)tileWorld.GetTileAt(49, 150 + relativeX);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 6)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Left)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Right)
        {
            //going into side 2
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [2]");
            var newOrientation = Orientation.Left;
            var tile = (TristateTile)tileWorld.GetTileAt(149, cubeSize - relativeY - 1);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 2)
                throw new Exception();

            return (tile, newOrientation);
        }
        else throw new Exception();
    }
    else if (currentCubeFace.Id == 6)
    {
        if (currentOrientation == Orientation.Up)
        {
            throw new Exception();
        }
        else if (currentOrientation == Orientation.Down)
        {
            //going into side 2
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [2]");
            var newOrientation = Orientation.Down;
            var tile = (TristateTile)tileWorld.GetTileAt(100 + relativeX, 0);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 2)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Left)
        {
            //going into side 1
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [1]");
            var newOrientation = Orientation.Down;

            var tile = (TristateTile)tileWorld.GetTileAt(50 + relativeY, 0);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 1)
                throw new Exception();

            return (tile, newOrientation);
        }
        else if (currentOrientation == Orientation.Right)
        {
            //going into side 5
            Console.WriteLine($"Transitioning from Side {currentCubeFace.Id} to [5]");
            var newOrientation = Orientation.Up;
            var tile = (TristateTile)tileWorld.GetTileAt(50 + relativeY, 149);

            if (cubes.First(w => w.ContainsPoint(tile.Position)).Id != 5)
                throw new Exception();

            return (tile, newOrientation);
        }
        else throw new Exception();
    }
    else throw new Exception();
}

enum Orientation { Left, Right, Up, Down };
enum Turn { Clockwise, Counterclockwise, None };

record Instruction(int NumberOfSteps, Turn TurnAfterwards);

class TristateTile : Tile
{
    public bool IsNonExistant { get; }

    public override char CharRepresentation => this.IsNonExistant ? ' ' : this.IsTraversable ? '.' : '#';

    public TristateTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, c == '.', fillTraversibleNeighboursFunc)
    {
        this.IsNonExistant = c == ' ';
    }
}

record CubeSide(int Id, Point TopLeft, Point BottomRight)
{
    public bool ContainsPoint(Point p)
    {
        return p.X >= this.TopLeft.X && p.X <= this.BottomRight.X &&
            p.Y >= this.TopLeft.Y && p.Y <= BottomRight.Y;
    }

    public (int x, int y) GetRelativePositionWithinCube(Point p)
    {
        if (!this.ContainsPoint(p))
            throw new Exception();

        return (p.X - TopLeft.X, p.Y - TopLeft.Y);
    }
}