abstract record CaveState(int Minute, int CommulativeFlow)
    : SearchStateState(CommulativeFlow, -CommulativeFlow);

record State1(int Minute, Valve CurrentLocation, int CommulativeFlow, HashSet<Valve> OpenValves, State1? PreviousState, string TransitionAction)
    : CaveState(Minute, CommulativeFlow)
{
    public static int nonZeroValves;

    public override string GetStringHash()
    {
        return this.ToString();
    }

    public override IEnumerable<State1> GetSubsequentStates()
    {
        foreach (var newState in NewState_JustStandInPlace())
            yield return newState;

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

    private IEnumerable<State1> NewState_JustStandInPlace()
    {
        if (this.OpenValves.Count < nonZeroValves)
            yield break;

        yield return new State1(this.Minute + 1, this.CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, "Stand in place");
    }

    private IEnumerable<State1> NewState_MoveToNeighbour()
    {
        if (this.OpenValves.Count >= nonZeroValves)
            yield break;

        foreach (var neighbour in this.CurrentLocation.ConnectedValves)
        {
            //don't go to neighbour if it is open except if it has an unopened neighbour
            if (this.OpenValves.Contains(neighbour))
            {
                if (neighbour.ConnectedValves.All(w => w.FlowRate == 0 || this.OpenValves.Contains(w)))
                    continue;
            }

            yield return new State1(this.Minute + 1, neighbour, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, $"Move to {neighbour.Name}");
        }
    }

    private IEnumerable<State1> NewState_OpenThisValve()
    {
        if (this.CurrentLocation.FlowRate <= 0)
            yield break;

        if (this.OpenValves.Contains(this.CurrentLocation))
            yield break;

        var newOpenValves = this.OpenValves.ToHashSet();
        newOpenValves.Add(this.CurrentLocation);

        yield return new State1(this.Minute + 1, CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Open Valve {this.CurrentLocation.Name}");
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
