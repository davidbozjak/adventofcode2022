using System.Drawing;
using System.Text.RegularExpressions;

//var stopwatch = System.Diagnostics.Stopwatch.StartNew();

var pairs = new InputProvider<SensorBeconPair?>("Input.txt", GetSensorBeconPair).Where(w => w != null).Cast<SensorBeconPair>().ToList();

int targetY = 2000000;

var intervalsOnTargetRow = GetIntervalsForRow(targetY, pairs);

int canNotExist = intervalsOnTargetRow.Sum(w => w.Length);

//Console.WriteLine(stopwatch.ElapsedMilliseconds);
Console.WriteLine($"Part 1: {canNotExist}");
//stopwatch = System.Diagnostics.Stopwatch.StartNew();

Point? beconPoint = null;

int searchSpace = 4000000;
var wholeRowInterval = new ClosedInterval(0, searchSpace);

for (int y = 0; y <= searchSpace && beconPoint == null; y++)
{
    var intervalsForRow = GetIntervalsForRow(y, pairs);

    if (intervalsForRow.Any(w => w.CoversWholeInterval(wholeRowInterval)))
    {
        continue;
    }

    var xThresholdValues = intervalsForRow.SelectMany(w => new[] { w.Start, w.End }).OrderBy(w => w)
        .Where(w => wholeRowInterval.ContainsPoint(w))
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

static IEnumerable<ClosedInterval> GetIntervalsForRow(int y, IEnumerable<SensorBeconPair> pairs)
{
    var nonOverlappingIntervals = new List<ClosedInterval>();

    var intervalsForRow = pairs.Select(w => w.GetCoveredIntervalForRow(y))
        .Where(w => w != null)
        .Cast<ClosedInterval>()
        .ToList();

    var coveredInterval = intervalsForRow.First();
    intervalsForRow = intervalsForRow.Skip(1).ToList();

    while (intervalsForRow.Count > 0)
    {
        bool removedAny = false;

        for (int i = 0; i < intervalsForRow.Count; i++)
        {
            var interval = intervalsForRow[i];

            if (coveredInterval.HasIntersect(interval))
            {
                coveredInterval = coveredInterval.Union(interval);
                intervalsForRow.Remove(interval);
                removedAny = true;
            }
        }

        if (!removedAny)
        {
            nonOverlappingIntervals.Add(coveredInterval);
            coveredInterval = intervalsForRow.First();
            intervalsForRow = intervalsForRow.Skip(1).ToList();
        }
    }

    nonOverlappingIntervals.Add(coveredInterval);

    return nonOverlappingIntervals;
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

    public ClosedInterval? GetCoveredIntervalForRow(int y)
    {
        int distanceY = Math.Abs(SensorLocation.Y - y);

        var distanceX = this.Range - distanceY;

        if (distanceX < 0) return null;

        return new ClosedInterval(SensorLocation.X - distanceX, SensorLocation.X + distanceX);
    }
}

