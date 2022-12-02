var rounds = new InputProvider<RoundRecord?>("Input.txt", GetRoundRecord).Where(w => w != null).Cast<RoundRecord>().ToList();

Console.WriteLine($"Part 1: {rounds.Select(CalculateRoundScorePart1).Sum()}");
Console.WriteLine($"Part 2: {rounds.Select(CalculateRoundScorePart2).Sum()}");

int CalculateRoundScorePart1(RoundRecord record)
{
    int score = 0;

    // shape you selected (1 for Rock, 2 for Paper, and 3 for Scissors)
    score += record.Player2Move switch
    {
        Move.Rock => 1,
        Move.Paper => 2,
        Move.Scisors => 3,
        _ => throw new Exception()
    };

    // plus the score for the outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won)

    if (record.Player1Move == record.Player2Move)
    {
        score += 3;
    }
    else
    {
        if (record.Player2Move == Move.Rock)
        {
            if (record.Player1Move == Move.Paper)
            {
                // I lose
                score += 0;
            }
            else
            {
                score += 6;
            }
        }
        else if  (record.Player2Move == Move.Paper)
        {
            if (record.Player1Move == Move.Scisors)
            {
                // I lose
                score += 0;
            }
            else
            {
                score += 6;
            }
        }
        else if (record.Player2Move == Move.Scisors)
        {
            if (record.Player1Move == Move.Rock)
            {
                // I lose
                score += 0;
            }
            else
            {
                score += 6;
            }
        }
    }

    return score;
}

int CalculateRoundScorePart2(RoundRecord record)
{
    Move augmentedPlayerMove;

    if (record.DesiredOutcome == Outcome.Draw)
    {
        augmentedPlayerMove = record.Player1Move;
    }
    else if (record.DesiredOutcome == Outcome.Lose)
    {
        if (record.Player1Move == Move.Rock)
        {
            augmentedPlayerMove = Move.Scisors;
        }
        else if (record.Player1Move == Move.Paper)
        {
            augmentedPlayerMove = Move.Rock;
        }
        else
        {
            augmentedPlayerMove = Move.Paper;
        }
    }
    else
    {
        if (record.Player1Move == Move.Rock)
        {
            augmentedPlayerMove = Move.Paper;
        }
        else if (record.Player1Move == Move.Paper)
        {
            augmentedPlayerMove = Move.Scisors;
        }
        else
        {
            augmentedPlayerMove = Move.Rock;
        }
    }

    var augmentedRecord = new RoundRecord(record.Player1Move, augmentedPlayerMove, record.DesiredOutcome);

    return CalculateRoundScorePart1(augmentedRecord);
}

static bool GetRoundRecord(string? input, out RoundRecord? value)
{
    value = null;

    if (input == null) return false;

    if (input.Length != 3) throw new Exception();

    var player1Move = input[0] switch
    {
        'A' => Move.Rock,
        'B' => Move.Paper,
        'C' => Move.Scisors,
        _ => throw new Exception()
    };

    var player2Move = input[2] switch
    {
        'X' => Move.Rock,
        'Y' => Move.Paper,
        'Z' => Move.Scisors,
        _ => throw new Exception()
    };

    var desiredOutcome = input[2] switch
    {
        'X' => Outcome.Lose,
        'Y' => Outcome.Draw,
        'Z' => Outcome.Win,
        _ => throw new Exception()
    };

    value = new RoundRecord(player1Move, player2Move, desiredOutcome);

    return true;
}

enum Move { Rock, Paper, Scisors };
enum Outcome { Lose, Draw, Win}
record RoundRecord(Move Player1Move, Move Player2Move, Outcome DesiredOutcome);