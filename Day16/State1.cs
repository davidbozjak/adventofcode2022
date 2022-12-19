record State1(int Minute, Valve CurrentLocation, int CommulativeFlow, HashSet<Valve> OpenValves, HashSet<Valve> ValvesToOpen, State1? PreviousState, string TransitionAction, List<Valve> PathToTake)
    : CaveState(Minute, CommulativeFlow, OpenValves, ValvesToOpen)
{
    public override string GetStringHash()
    {
        return this.ToString();
    }

    public override IEnumerable<State1> GetSubsequentStates()
    {
        if (this.OpenValves.Count == nonZeroValves)
        {
            // nothing to do so just stand here and tick rounds
            yield return new State1(this.Minute + 1, this.CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this.ValvesToOpen, this, "Both stand in place - no nodes", this.PathToTake);
        }
        else
        {
            if (PathToTake.Count > 1)
            {
                if (PathToTake[0] != this.CurrentLocation)
                    throw new Exception();

                var newPath = PathToTake.Skip(1).ToList();

                yield return new State1(this.Minute + 1, newPath[0],
                        this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this.ValvesToOpen, this,
                        $"Continues to {this.PathToTake[1].Name}, on the path to {this.PathToTake.Last().Name}",
                        newPath);
            }
            else
            {
                bool hasOpenedValve = false;

                var newOpenValves = this.OpenValves.ToHashSet();

                if (this.CurrentLocation.FlowRate > 0)
                {
                    newOpenValves.Add(this.CurrentLocation);
                    hasOpenedValve = true;
                }

                var paths = GetPathsToAllValvesLeftToOpen(this.CurrentLocation);

                if (!paths.Any())
                {
                    // occurs in cases when we just opened the last valve. We just stay in place.
                    paths = new List<List<Valve>>() { new List<Valve>() { this.CurrentLocation } };
                }

                foreach (var path in paths)
                {
                    var newPath = path.Skip(hasOpenedValve ? 0 : 1).ToList();
                    var destination = path.Last();

                    var newValvesToOpen = this.ValvesToOpen.ToHashSet();
                    newValvesToOpen.Remove(destination);

                    yield return new State1(this.Minute + 1, newPath[0],
                        this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, newValvesToOpen, this,
                        $"Reached {this.CurrentLocation.Name} and {(hasOpenedValve ? "has" : "has NOT")} opened the valve. Next possible destinations: {string.Join(", ", paths.Select(w => w.Last().Name))}",
                        newPath);
                }
            }
        }
    }

    public void PrintHistory(Action<string> printAction)
    {
        this.PreviousState?.PrintHistory(printAction);

        printAction(this.ToString());
    }

    //private IEnumerable<State1> NewState_JustStandInPlace()
    //{
    //    if (this.OpenValves.Count < nonZeroValves)
    //        yield break;

    //    yield return new State1(this.Minute + 1, this.CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, "Stand in place");
    //}

    //private IEnumerable<State1> NewState_MoveToNeighbour()
    //{
    //    if (this.OpenValves.Count >= nonZeroValves)
    //        yield break;

    //    foreach (var neighbour in this.CurrentLocation.ConnectedValves)
    //    {
    //        //don't go to neighbour if it is open except if it has an unopened neighbour
    //        if (this.OpenValves.Contains(neighbour))
    //        {
    //            if (neighbour.ConnectedValves.All(w => w.FlowRate == 0 || this.OpenValves.Contains(w)))
    //                continue;
    //        }

    //        yield return new State1(this.Minute + 1, neighbour, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, $"Move to {neighbour.Name}");
    //    }
    //}

    //private IEnumerable<State1> NewState_OpenThisValve()
    //{
    //    if (this.CurrentLocation.FlowRate <= 0)
    //        yield break;

    //    if (this.OpenValves.Contains(this.CurrentLocation))
    //        yield break;

    //    var newOpenValves = this.OpenValves.ToHashSet();
    //    newOpenValves.Add(this.CurrentLocation);

    //    yield return new State1(this.Minute + 1, CurrentLocation, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Open Valve {this.CurrentLocation.Name}");
    //}

    public override string ToString()
    {
        return $"[Minute{this.Minute}][{this.CurrentLocation.Name}]->[{this.PathToTake.Last().Name}][{this.CommulativeFlow}][{string.Join(", ", this.OpenValves.Select(w => w.Name).OrderBy(w => w))}][{GetFlowDuringThisState()}]";
    }
}
