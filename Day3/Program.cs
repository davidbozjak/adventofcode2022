var backpacks = new StringInputProvider("Input.txt").ToArray();

int prioritySum = 0;

foreach (var backpack in backpacks)
{
    int compartmentSize = backpack.Length / 2;
    var compartment1 = backpack[..compartmentSize];
    var compartment2 = backpack[compartmentSize..];

    for (int i = 0; i < compartmentSize; i++)
    {
        var item = compartment1[i];
        if (compartment2.Contains(item))
        {
            prioritySum += GetItemPriority(item);
            break;
        }
    }
}

Console.WriteLine($"Part 1: {prioritySum}");

int badgePrioritySum = 0;

for (int i = 0; i < backpacks.Length; i += 3)
{
    for (int itemIndex = 0; itemIndex < backpacks[i].Length; itemIndex++)
    {
        var item = backpacks[i][itemIndex];

        if (backpacks[i + 1].Contains(item) && backpacks[i + 2].Contains(item))
        {
            badgePrioritySum += GetItemPriority(item);
            break;
        }
    }
}

Console.WriteLine($"Part 2: {badgePrioritySum}");

int GetItemPriority(char item) =>
    item - (char.IsLower(item) ? 96 : 38);