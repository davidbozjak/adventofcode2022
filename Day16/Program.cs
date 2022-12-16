using System.Text.RegularExpressions;

var valveRecords  = new InputProvider<ValveRecord?>("Input.txt", GetValveRecord).Where(w => w != null).Cast<ValveRecord>().ToList();

var valveFactory = new UniqueFactory<string, Valve>(w => new Valve(w));

var valves = valveRecords.Select(w => w.Build(valveFactory)).ToHashSet();

var startValve = valveFactory.GetOrCreateInstance("AA");

Dictionary<string, int> memoizationDict = new();

var initialState = new State(0, startValve, 0, new HashSet<Valve>(), null, "Initial State");

var openSet = new PriorityQueue<State, int>();
openSet.Enqueue(initialState, 0);

int maxMinutes = 30;

State? currentBestState = null;

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
            Console.WriteLine($"Found new best: {current.CommulativeFlow}");
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

record State(int Minute, Valve CurrentLocation, int CommulativeFlow, HashSet<Valve> OpenValves, State? PreviousState, string TransitionAction)
{
    public IEnumerable<State> GetFollowingStates()
    {
        yield return NewState_JustStandInPlace();

        foreach (var newState in NewState_MoveToNeighbour())
            yield return newState;

        foreach (var newState in NewState_OpenThisValve())
            yield return newState;
    }

    public void PrintHistory(Action<string> printAction)
    {
        if (this.PreviousState != null)
            this.PreviousState.PrintHistory(printAction);

        printAction(this.ToString());
    }

    private State NewState_JustStandInPlace()
    {
        return new State(this.Minute + 1, this.CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, "Stand in place");
    }

    private IEnumerable<State> NewState_MoveToNeighbour()
    {
        foreach (var neighbour in this.CurrentLocation.ConnectedValves)
        {
            //don't go to neighbour if it is open except if it has an unopened neighbour
            if (this.OpenValves.Contains(neighbour))
            {
                if (neighbour.ConnectedValves.All(w => w.FlowRate == 0 || this.OpenValves.Contains(w)))
                    continue;
            }

            yield return new State(this.Minute + 1, neighbour, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, $"Move to {neighbour.Name}");
        }
    }

    private IEnumerable<State> NewState_OpenThisValve()
    {
        if (this.CurrentLocation.FlowRate <= 0)
            yield break;

        if (this.OpenValves.Contains(this.CurrentLocation))
            yield break;

        var newOpenValves = this.OpenValves.ToHashSet();
        newOpenValves.Add(this.CurrentLocation);

        yield return new State(this.Minute + 1, CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Open Valve {this.CurrentLocation.Name}");
    }

    private int GetFlowDuringThisState()
    {
        return this.OpenValves.Sum(w => w.FlowRate);
    }

    public override string ToString()
    {
        return $"== Minute {this.Minute} at {this.CurrentLocation.Name} Commulative {this.CommulativeFlow} =={Environment.NewLine}Valves {string.Join(", ", this.OpenValves.Select(w => w.Name).OrderBy(w => w))} are open, releasing {GetFlowDuringThisState()} pressure.{Environment.NewLine}{this.TransitionAction}";
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