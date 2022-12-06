var input = new StringInputProvider("Input.txt").First();

for (int i = 3; i < input.Length; i++)
{
    var map = new HashSet<char>();
    map.Add(input[i]);
    map.Add(input[i-1]);
    map.Add(input[i - 2]);
    map.Add(input[i - 3]);

    if (map.Count == 4)
    {
        Console.WriteLine($"Part 1: {i+1}");
        break;
    }
}

for (int i = 13; i < input.Length; i++)
{
    var map = new HashSet<char>();
    
    for (int j = 13; j >= 0; j--)
    {
        map.Add(input[i - j]);
    }
    
    if (map.Count == 14)
    {
        Console.WriteLine($"Part 2: {i + 1}");
        break;
    }
}