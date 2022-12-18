using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

var valveRecords  = new InputProvider<ValveRecord?>("Input.txt", GetValveRecord).Where(w => w != null).Cast<ValveRecord>().ToList();

var valveFactory = new UniqueFactory<string, Valve>(w => new Valve(w));

var valves = valveRecords.Select(w => w.Build(valveFactory)).ToHashSet();

State2.nonZeroValves = valves.Where(w => w.FlowRate > 0).Count();
State2.cachedPathfinder = new CachedPathfinder<Valve>();

var startValve = valveFactory.GetOrCreateInstance("AA");

Dictionary<string, int> memoizationDict = new();

//var initialState = new State1(0, startValve, 0, new HashSet<Valve>(), null, "Initial State");
var initialState = new State2(0, startValve, startValve, 0, new HashSet<Valve>(), valves.Where(w => w.FlowRate > 0).ToHashSet(), null, "Initial State", new List<Valve>(), new List<Valve>());

var openSet = new PriorityQueue<State2, int>();
openSet.Enqueue(initialState, 0);

int maxMinutes = 26;

State2? currentBestState = null;

HashSet<string> visitedSates = new();

while (openSet.Count > 0)
{
    var current = openSet.Dequeue();

    if (current.Minute > maxMinutes)
        continue;

    if (current.Minute == maxMinutes)
    {
        if (currentBestState == null || current.CommulativeFlow > currentBestState.CommulativeFlow)
        {
            //current.PrintHistory(Console.WriteLine);
            Console.WriteLine($"{DateTime.Now.TimeOfDay}: Found new best: {current.CommulativeFlow} Open nodes: {openSet.Count}");
            currentBestState = current;
        }
        continue;
    }

    foreach (var followingState in current.GetFollowingStates())
    {
        if (visitedSates.Contains(followingState.ToString()))
            continue;

        visitedSates.Add(followingState.ToString());

        openSet.Enqueue(followingState, -followingState.CommulativeFlow);
    }
}

if (currentBestState == null) throw new Exception();


Console.WriteLine();
Console.WriteLine();
currentBestState.PrintHistory(Console.WriteLine);

Console.WriteLine();
Console.WriteLine();
Console.WriteLine($"Result: {currentBestState.CommulativeFlow}");



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

class CachedPathfinder<T>
    where T : class, INode, IEquatable<T>
{
    private readonly Dictionary<(T, T), List<T>> memcache = new();

    public List<T> FindPath(T start, T goal, Func<T, int> GetHeuristicCost, Func<T, IEnumerable<T>> GetNeighbours)
    {
        var key = (start, goal);

        if (!memcache.ContainsKey(key))
        {
            var path = AStarPathfinder.FindPath(start, goal, GetHeuristicCost, GetNeighbours);

            if (path == null)
                throw new Exception();

            memcache[key] = path;
        }

        return memcache[key];
    }
}