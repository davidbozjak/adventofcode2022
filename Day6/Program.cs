var input = new StringInputProvider("Input.txt").First();

for (int i = 4; i < input.Length; i++)
{
    var map = input[(i - 4)..i].ToHashSet();

    if (map.Count == 4)
    {
        Console.WriteLine($"Part 1: {i}");
        break;
    }
}

for (int i = 14; i < input.Length; i++)
{
    var map = input[(i - 14)..i].ToHashSet();

    if (map.Count == 14)
    {
        Console.WriteLine($"Part 2: {i}");
        break;
    }
}