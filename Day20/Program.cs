var originalOrder = new InputProvider<int>("Input.txt", int.TryParse).ToList();

var workingCopy = Enumerable.Range(0, originalOrder.Count).Select(w => new OrderedNumber(originalOrder[w], w)).ToList();
var list = new LinkedList<OrderedNumber>(workingCopy);

MixList(list, workingCopy);

Console.WriteLine($"Part 1: {GetAnswer(list, workingCopy)}");

workingCopy = Enumerable.Range(0, originalOrder.Count).Select(w => new OrderedNumber(originalOrder[w] * (long)811589153, w)).ToList();
list = new LinkedList<OrderedNumber>(workingCopy);

for (int operation = 0; operation < 10; operation++)
{
    MixList(list, workingCopy);
}

Console.WriteLine($"Part 2: {GetAnswer(list, workingCopy)}");

static void MixList(LinkedList<OrderedNumber> list, List<OrderedNumber> workingCopy)
{
    for (int i = 0; i < workingCopy.Count; i++)
    {
        var elementToMove = list.Find(workingCopy[i]);

        int numberOfSteps = (int)(elementToMove.Value.FullNumber % (list.Count - 1));

        if (elementToMove.Value.FullNumber == 0 ||
            numberOfSteps == 0)
            continue;

        var next = elementToMove.Next ?? list.First;
        var previous = elementToMove.Previous ?? list.Last;

        list.Remove(elementToMove);

        if (numberOfSteps > 0)
        {
            for (int step = 0; step < numberOfSteps - 1; step++)
            {
                next = next.Next ?? list.First;
            }

            list.AddAfter(next, elementToMove.Value);
        }
        else
        {
            numberOfSteps = -numberOfSteps;
            for (int step = 0; step < numberOfSteps - 1; step++)
            {
                previous = previous.Previous ?? list.Last;
            }

            list.AddBefore(previous, elementToMove.Value);
        }
    }
}

static long GetAnswer(LinkedList<OrderedNumber> list, List<OrderedNumber> workingCopy)
{
    var element = list.Find(workingCopy.First(w => w.FullNumber == 0));

    var elements = new List<long>();

    for (int i = 0; i <= 3000; i++)
    {
        if (i > 0 && i % 1000 == 0)
        {
            elements.Add(element.Value.FullNumber);
        }

        element = element.Next ?? list.First;
    }

    return elements.Sum();
}

void Print(LinkedList<OrderedNumber> list, List<OrderedNumber> numbers)
{
    var element = list.Find(numbers.First(w => w.FullNumber == 0));

    for (int i = 0; i < numbers.Count; i++)
    {
        Console.Write(element.Value.FullNumber.ToString() + ", ");
        element = element.Next ?? list.First;
    }
}

record OrderedNumber(long FullNumber, int OriginalIndex);