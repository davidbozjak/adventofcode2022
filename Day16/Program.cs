using System.Text.RegularExpressions;

var valveRecords  = new InputProvider<ValveRecord?>("Input.txt", GetValveRecord).Where(w => w != null).Cast<ValveRecord>().ToList();

var valveFactory = new UniqueFactory<string, Valve>(w => new Valve(w));

var valves = valveRecords.Select(w => w.Build(valveFactory)).ToHashSet();

State2.nonZeroValves = valves.Where(w => w.FlowRate > 0).Count();

var startValve = valveFactory.GetOrCreateInstance("AA");

Dictionary<string, int> memoizationDict = new();

//var initialState = new State1(0, startValve, 0, new HashSet<Valve>(), null, "Initial State");
var initialState = new State2(0, startValve, startValve, 0, new HashSet<Valve>(), null, "Initial State", new List<Valve>(), new List<Valve>());

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
            Console.WriteLine($"{DateTime.Now.TimeOfDay}: Found new best: {current.CommulativeFlow}");
            currentBestState = current;
        }
        continue;
    }

    foreach (var followingState in current.GetFollowingStates())
    {
        if (visitedSates.Contains(followingState.ToString()))
            continue;

        visitedSates.Add(current.ToString());

        openSet.Enqueue(followingState, followingState.Minute - followingState.OpenValves.Count - followingState.CommulativeFlow);
    }
}

if (currentBestState == null) throw new Exception();

Console.WriteLine($"Part 1: {currentBestState.CommulativeFlow}");

currentBestState.PrintHistory(Console.WriteLine);

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

class Valve
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

    public IEnumerable<Valve> ConnectedValves => this.connectedValves;
}