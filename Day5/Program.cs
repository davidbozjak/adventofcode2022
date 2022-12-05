using System.Text.RegularExpressions;

var moveInstructions = new InputProvider<MoveInstruction?>("Input_Moves.txt", GetMoveInstruction).Where(w => w != null).Cast<MoveInstruction>().ToList();

var initialState = new StringInputProvider("Input_Crates.txt").ToArray();

var crateMover = new CrateMover9000(initialState);
crateMover.ApplyMoveInstructions(moveInstructions);

Console.WriteLine($"Part 1: {crateMover.GetTopRow()}");

var advancedCrateMover = new CrateMover9001(initialState);
advancedCrateMover.ApplyMoveInstructions(moveInstructions);

Console.WriteLine($"Part 2: {advancedCrateMover.GetTopRow()}");

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