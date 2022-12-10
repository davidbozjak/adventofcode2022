using System.Diagnostics;

class Computer
{
    private readonly UniqueFactory<string, Register> registers;
    private readonly List<string> instructions;
    private readonly List<RegisterSnapshot> registerSnapshots = new();

    public int InstructionsExecuted { get; private set; }

    public int CylceNumber { get; private set; }

    public Computer(IEnumerable<string> instructions)
    {
        this.instructions = instructions.ToList();
        this.registers = new UniqueFactory<string, Register>(name => new Register(name));
    }

    public void SetRegisterValue(string register, long value)
    {
        var r = registers.GetOrCreateInstance(register);
        r.Value = value;
    }

    public long GetRegisterValue(string register)
    {
        var r = registers.GetOrCreateInstance(register);
        return r.Value;
    }

    public long GetRegisterValueAtCycle(string register, int cycleNumber)
    {
        return this.registerSnapshots.Where(w => w.CycleNumber == cycleNumber && w.Register == register).First().Value;
    }

    public bool Run()
    {
        this.CylceNumber = 1;

        for (int instructionLine = 0; instructionLine >= 0 && instructionLine < instructions.Count; instructionLine++, this.InstructionsExecuted++)
        {
            //if (instructionPointerRegister.Value == 29)
            //{
            //    Console.WriteLine($"Debugging computer, accepted input would be: {GetRegisterValue("3")}");
            //}

            var instruction = instructions[instructionLine];

            var parts = instruction.Split(" ");

            var operation = parts[0];

            for (int i = 0; i < GetInstructionDuration(operation); i++)
            {
                ProcessCycle();
            }

            if (operation == "addx")
            {
                var r = registers.GetOrCreateInstance("x");

                r.Value += int.Parse(parts[1]);
            }
            else if (operation == "noop")
            {
                // no op
            }
            else throw new Exception("Unknown instruction");
        }

        ProcessCycle();

        return true;
    }

    private static int GetInstructionDuration(string instructionName)
    {
        return instructionName switch
        {
            "addx" => 2,
            "noop" => 1,
            _ => throw new Exception()
        };
    }

    private void ProcessCycle()
    {
        foreach (var register in this.registers.AllCreatedInstances)
        {
            this.registerSnapshots.Add(new RegisterSnapshot(this.CylceNumber, register.Name, register.Value));
        }

        this.CylceNumber++;
    }

    [DebuggerDisplay("{Name}:{Value}")]
    protected class Register
    {
        public string Name { get; }
        public long Value { get; set; } = 0;

        public Register(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"[[{Name}]:[{Value}]]";
        }
    }

    protected record RegisterSnapshot(int CycleNumber, string Register, long Value);
}