abstract class Crane
{
    protected readonly List<Stack<Crate>> stacks;

    public Crane(string[] initialState)
    {
        stacks = new List<Stack<Crate>>();

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
    }

    public string GetTopRow()
    {
        var row = new List<char>();
        for (int i = 0; i < stacks.Count; i++)
        {
            row.Add(stacks[i].Peek().Name);
        }
        return new string(row.ToArray());
    }

    public void ApplyMoveInstructions(IEnumerable<MoveInstruction> moveInstructions)
    {
        foreach (var instruction in moveInstructions)
        {
            MoveCrate(instruction);
        }
    }

    protected abstract void MoveCrate(MoveInstruction instruction);
}

class CrateMover9000 : Crane
{
    public CrateMover9000(string[] initialState)
        : base(initialState)
    { }

    protected override void MoveCrate(MoveInstruction instruction)
    {
        for (int i = 0; i < instruction.NumberOfCrates; i++)
        {
            var crate = stacks[instruction.FromStackId - 1].Pop();
            stacks[instruction.ToStackId - 1].Push(crate);
        }
    }
}

class CrateMover9001 : Crane
{
    public CrateMover9001(string[] initialState)
        : base(initialState)
    { }

    protected override void MoveCrate(MoveInstruction instruction)
    {
        var movingTogether = new List<Crate>();
        for (int i = 0; i < instruction.NumberOfCrates; i++)
        {
            var crate = stacks[instruction.FromStackId - 1].Pop();
            movingTogether.Add(crate);
        }

        movingTogether.Reverse();

        foreach (var crate in movingTogether)
        {
            stacks[instruction.ToStackId - 1].Push(crate);
        }
    }
}