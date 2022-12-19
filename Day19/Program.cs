using System.Text.RegularExpressions;

var blueprints = new InputProvider<Blueprint?>("Input.txt", GetBlueprint).Where(w => w != null).Cast<Blueprint>().ToList();

var bestStatesPerBlueprint = ProcessBlueprints(24, blueprints);
Console.WriteLine($"Part 1: {bestStatesPerBlueprint.Sum(w => w.GetQuality())}");

bestStatesPerBlueprint = ProcessBlueprints(32, blueprints.Take(3));
Console.WriteLine($"Part 2: {bestStatesPerBlueprint.Select(w => w.NumberOfGeods).Aggregate((w1, w2) => w1 * w2)}");

static IEnumerable<State> ProcessBlueprints(int maxMinutes, IEnumerable<Blueprint> blueprints)
{
    var searcher = new PriorityQueueSpaceSearcher<State>() { DiscardVisited = false, EnableTracing = true };

    foreach (var blueprint in blueprints)
    {
        var initialState = new State(0, 0, 0, 0, 0, 1, 0, 0, 0, true, true, true, true, blueprint);

        int maxCostOre = new[] { blueprint.OreRobotCostOre, blueprint.ClayRobotCostOre, blueprint.ObsidianRobotCostOre, blueprint.GeodeRobotCostOre }.Max();
        int maxCostClay = new[] { blueprint.ObsidianRobotCostClay }.Max();
        int maxCostObsidian = new[] { blueprint.GeodeRobotCostObsidian }.Max();

        var bestState = searcher.FindHighestScore(initialState,
            state => state.Minute == maxMinutes,
            (state, currentBestState) => DiscardState(state, currentBestState, maxCostOre, maxCostClay, maxCostObsidian, maxMinutes));

        yield return bestState;
    }
}

static bool DiscardState(State state, State? currentBestState, int maxCostOre, int maxCostClay, int maxCostObsidian, int maxMinutes)
{
    if (currentBestState == null)
        return false;

    if (state.NumberOfOreRobots > maxCostOre + 1)
        return true;

    if (state.NumberOfOreRobots > maxCostOre + 1 &&
        state.NumberOfClayRobots > maxCostClay + 1)
        return true;

    if (state.NumberOfOreRobots > maxCostOre + 1 &&
        state.NumberOfObsidianRobots > maxCostObsidian + 1)
        return true;

    //if building one geode robot per minute won't get you over the current best, don't continue this branch
    if (currentBestState != null &&
        state.NumberOfGeods + 
            (Enumerable.Range(1, maxMinutes - state.Minute).Sum(w => state.NumberOfGeodRobots + w)) 
                < currentBestState.NumberOfGeods)
        return true;

    //if not allowed to build anything, don't continue this branch
    var allowedMatrix = new[] { state.AllowedToBuildOreRobot, state.AllowedToBuildClayRobot, state.AllowedToBuildObsidianRobot, state.AllowedToBuildGeodeRobot };
    if (allowedMatrix.All(w => !w))
        return true;

    return false;
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
    : SearchSpaceState(NumberOfGeods, 
        -(Minute + NumberOfOre + NumberOfClay + NumberOfObsidian + NumberOfGeods
            + NumberOfOreRobots + NumberOfClayRobots + NumberOfObsidianRobots + NumberOfGeodRobots))
{
    public override string GetStringHash()
    {
        return this.ToString();
    }

    public override IEnumerable<SearchSpaceState> GetSubsequentStates()
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