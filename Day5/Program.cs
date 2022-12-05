using System.Text.RegularExpressions;

var moveInstructions = new InputProvider<MoveInstruction?>("Input_Moves.txt", GetMoveInstruction).Where(w => w != null).Cast<MoveInstruction>().ToList();

var initialState = new StringInputProvider("Input_Crates.txt").ToArray();

var stacks = GetStacksFromInitialState(initialState);

foreach (var move in moveInstructions)
{
    for (int i = 0; i < move.NumberOfCrates; i++)
    {
        var crate = stacks[move.FromStackId - 1].Pop();
        stacks[move.ToStackId - 1].Push(crate);
    }
}

Console.WriteLine($"Part 1: {GetTopRow(stacks)}");

stacks = GetStacksFromInitialState(initialState);

foreach (var move in moveInstructions)
{
    var movingTogether = new List<Crate>();
    for (int i = 0; i < move.NumberOfCrates; i++)
    {
        var crate = stacks[move.FromStackId - 1].Pop();
        movingTogether.Add(crate);
    }

    movingTogether.Reverse();

    foreach (var crate in movingTogether)
    {
        stacks[move.ToStackId - 1].Push(crate);
    }
}

Console.WriteLine($"Part 2: {GetTopRow(stacks)}");

static string GetTopRow(List<Stack<Crate>> stacks)
{
    var row = new List<char>();
    for (int i = 0; i < stacks.Count; i++)
    {
        row.Add(stacks[i].Peek().Name);
    }
    return new string(row.ToArray());
}

static List<Stack<Crate>> GetStacksFromInitialState(string[] initialState)
{
    var stacks = new List<Stack<Crate>>();

    var numberOfStacks = int.Parse(initialState[^1][^2..^1]);

    stacks.AddRange(Enumerable.Range(0, numberOfStacks).Select(w => new Stack<Crate>()));

    for (int rowIndex = initialState.Length - 2; rowIndex >= 0; rowIndex--)
    {
        var line = initialState[rowIndex];

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '[')
            {
                int stackIndex = (i + 1) / 4;
                var name = line[++i];
                var crate = new Crate(name);
                stacks[stackIndex].Push(crate);
            }
        }
    }

    return stacks;
}

static bool GetMoveInstruction(string? input, out MoveInstruction? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    value = new MoveInstruction(numbers[0], numbers[1], numbers[2]);

    return true;
}

record MoveInstruction(int NumberOfCrates, int FromStackId, int ToStackId);

record Crate(char Name);