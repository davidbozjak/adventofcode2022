using System.Text.RegularExpressions;

var parser = new MultiLineParser<MonkeyBuilder>(() => new MonkeyBuilder(), (builder, str) => builder.AddRow(str));
var lineProvider = new StringInputProvider("Input.txt") { EndAtEmptyLine = false };
var builders = parser.AddRange(lineProvider);

var part1 = RunRounds(20, builders, w => w / 3);
Console.WriteLine($"Part 1: {part1}");

long modulo = builders.Select(w => w.Build()).Select(w => w.ModuloTest).Aggregate((a, b) => a * b);
Monkey.allCreatedInstances.Clear();

var part2 = RunRounds(10000, builders, w => w % modulo);
Console.WriteLine($"Part 2: {part2}");

static long RunRounds(int rounds, IEnumerable<MonkeyBuilder> monkeyBuilders, Func<long, long> dealingWithWorryFunc)
{
    var monkeys = monkeyBuilders.Select(w => w.Build()).OrderBy(w => w.Id).ToList();

    var roundsToInspect = new[] { 1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };

    for (int round = 1; round <= rounds; round++)
    {
        foreach (var monkey in monkeys)
        {
            monkey.ProcessItems(dealingWithWorryFunc);
        }

        //if (roundsToInspect.Contains(round))
        //{
        //    Console.WriteLine($"After round {round}:");
        //    foreach (var monkey in orderedMonkeys)
        //    {
        //        //Console.WriteLine($"Monkey{monkey.Id}: {string.Join(", ", monkey.Items.Select(w => w.Id))}");
        //        Console.WriteLine($"Monkey{monkey.Id}: {monkey.TotalInspectedItems}");
        //    }
        //}
    }

    var sorted = monkeys.Select(w => w.TotalInspectedItems).OrderByDescending(w => w);

    var mostActiveMonkey = sorted.First();
    var secondMostActiveMonkey = sorted.Skip(1).First();

    return mostActiveMonkey * secondMostActiveMonkey;
}

class Monkey
{
    public static readonly List<Monkey> allCreatedInstances = new();

    public int Id { get; }

    public Func<long, long> Operation { get; init; }

    public int RecepientIdIfTrue { get; init; }
    public int RecepientIdIfFalse { get; init; }

    private readonly Cached<Monkey> cachedRecepientIfTrue;
    private readonly Cached<Monkey> cachedRecepientIfFalse;

    public Monkey RecepientIfTrue => this.cachedRecepientIfTrue.Value;

    public Monkey RecepientIfFalse => this.cachedRecepientIfFalse.Value;

    public long ModuloTest { get; init; }

    public List<long> Items { get; init; } = new();

    public long TotalInspectedItems = 0;

    public Monkey(int id)
    {
        this.Id = id;
        
        cachedRecepientIfTrue = new Cached<Monkey>(() => allCreatedInstances.First(w => w.Id == RecepientIdIfTrue));
        cachedRecepientIfFalse = new Cached<Monkey>(() => allCreatedInstances.First(w => w.Id == RecepientIdIfFalse));
        
        allCreatedInstances.Add(this);
    }

    public void ProcessItems(Func<long, long> dealingWithWorryFunc)
    {
        if (this.RecepientIfTrue == this) throw new Exception();
        if (this.RecepientIfFalse == this) throw new Exception();

        foreach (var item in Items)
        {
            this.TotalInspectedItems++;

            var worry = item;
            worry = this.Operation(worry);

            worry = dealingWithWorryFunc(worry);

            var testResult = worry % this.ModuloTest == 0;

            var recepient = testResult ? this.RecepientIfTrue : this.RecepientIfFalse;

            recepient.Items.Add(worry);
        }

        this.Items.Clear();
    }
}

class MonkeyBuilder
{
    private static readonly Regex numRegex = new(@"-?\d+");
    private int row = 0;
    private int id;
    private List<long>? items;
    private Func<long, long>? operation;
    private int recepientIdIfTrue;
    private int recepientIdIfFalse;
    private long moduloTest;

    public void AddRow(string input)
    {
        if (row == 0)
        {
            this.id = GetSingleNumberFromInput(input);
        }
        else if (row == 1)
        {
            this.items = numRegex.Matches(input).Select(w => long.Parse(w.Value)).ToList();
        }
        else if (row == 2)
        {
            SetOperation(input);
        }
        else if (row == 3)
        {
            moduloTest = GetSingleNumberFromInput(input);
        }
        else if (row == 4)
        {
            recepientIdIfTrue = GetSingleNumberFromInput(input);
        }
        else if (row == 5)
        {
            recepientIdIfFalse = GetSingleNumberFromInput(input);
        }

        row++;
    }

    public Monkey Build()
    {
        if (row != 6) throw new Exception();

        return new Monkey(id)
        {
            Items = items?.ToList() ?? throw new Exception(),
            Operation = operation ?? throw new Exception(),
            ModuloTest = moduloTest,
            RecepientIdIfTrue = recepientIdIfTrue,
            RecepientIdIfFalse = recepientIdIfFalse
        };
    }

    private void SetOperation(string input)
    {
        if (input.Contains('+'))
        {
            var number = GetSingleNumberFromInput(input);
            operation = old => old + number;
        }
        else if (input.Contains('*'))
        {
            if (numRegex.IsMatch(input))
            {
                var number = GetSingleNumberFromInput(input);
                operation = old => old * number;
            }
            else
            {
                //we know it's only old * old that matches this
                operation = old => old * old;
            }
        }
    }

    private static int GetSingleNumberFromInput(string input) =>
        int.Parse(numRegex.Match(input).Value);
}