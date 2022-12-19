using System.Text.RegularExpressions;

var valveRecords  = new InputProvider<ValveRecord?>("Input.txt", GetValveRecord).Where(w => w != null).Cast<ValveRecord>().ToList();

var valveFactory = new UniqueFactory<string, Valve>(w => new Valve(w));

var valves = valveRecords.Select(w => w.Build(valveFactory)).ToHashSet();

CaveState.nonZeroValves = valves.Where(w => w.FlowRate > 0).Count();
CaveState.cachedPathfinder = new CachedPathfinder<Valve>();
int maxFlow = valves.Sum(w => w.FlowRate);

var startValve = valveFactory.GetOrCreateInstance("AA");

int maxMinutes = 30;
var initialStatePart1 = new State1(0, startValve, 0, new HashSet<Valve>(), valves.Where(w => w.FlowRate > 0).ToHashSet(), null, "Initial State", new List<Valve>());

var part1Searcher = new PriorityQueueSpaceSearcher<State1>() { EnableTracing = true, DiscardVisited = true };

var bestStatePart1 = part1Searcher.FindHighestScore(initialStatePart1,
    state => state.Minute == maxMinutes,
    (state, currentBestState) => ShouldDiscardState(state, currentBestState));

Console.WriteLine($"Part 1: {bestStatePart1.CommulativeFlow}");

maxMinutes = 26;

var initialStatePart2 = new State2(0, startValve, startValve, 0, new HashSet<Valve>(), valves.Where(w => w.FlowRate > 0).ToHashSet(), null, "Initial State", new List<Valve>(), new List<Valve>());

var part2Searcher = new PriorityQueueSpaceSearcher<State2>() { EnableTracing = true, DiscardVisited = true };

var bestStatePart2 = part2Searcher.FindHighestScore(initialStatePart2,
    state => state.Minute == maxMinutes,
    ShouldDiscardState);

Console.WriteLine($"Part 2: {bestStatePart2.CommulativeFlow}");

bool ShouldDiscardState(CaveState state, CaveState? currentBestState)
{
    if (currentBestState != null)
    {
        int minutesLeft = maxMinutes - state.Minute;

        if (currentBestState.CommulativeFlow > state.CommulativeFlow + (maxFlow * minutesLeft))
            return true;
    }

    return false;
}

static bool GetValveRecord(string? input, out ValveRecord? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var number = int.Parse(numRegex.Match(input).Value);

    Regex valveNameRegex = new(@"[A-Z][A-Z]");

    var valveNames = valveNameRegex.Matches(input).Select(w => w.Value).ToArray();

    string name = valveNames[0];

    value = new ValveRecord(name, number, valveNames[1..]);

    return true;
}

abstract record CaveState(int Minute, int CommulativeFlow, HashSet<Valve> OpenValves, HashSet<Valve> ValvesToOpen)
    : SearchStateState(CommulativeFlow, -CommulativeFlow)
{
    public static int nonZeroValves;
    public static CachedPathfinder<Valve> cachedPathfinder;

    protected IEnumerable<List<Valve>> GetPathsToAllValvesLeftToOpen(Valve startLocation)
    {
        foreach (var endLocation in this.ValvesToOpen)
        {
            yield return cachedPathfinder.FindPath(startLocation, endLocation, _ => 0, w => w.ConnectedValves);
        }
    }

    protected int GetFlowDuringThisState()
    {
        return this.OpenValves.Sum(w => w.FlowRate);
    }
}

record ValveRecord(string Name, int FlowRate, IEnumerable<string> ReachableValves)
{
    public Valve Build(UniqueFactory<string, Valve> factory)
    {
        var valve = factory.GetOrCreateInstance(this.Name);

        valve.FlowRate = this.FlowRate;

        valve.AddConnections(this.ReachableValves.Select(w => factory.GetOrCreateInstance(w)));

        return valve;
    }
}

[System.Diagnostics.DebuggerDisplay("{Name}")]
class Valve : INode, IEquatable<Valve>
{
    public string Name { get; }

    public int FlowRate { get; set; }

    private readonly List<Valve> connectedValves = new();

    public Valve(string name)
    {
        this.Name = name;
    }

    public void AddConnections(IEnumerable<Valve> connectedValves)
    {
        this.connectedValves.AddRange(connectedValves);
    }

    public bool Equals(Valve? other)
    {
        return base.Equals(other);
    }

    public IEnumerable<Valve> ConnectedValves => this.connectedValves;

    public int Cost => 1;
}