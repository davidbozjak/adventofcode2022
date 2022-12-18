using System.Text;

record State2(int Minute, Valve CurrentLocation1, Valve CurrentLocation2, int CommulativeFlow, HashSet<Valve> OpenValves, HashSet<Valve> ValvesToOpen, State2? PreviousState, string TransitionAction, List<Valve> Player1Path, List<Valve> Player2Path)
{
    public static int nonZeroValves;
    public static CachedPathfinder<Valve> cachedPathfinder;

    public IEnumerable<State2> GetFollowingStates()
    {
        if (this.OpenValves.Count == nonZeroValves)
        {
            // nothing to do so just stand here and tick rounds
            yield return new State2(this.Minute + 1, this.CurrentLocation1, this.CurrentLocation2, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this.ValvesToOpen, this, "Both stand in place - no nodes", Player1Path, Player2Path);
        }
        else
        {
            StringBuilder actionStringBuilder = new();

            List<List<Valve>> player1NextPath = new();
            List<List<Valve>> player2NextPath = new();

            var newOpenValves = this.OpenValves.ToHashSet();

            if (Player1Path.Count > 1)
            {
                if (Player1Path[0] != this.CurrentLocation1)
                    throw new Exception();

                player1NextPath.Add(Player1Path.Skip(1).ToList());

                actionStringBuilder.AppendLine($"Player 1 continues to {Player1Path[1].Name}, on the path to {Player1Path.Last().Name}");
            }
            else
            {
                bool hasOpenedValve = false;

                if (this.CurrentLocation1.FlowRate > 0)
                {
                    newOpenValves.Add(this.CurrentLocation1);
                    hasOpenedValve = true;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }

                var paths = GetPathsToAllValvesLeftToOpen(this.CurrentLocation1);

                if (!hasOpenedValve)
                {
                    paths = paths.Select(w => w.Skip(1).ToList());
                }

                player1NextPath.AddRange(paths);

                actionStringBuilder.AppendLine($"Player 1 reached {this.CurrentLocation1.Name} and {(hasOpenedValve ? "has" : "has NOT")} opened the valve. Next possible destinations: {string.Join(", ", paths.Select(w => w.Last().Name))}");
            }

            if (Player2Path.Count > 1)
            {
                if (Player2Path[0] != this.CurrentLocation2)
                    throw new Exception();

                player2NextPath.Add(Player2Path.Skip(1).ToList());

                actionStringBuilder.AppendLine($"Player 2 continues to {Player2Path[1].Name}, on the path to {Player2Path.Last().Name}");
            }
            else
            {
                bool hasOpenedValve = false;

                if (this.CurrentLocation2.FlowRate > 0)
                {
                    newOpenValves.Add(this.CurrentLocation2);
                    hasOpenedValve = true;
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }

                var paths = GetPathsToAllValvesLeftToOpen(this.CurrentLocation2);

                if (!hasOpenedValve)
                {
                    paths = paths.Select(w => w.Skip(1).ToList());
                }

                player2NextPath.AddRange(paths);
                actionStringBuilder.AppendLine($"Player 2 reached {this.CurrentLocation2.Name} and {(hasOpenedValve ? "has" : "has NOT")} opened the valve. Next possible destinations: {string.Join(", ", paths.Select(w => w.Last().Name))}");
            }

            if (player1NextPath.Count == 0)
            {
                player1NextPath.Add(new List<Valve>() { this.CurrentLocation1 });
                actionStringBuilder.AppendLine($"Player 1 has no more destinations, staying in palce at {this.CurrentLocation1.Name}");
            }

            if (player2NextPath.Count == 0)
            {
                player2NextPath.Add(new List<Valve>() { this.CurrentLocation2 });
                actionStringBuilder.AppendLine($"Player 2 has no more destinations, staying in palce at {this.CurrentLocation2.Name}");
            }

            foreach (var path1 in player1NextPath)
            {
                var destination1 = path1.Last();

                foreach (var path2 in player2NextPath)
                {
                    var destination2 = path2.Last();

                    var newValvesToOpen = this.ValvesToOpen.ToHashSet();
                    newValvesToOpen.Remove(destination1);
                    newValvesToOpen.Remove(destination2);

                    yield return new State2(this.Minute + 1, path1[0], path2[0],
                        this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, newValvesToOpen, this,
                        actionStringBuilder.ToString(), 
                        path1, path2);
                }
            }
        }
    }

    public void PrintHistory(Action<string> printAction)
    {
        this.PreviousState?.PrintHistory(printAction);

        printAction($"== Minute {this.Minute} Player1 at {this.CurrentLocation1.Name} Player2 at {this.CurrentLocation2.Name} Commulative {this.CommulativeFlow} =={Environment.NewLine}Valves {string.Join(", ", this.OpenValves.Select(w => w.Name).OrderBy(w => w))} are open, releasing {GetFlowDuringThisState()} pressure.{Environment.NewLine}{this.TransitionAction}");
    }

    private IEnumerable<List<Valve>> GetPathsToAllValvesLeftToOpen(Valve startLocation)
    {
        foreach (var endLocation in this.ValvesToOpen)
        {
            yield return cachedPathfinder.FindPath(startLocation, endLocation, _ => 0, w => w.ConnectedValves);
        }
    }

    private int GetFlowDuringThisState()
    {
        return this.OpenValves.Sum(w => w.FlowRate);
    }

    public override string ToString()
    {
        return $"== Minute {this.Minute} Player1 at {this.CurrentLocation1.Name} on the way to {this.Player1Path.Last().Name} Player2 at {this.CurrentLocation2.Name} on the way to {this.Player2Path.Last().Name}. Commulative {this.CommulativeFlow} Open Valves {string.Join(", ", this.OpenValves.Select(w => w.Name).OrderBy(w => w))}, releasing {GetFlowDuringThisState()}";
    }
}
