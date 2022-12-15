using System.Drawing;
using System.Text.RegularExpressions;

//var stopwatch = System.Diagnostics.Stopwatch.StartNew();

var pairs = new InputProvider<SensorBeconPair?>("Input.txt", GetSensorBeconPair).Where(w => w != null).Cast<SensorBeconPair>().ToList();

int targetY = 2000000;

var rangesOnTargetRow = GetRangesForRow(targetY, pairs);

int canNotExist = rangesOnTargetRow.Sum(w => w.End - w.Start);

//Console.WriteLine(stopwatch.ElapsedMilliseconds);
Console.WriteLine($"Part 1: {canNotExist}");
//stopwatch = System.Diagnostics.Stopwatch.StartNew();

Point? beconPoint = null;

int searchSpace = 4000000;
var wholeRowRange = new Range(0, searchSpace);

for (int y = 0; y <= searchSpace && beconPoint == null; y++)
{
    var rangesForRow = GetRangesForRow(y, pairs);

    if (rangesForRow.Any(w => w.CoversWholeRange(wholeRowRange)))
    {
        continue;
    }

    var xThresholdValues = rangesForRow.SelectMany(w => new[] { w.Start, w.End }).OrderBy(w => w)
        .Where(w => wholeRowRange.ContainsPoint(w))
        .ToArray();

    if (xThresholdValues.Length != 2)
        throw new Exception();

    beconPoint = new Point(xThresholdValues[0] + 1, y);

    if (pairs.Any(w => w.CoversPoint(beconPoint.Value)))
        throw new Exception();
}

if (beconPoint == null) throw new Exception();

//Console.WriteLine(stopwatch.ElapsedMilliseconds);
Console.WriteLine($"Part 2: {((long)beconPoint.Value.X * 4000000) + beconPoint.Value.Y}");

static IEnumerable<Range> GetRangesForRow(int y, IEnumerable<SensorBeconPair> pairs)
{
    var nonOverlappingRanges = new List<Range>();

    var rangesForRow = pairs.Select(w => w.GetCoveredRangeInRow(y))
        .Where(w => w != null)
        .Cast<Range>()
        .ToList();

    var coveredRange = rangesForRow.First();
    rangesForRow = rangesForRow.Skip(1).ToList();

    while (rangesForRow.Count > 0)
    {
        bool removedAny = false;

        for (int i = 0; i < rangesForRow.Count; i++)
        {
            var range = rangesForRow[i];

            if (coveredRange.HasIntersect(range))
            {
                coveredRange = coveredRange.Union(range);
                rangesForRow.Remove(range);
                removedAny = true;
            }
        }

        if (!removedAny)
        {
            nonOverlappingRanges.Add(coveredRange);
            coveredRange = rangesForRow.First();
            rangesForRow = rangesForRow.Skip(1).ToList();
        }
    }

    nonOverlappingRanges.Add(coveredRange);

    return nonOverlappingRanges;
}

static bool GetSensorBeconPair(string? input, out SensorBeconPair? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 4) throw new Exception();

    value = new SensorBeconPair(new Point(numbers[0], numbers[1]), new Point(numbers[2], numbers[3]));

    return true;
}

class SensorBeconPair
{
    public Point SensorLocation { get; }

    public Point BeconLocation { get; }

    public int Range { get; }

    public SensorBeconPair(Point sensorLocation, Point beconLocation)
    {
        SensorLocation = sensorLocation;
        BeconLocation = beconLocation;

        int distanceX = Math.Abs(SensorLocation.X - BeconLocation.X);
        int distanceY = Math.Abs(SensorLocation.Y - BeconLocation.Y);

        this.Range = distanceX + distanceY;
    }

    public bool CoversPoint(Point p)
    {
        return p.Distance(SensorLocation) <= this.Range;
    }

    public Range? GetCoveredRangeInRow(int y)
    {
        int distanceY = Math.Abs(SensorLocation.Y - y);

        var distanceX = this.Range - distanceY;

        if (distanceX < 0) return null;

        return new Range(SensorLocation.X - distanceX, SensorLocation.X + distanceX);
    }
}

class Range
{
    public int Start { get; }
    public int End { get; }

    public Range (int start, int end)
    {
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
    }

    public bool HasIntersect(Range other)
    {
        if (this.End < other.Start) return false;
        if (this.Start > other.End) return false;

        return true;
    }

    public bool CoversWholeRange(Range other)
    {
        return other.Start >= this.Start && other.End <= this.End;
    }

    public bool ContainsPoint(int value)
    {
        return value >= this.Start && value <= this.End;
    }

    public Range Union(Range other)
    {
        if (!HasIntersect(other)) throw new Exception();

        return new Range(Math.Min(this.Start, other.Start), Math.Max(this.End, other.End));
    }
}