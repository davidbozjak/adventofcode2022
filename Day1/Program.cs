var parser = new MultiLineParser<Elf>(() => new Elf(), (elf, str) => elf.AddItem(int.Parse(str)));
var wholeStringInput = new StringInputProvider("Input.txt") { EndAtEmptyLine = false };

var elves = parser.AddRange(wholeStringInput);

Console.WriteLine($"Part 1: Max: {elves.Max(w => w.TotalCalories)}");
Console.WriteLine($"Part 2: Max 3: {elves.OrderByDescending(w => w.TotalCalories).Take(3).Sum(w => w.TotalCalories)}");

class Elf
{
    public int TotalCalories { get; private set; } = 0;

    public void AddItem(int calories)
    {
        this.TotalCalories += calories;
    }
}