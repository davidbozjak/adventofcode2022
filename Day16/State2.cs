record State2(int Minute, Valve CurrentLocation1, Valve CurrentLocation2, int CommulativeFlow, HashSet<Valve> OpenValves, State2? PreviousState, string TransitionAction, List<Valve> Player1Path, List<Valve> Player2Path)
{
    public static int nonZeroValves;

    public IEnumerable<State2> GetFollowingStates()
    {
        if (this.OpenValves.Count >= nonZeroValves)
        {
            // nothing to do so just stand here and tick rounds
            yield return new State2(this.Minute + 1, this.CurrentLocation1, this.CurrentLocation2, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, "Both stand in place", Player1Path, Player2Path);
        }
        else
        {
            if (this.CurrentLocation1.FlowRate > 0 && !this.OpenValves.Contains(this.CurrentLocation1))
            {
                var newOpenValves = this.OpenValves.ToHashSet();
                newOpenValves.Add(this.CurrentLocation1);

                foreach (var player2Neighbour in CurrentLocation2.ConnectedValves)
                {
                    if (Player2Path.Count(w => w == player2Neighbour) > 2)
                        continue;

                    var newPath = Player2Path.ToList();
                    newPath.Add(player2Neighbour);

                    yield return new State2(this.Minute + 1, this.CurrentLocation1, player2Neighbour, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Player 1 Open valve {this.CurrentLocation1.Name}, Player 2 move to {player2Neighbour.Name}", Player1Path, newPath);
                }
            }

            if (this.CurrentLocation2.FlowRate > 0 && !this.OpenValves.Contains(this.CurrentLocation2))
            {
                var newOpenValves = this.OpenValves.ToHashSet();
                newOpenValves.Add(this.CurrentLocation2);

                foreach (var player1Neighbour in CurrentLocation1.ConnectedValves)
                {
                    if (Player1Path.Count(w => w == player1Neighbour) > 2)
                        continue;

                    var newPath = Player1Path.ToList();
                    newPath.Add(player1Neighbour);

                    yield return new State2(this.Minute + 1, player1Neighbour, this.CurrentLocation2, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Player 1 Move to {player1Neighbour.Name}, Player 2 Open valve {this.CurrentLocation2.Name}", newPath, Player2Path);
                }
            }

            if (this.CurrentLocation1.FlowRate > 0 && !this.OpenValves.Contains(this.CurrentLocation1) &&
                this.CurrentLocation2.FlowRate > 0 && !this.OpenValves.Contains(this.CurrentLocation2) &&
                (this.CurrentLocation1 != this.CurrentLocation2))
            {
                var newOpenValves = this.OpenValves.ToHashSet();
                newOpenValves.Add(this.CurrentLocation1);
                newOpenValves.Add(this.CurrentLocation2);

                yield return new State2(this.Minute + 1, this.CurrentLocation1, this.CurrentLocation2, this.CommulativeFlow + GetFlowDuringThisState(), newOpenValves, this, $"Player 1 Open valve {this.CurrentLocation1.Name}, Player 2 Open valve {this.CurrentLocation2.Name}", Player1Path, Player2Path);
            }

            foreach (var player1Neighbour in CurrentLocation1.ConnectedValves)
            {
                if (Player1Path.Count(w => w == player1Neighbour) > 2)
                    continue;

                var newPlayer1Path = Player1Path.ToList();
                newPlayer1Path.Add(player1Neighbour);

                foreach (var player2Neighbour in CurrentLocation2.ConnectedValves)
                {
                    if (Player2Path.Count(w => w == player2Neighbour) > 2)
                        continue;

                    var newPlayer2Path = Player2Path.ToList();
                    newPlayer2Path.Add(player2Neighbour);

                    yield return new State2(this.Minute + 1, player1Neighbour, player2Neighbour, this.CommulativeFlow + GetFlowDuringThisState(), this.OpenValves, this, $"Player 1 Move to {player1Neighbour.Name}, Player 2 move to {player2Neighbour.Name}", newPlayer1Path, newPlayer2Path);
                }
            }
        }
    }

    public void PrintHistory(Action<string> printAction)
    {
        if (this.PreviousState != null)
            this.PreviousState.PrintHistory(printAction);

        printAction(this.ToString());
    }

    private int GetFlowDuringThisState()
    {
        return this.OpenValves.Sum(w => w.FlowRate);
    }

    public override string ToString()
    {
        return $"== Minute {this.Minute} Player1 at {this.CurrentLocation1.Name} Player2 at {this.CurrentLocation2.Name} Commulative {this.CommulativeFlow} =={Environment.NewLine}Valves {string.Join(", ", this.OpenValves.Select(w => w.Name).OrderBy(w => w))} are open, releasing {GetFlowDuringThisState()} pressure.{Environment.NewLine}{this.TransitionAction}";
    }
}
