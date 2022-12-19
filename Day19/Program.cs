using System.Text.RegularExpressions;

var blueprints = new InputProvider<Blueprint?>("Input.txt", GetBlueprint).Where(w => w != null).Cast<Blueprint>().ToList();

List<State> bestStatesPerBlueprint = new();

foreach (var blueprint in blueprints)
{
    var initialState = new State(0, 0, 0, 0, 0, 1, 0, 0, 0, true, true, true, true, blueprint);

    var bestState = SimulateFactory(initialState, 24);

    bestStatesPerBlueprint.Add(bestState);
}

Console.WriteLine($"Part 1: {bestStatesPerBlueprint.Sum(w => w.GetQuality())}");

var firstThree = blueprints.Take(3).ToList();
bestStatesPerBlueprint.Clear();

foreach (var blueprint in firstThree)
{
    var initialState = new State(0, 0, 0, 0, 0, 1, 0, 0, 0, true, true, true, true, blueprint);

    var bestState = SimulateFactory(initialState, 32);

    bestStatesPerBlueprint.Add(bestState);
}

Console.WriteLine($"Part 2: {bestStatesPerBlueprint.Select(w => w.NumberOfGeods).Aggregate((w1, w2) => w1 * w2)}");

static State SimulateFactory(State initialState, int maxMinutes)
{
    var openSet = new PriorityQueue<State, int>();
    openSet.Enqueue(initialState, 0);

    State? currentBestState = null;

    HashSet<string> visitedSates = new();

    int maxCostOre = new[] { initialState.Costs.OreRobotCostOre, initialState.Costs.ClayRobotCostOre, initialState.Costs.ObsidianRobotCostOre, initialState.Costs.GeodeRobotCostOre }.Max();
    int maxCostClay = new[] { initialState.Costs.ObsidianRobotCostClay }.Max();
    int maxCostObsidian = new[] { initialState.Costs.GeodeRobotCostObsidian }.Max();

    while (openSet.Count > 0)
    {
        var current = openSet.Dequeue();

        if (current.Minute > maxMinutes)
            continue;

        if (current.Minute == maxMinutes)
        {
            if (currentBestState == null || current.NumberOfGeods > currentBestState.NumberOfGeods)
            {
                //current.PrintHistory(Console.WriteLine);
                //Console.WriteLine($"{DateTime.Now.TimeOfDay}: Found new best for {current.Costs.Id}: {current.NumberOfGeods} Open nodes: {openSet.Count}");
                currentBestState = current;
            }
            continue;
        }

        foreach (var followingState in current.GetFollowingStates())
        {
            if (followingState.NumberOfOreRobots > maxCostOre + 1)
                continue;

            if (followingState.NumberOfOreRobots > maxCostOre + 1 && 
                followingState.NumberOfClayRobots > maxCostClay + 1)
                continue;

            if (followingState.NumberOfOreRobots > maxCostOre + 1 && 
                followingState.NumberOfObsidianRobots > maxCostObsidian + 1)
                continue;

            if (currentBestState != null &&
                followingState.NumberOfGeods + (Enumerable.Range(1, maxMinutes - followingState.Minute).Sum(w => followingState.NumberOfGeodRobots + w)) < currentBestState.NumberOfGeods)
                continue;

            var allowedMatrix = new[] { followingState.AllowedToBuildOreRobot, followingState.AllowedToBuildClayRobot, followingState.AllowedToBuildObsidianRobot, followingState.AllowedToBuildGeodeRobot };
            if (allowedMatrix.All(w => !w))
                continue;

            openSet.Enqueue(followingState, -followingState.GetSum());
        }
    }

    if (currentBestState == null) throw new Exception();

    return currentBestState;

}

static bool GetBlueprint(string? input, out Blueprint? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 7) throw new Exception();

    value = new Blueprint(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5], numbers[6]);

    return true;
}

record State(int Minute, 
    int NumberOfOre, int NumberOfClay, int NumberOfObsidian, int NumberOfGeods,
    int NumberOfOreRobots, int NumberOfClayRobots, int NumberOfObsidianRobots, int NumberOfGeodRobots,
    bool AllowedToBuildOreRobot, bool AllowedToBuildClayRobot, bool AllowedToBuildObsidianRobot, bool AllowedToBuildGeodeRobot,
    Blueprint Costs)
{
    public IEnumerable<State> GetFollowingStates()
    {
        int numberOfOre = this.NumberOfOre + this.NumberOfOreRobots;
        int numberOfClay = this.NumberOfClay + this.NumberOfClayRobots;
        int numberOfObsidian = this.NumberOfObsidian + this.NumberOfObsidianRobots;
        int numberOfGeods = this.NumberOfGeods + this.NumberOfGeodRobots;

        if (this.AllowedToBuildOreRobot && this.CanAffordToBuildOreRobot())
        {
            yield return new State(this.Minute + 1,
                numberOfOre - this.Costs.OreRobotCostOre,
                numberOfClay, numberOfObsidian, numberOfGeods,
                this.NumberOfOreRobots + 1,
                this.NumberOfClayRobots, this.NumberOfObsidianRobots, this.NumberOfGeodRobots,
                true, true, true, true,
                this.Costs);
        }

        if (this.AllowedToBuildClayRobot && this.CanAffordToBuildClayRobot())
        {
            yield return new State(this.Minute + 1,
                numberOfOre - this.Costs.ClayRobotCostOre,
                numberOfClay, numberOfObsidian, numberOfGeods,
                this.NumberOfOreRobots,
                this.NumberOfClayRobots + 1,
                this.NumberOfObsidianRobots, this.NumberOfGeodRobots,
                true, true, true, true,
                this.Costs);
        }

        if (this.AllowedToBuildObsidianRobot && this.CanAffordToBuildObsidianRobot())
        {
            yield return new State(this.Minute + 1,
                numberOfOre - this.Costs.ObsidianRobotCostOre,
                numberOfClay - this.Costs.ObsidianRobotCostClay,
                numberOfObsidian, numberOfGeods,
                this.NumberOfOreRobots, this.NumberOfClayRobots,
                this.NumberOfObsidianRobots + 1,
                this.NumberOfGeodRobots,
                true, true, true, true,
                this.Costs);
        }

        if (this.AllowedToBuildGeodeRobot && this.CanAffordToBuildGeodeRobot())
        {
            yield return new State(this.Minute + 1,
                numberOfOre - this.Costs.GeodeRobotCostOre,
                numberOfClay,
                numberOfObsidian - this.Costs.GeodeRobotCostObsidian,
                numberOfGeods,
                this.NumberOfOreRobots, this.NumberOfClayRobots, this.NumberOfObsidianRobots,
                this.NumberOfGeodRobots + 1,
                true, true, true, true,
                this.Costs);
        }

        yield return new State(this.Minute + 1,
            numberOfOre,
            numberOfClay,
            numberOfObsidian,
            numberOfGeods,
            this.NumberOfOreRobots, this.NumberOfClayRobots, this.NumberOfObsidianRobots, this.NumberOfGeodRobots,
            !this.CanAffordToBuildOreRobot(), !this.CanAffordToBuildClayRobot(), !this.CanAffordToBuildObsidianRobot(), !this.CanAffordToBuildGeodeRobot(),
            this.Costs);
    }

    public int GetQuality()
    {
        return this.Costs.Id * this.NumberOfGeods;
    }

    public int GetSum()
    {
        var lst = GetAllStats();
        return lst.Sum();
    }

    public override string ToString()
    {
        return string.Join(",", GetAllStats());
    }

    private IEnumerable<int> GetAllStats()
    {
        return new[] { Minute, NumberOfOre, NumberOfClay, NumberOfObsidian, NumberOfGeods,
                          NumberOfOreRobots, NumberOfClayRobots, NumberOfObsidianRobots, NumberOfGeodRobots,
                          Costs.Id };
    }

    private bool CanAffordToBuildOreRobot()
    {
        return this.NumberOfOre >= this.Costs.OreRobotCostOre;
    }

    private bool CanAffordToBuildClayRobot()
    {
        return this.NumberOfOre >= this.Costs.ClayRobotCostOre;
    }

    private bool CanAffordToBuildObsidianRobot()
    {
        return this.NumberOfOre >= this.Costs.ObsidianRobotCostOre &&
            this.NumberOfClay >= this.Costs.ObsidianRobotCostClay;
    }

    private bool CanAffordToBuildGeodeRobot()
    {
        return this.NumberOfOre >= this.Costs.GeodeRobotCostOre &&
            this.NumberOfObsidian >= this.Costs.GeodeRobotCostObsidian;
    }
}

record Blueprint(
    int Id, 
    int OreRobotCostOre, 
    int ClayRobotCostOre, 
    int ObsidianRobotCostOre,
    int ObsidianRobotCostClay,
    int GeodeRobotCostOre,
    int GeodeRobotCostObsidian);