using System.Text.RegularExpressions;

var assignments = new InputProvider<AssignmentForPair?>("Input.txt", GetAssignmentForPair).Where(w => w != null).Cast<AssignmentForPair>().ToList();

Console.WriteLine($"Part 1: {assignments.Count(w => w.IsOneFullyContainedByOther())}");
Console.WriteLine($"Part 2: {assignments.Count(w => w.DoOverlapAtAll())}");

static bool GetAssignmentForPair(string? input, out AssignmentForPair? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    value = new AssignmentForPair(numbers[0], numbers[1], numbers[2], numbers[3]);

    return true;
}

record AssignmentForPair(int Start1, int End1, int Start2, int End2)
{
    public bool IsOneFullyContainedByOther()
    {
        return Contains(Start1, End1, Start2, End2) ||
            Contains(Start2, End2, Start1, End1);

        static bool Contains(int start, int end, int otherStart, int otherEnd)
        {
            return start <= otherStart && end >= otherEnd;
        }
    }

    public bool DoOverlapAtAll()
    {
        return IsPointOnLine(Start1, Start2, End2) ||
            IsPointOnLine(End1, Start2, End2) ||
            IsPointOnLine(Start2, Start1, End1) ||
            IsPointOnLine(End2, Start1, End1);

        static bool IsPointOnLine(int point, int start, int end)
            => point >= start && point <= end;
    }
}