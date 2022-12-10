var instructions = new StringInputProvider("Input.txt").ToList();

var computer = new Computer(instructions);
computer.SetRegisterValue("x", 1);
computer.Run();

var cyclesOfInterest = new[] { 20, 60, 100, 140, 180, 220 };

var signalStrenghts = cyclesOfInterest.Select(w => w * computer.GetRegisterValueAtCycle("x", w)).ToArray();

Console.WriteLine($"Part 1: {signalStrenghts.Sum()}");

Console.WriteLine();
Console.WriteLine("Part 2:");

int cycleNumber = 1;
for (int row = 0; row < 6; row++)
{
    for (int i = 0; i < 40; i++)
    {
        var spriteX = computer.GetRegisterValueAtCycle("x", cycleNumber++);

        var isSpriteVisible = Math.Abs(i - spriteX) < 2;

        Console.Write(isSpriteVisible ? '#' : ' ');
    }

    Console.WriteLine();
}