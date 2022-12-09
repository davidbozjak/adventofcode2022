using System.Drawing;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction).Where(w => w != null).Cast<Instruction>().ToList();

(int x, int y) headPosition = (0, 0);
(int x, int y) tailPosition = (0, 0);

HashSet<(int x, int y)> tailPositions = new();

foreach (var instruction in instructions)
{
    for (int step = 0; step < instruction.NumberOfSteps; step++)
    {
        headPosition = instruction.Direction switch
        {
            Direction.Up => (headPosition.x, headPosition.y - 1),
            Direction.Down => (headPosition.x, headPosition.y + 1),
            Direction.Left => (headPosition.x - 1, headPosition.y),
            Direction.Right => (headPosition.x + 1, headPosition.y),
            _ => throw new Exception()
        };

        tailPosition = MoveTailAfterHead(headPosition, tailPosition);
        tailPositions.Add(tailPosition);
    }
}

Console.WriteLine($"Part 1: {tailPositions.Count}");

tailPositions = new();
List<(int x, int y)> rope = Enumerable.Range(0, 10).Select(w => (0, 0)).ToList();

foreach (var instruction in instructions)
{
    for (int step = 0; step < instruction.NumberOfSteps; step++)
    {
        rope[0] = instruction.Direction switch
        {
            Direction.Up => (rope[0].x, rope[0].y - 1),
            Direction.Down => (rope[0].x, rope[0].y + 1),
            Direction.Left => (rope[0].x - 1, rope[0].y),
            Direction.Right => (rope[0].x + 1, rope[0].y),
            _ => throw new Exception()
        };

        for (int i = 1; i < rope.Count; i++)
        {
            rope[i] = MoveTailAfterHead(rope[i - 1], rope[i]);
        }

        tailPositions.Add(rope.Last());
    }
}


Console.WriteLine($"Part 2: {tailPositions.Count}");

static (int x, int y) MoveTailAfterHead((int x, int y) headPosition, (int x, int y) tailPosition)
{
    int distance = Math.Abs(headPosition.x - tailPosition.x) + Math.Abs(headPosition.y - tailPosition.y);

    if (distance <= 1) return tailPosition;

    var xDiff = headPosition.x - tailPosition.x;
    var yDiff = headPosition.y - tailPosition.y;

    if (xDiff == 0)
    {
        return (tailPosition.x, tailPosition.y + (yDiff > 0 ? 1 : -1));
    }
    else if (yDiff == 0)
    {
        return (tailPosition.x + (xDiff > 0 ? 1 : -1), tailPosition.y);
    }
    else if (distance > 2)
    {
        return (tailPosition.x + (xDiff > 0 ? 1 : -1), tailPosition.y + (yDiff > 0 ? 1 : -1));
    }
    else return (tailPosition);
}

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    int number = int.Parse(input[1..]);

    var direction = input[0] switch
    {
        'U' => Direction.Up,
        'D' => Direction.Down,
        'L' => Direction.Left,
        'R' => Direction.Right,
        _ => throw new Exception()
    };

    value = new Instruction(direction, number);

    return true;
}

enum Direction { Left, Right, Up, Down };
record Instruction(Direction Direction, int NumberOfSteps);