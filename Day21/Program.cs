using System.Runtime.InteropServices;

var monkeys = new InputProvider<Monkey?>("Input.txt", GetMonkey).Where(w => w != null).Cast<Monkey>().ToList();

var root = monkeys.First(w => w.Name == "root");

Console.WriteLine($"Part 1: {root.Number}");

Monkey.allInstances.Remove("root");
Monkey.allInstances.Remove("humn");

root = new EqualsMonkey("root", ((OperationMonkey)root).Monkey1Name, ((OperationMonkey)root).Monkey2Name);
var humn = new InputMonkey("humn");

((EqualsMonkey)root).BalanceSides();

Console.WriteLine($"Part 2: {humn.Number}");

if (!((EqualsMonkey)root).AreSidesBalanced())
    throw new Exception();

Console.WriteLine($"Part 2: {humn.Number}");



static bool GetMonkey(string? input, out Monkey? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    parts[0] = parts[0][..^1];

    if (parts.Length == 2)
    {
        value = new NumberMonkey(parts[0], int.Parse(parts[1]));
    }
    else if (parts.Length == 4)
    {
        var operation = parts[2] switch
        {
            "+" => Operation.Plus,
            "-" => Operation.Minus,
            "*" => Operation.Multiply,
            "/" => Operation.Divide,
            _ => throw new Exception()
        };

        value = new OperationMonkey(parts[0], parts[1], parts[3], operation);
    }
    else throw new Exception();

    return true;
}

abstract class Monkey
{
    public static readonly Dictionary<string, Monkey> allInstances = new();

    public string Name { get; }

    public abstract decimal Number { get; }

    public abstract bool CanCalculate { get; }

    public Monkey(string name)
    {
        Name = name;
        allInstances.Add(name, this);
    }

    public abstract void Invalidate();
}

class NumberMonkey : Monkey
{
    public NumberMonkey(string name, int number) 
        : base(name)
    {
        this.Number = number;
    }

    public override decimal Number { get; }

    public override bool CanCalculate => true;

    public override void Invalidate()
    {
        //no op
    }
}

enum Operation { Plus, Minus, Divide, Multiply};

class OperationMonkey : Monkey
{
    public OperationMonkey(string name, string monkey1Name, string monkey2Name, Operation operation) 
        : base(name)
    {
        this.Monkey1Name = monkey1Name;
        this.Monkey2Name = monkey2Name;
        this.Operation = operation;
        
        this.cachedResult = new Cached<decimal>(GetResult);
    }

    private readonly Cached<decimal> cachedResult;
    public override decimal Number => this.cachedResult.Value;

    public override bool CanCalculate => allInstances[Monkey1Name].CanCalculate && allInstances[Monkey2Name].CanCalculate;

    public string Monkey1Name { get; }
    public string Monkey2Name { get; }
    public Operation Operation { get; }

    public override void Invalidate()
    {
        this.cachedResult.Reset();

        var monkey1 = allInstances[Monkey1Name];
        var monkey2 = allInstances[Monkey2Name];

        monkey1.Invalidate();
        monkey2.Invalidate();
    }

    private decimal GetResult()
    {
        var monkey1 = allInstances[Monkey1Name];
        var monkey2 = allInstances[Monkey2Name];

        var number1 = monkey1.Number;
        var number2 = monkey2.Number;

        var result = this.Operation switch
        {
            Operation.Plus => number1 + number2,
            Operation.Minus => number1 - number2,
            Operation.Divide => number1 / number2,
            Operation.Multiply => number1 * number2,
            _ => throw new Exception()
        };

        return result;
    }

    public void SetResult(decimal result)
    {
        var monkey1 = allInstances[Monkey1Name];
        var monkey2 = allInstances[Monkey2Name];

        Monkey monkeyThatCouldNot;

        decimal subResult;

        if (monkey1.CanCalculate)
        {
            if (monkey2.CanCalculate)
                throw new Exception();

            monkeyThatCouldNot = monkey2;

            subResult = this.Operation switch
            {
                Operation.Plus => -(monkey1.Number - result),
                Operation.Minus => monkey1.Number - result,
                Operation.Divide => monkey1.Number / result,
                Operation.Multiply => result / monkey1.Number,
                _ => throw new Exception()
            };

            var verifyResult = this.Operation switch
            {
                Operation.Plus => monkey1.Number + subResult,
                Operation.Minus => monkey1.Number - subResult,
                Operation.Divide => monkey1.Number / subResult,
                Operation.Multiply => monkey1.Number * subResult,
                _ => throw new Exception()
            };

            if (Math.Abs(result - verifyResult) > (decimal)1e2)
                throw new Exception();
        }
        else if (monkey2.CanCalculate)
        {
            if (monkey1.CanCalculate)
                throw new Exception();

            monkeyThatCouldNot = monkey1;

            subResult = this.Operation switch
            {
                Operation.Plus => result - monkey2.Number,
                Operation.Minus => result + monkey2.Number,
                Operation.Divide => result * monkey2.Number,
                Operation.Multiply => result / monkey2.Number,
                _ => throw new Exception()
            };

            var verifyResult = this.Operation switch
            {
                Operation.Plus => subResult + monkey2.Number,
                Operation.Minus => subResult - monkey2.Number,
                Operation.Divide => subResult / monkey2.Number,
                Operation.Multiply => subResult * monkey2.Number,
                _ => throw new Exception()
            };

            if (Math.Abs(result - verifyResult) > (decimal)1e2)
                throw new Exception();
        }
        else throw new Exception();

        if (monkeyThatCouldNot is InputMonkey inpt)
        {
            inpt.SetInput(subResult);
        }
        else
        {
            ((OperationMonkey)monkeyThatCouldNot).SetResult(subResult);
        }

        this.Invalidate();

        if (Math.Abs(this.Number - result) > (decimal)1e-2)
            throw new Exception();
    }
}

class InputMonkey : Monkey
{
    public InputMonkey(string name) : base(name)
    {
    }

    private decimal? input;

    public override decimal Number => this.input ?? throw new Exception();

    public override bool CanCalculate => this.input != null;

    public void SetInput(decimal resultToGive)
    {
        this.input = resultToGive;
    }

    public override void Invalidate()
    {
        // no op
    }
}

class EqualsMonkey : Monkey
{
    public string Monkey1Name { get; }
    public string Monkey2Name { get; }

    public EqualsMonkey(string name, string monkey1Name, string monkey2Name)
        : base(name)
    {
        this.Monkey1Name = monkey1Name;
        this.Monkey2Name = monkey2Name;
    }

    public override decimal Number => throw new Exception();

    public override bool CanCalculate => allInstances[Monkey1Name].CanCalculate && allInstances[Monkey2Name].CanCalculate;

    public override void Invalidate()
    {
        // no op
    }

    public void BalanceSides()
    {
        var monkey1 = allInstances[Monkey1Name];
        var monkey2 = allInstances[Monkey2Name];

        Monkey monkeyThatCould;
        OperationMonkey monkeyThatCouldNot;

        if (monkey1.CanCalculate)
        {
            monkeyThatCould = monkey1;
            monkeyThatCouldNot = (OperationMonkey)monkey2;
        }
        else if (monkey2.CanCalculate)
        {
            monkeyThatCould = monkey2;
            monkeyThatCouldNot = (OperationMonkey)monkey1;
        }
        else throw new Exception();

        monkeyThatCouldNot.SetResult(monkeyThatCould.Number);
    }
    
    public bool AreSidesBalanced()
    {
        var monkey1 = allInstances[Monkey1Name];
        var monkey2 = allInstances[Monkey2Name];

        monkey1.Invalidate();
        monkey2.Invalidate();

        return monkey1.Number == monkey2.Number;
    }
}