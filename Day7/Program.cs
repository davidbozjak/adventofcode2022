var input = new StringInputProvider("Input.txt").Skip(1).ToArray();

var root = new AoCDirectory() { Name = "/", Parent = null };
var currentDirectory = root;

bool isInLS = false;

foreach (var line in input)
{
    if (isInLS)
    {
        if (line.StartsWith("$"))
        {
            isInLS = false;
        }
        else
        {
            if (line.StartsWith("dir"))
            {
                var subdirName = line[4..];
                currentDirectory.AddSubdir(subdirName);
            }
            else
            {
                var fileDescriptors = line.Split(" ");
                var file = new AocFile() { Name = fileDescriptors[1], Size = long.Parse(fileDescriptors[0]), Parent = currentDirectory };
                currentDirectory.AddFile(file);
            }
            continue;
        }
    }

    if (line == "$ cd ..")
    {
        currentDirectory = currentDirectory.Parent;
    }
    else if (line.StartsWith("$ cd"))
    {
        var subdirName = line[5..];
        currentDirectory = currentDirectory.GetSubdir(subdirName);
    }
    else if (line == "$ ls")
    {
        isInLS = true;
    }
}

Console.WriteLine($"Part 1: {AoCDirectory.AllCreatedInstances.Where(w => w.Size < 100000).Sum(w => w.Size)}");

long totalDisc = 70000000;
long requiredSpace = 30000000;

long remaining = totalDisc - root.Size;
long minSpaceToFreeUp = requiredSpace - remaining;

Console.WriteLine($"Part 2: {AoCDirectory.AllCreatedInstances.Where(w => w.Size >= minSpaceToFreeUp).Min(w => w.Size)}");

class AoCDirectory
{
    private static List<AoCDirectory> allCreatedInstances = new List<AoCDirectory>();
    public static IEnumerable<AoCDirectory> AllCreatedInstances => allCreatedInstances;

    public AoCDirectory? Parent { get; init; }
    public string Name { get; init; }

    private readonly List<AoCDirectory> subdirs = new();
    private readonly List<AocFile> files = new();
    private readonly Cached<long> cachedSize;

    public long Size => this.cachedSize.Value;

    public AoCDirectory()
    {
        this.cachedSize = new Cached<long>(() => this.subdirs.Sum(w => w.Size) + this.files.Sum(w => w.Size));
        allCreatedInstances.Add(this);
    }

    public AoCDirectory AddSubdir(string name)
    {
        this.cachedSize.Reset();

        var subdir = new AoCDirectory() { Name = name, Parent = this };
        this.subdirs.Add(subdir);
        return subdir;
    }

    public AoCDirectory GetSubdir(string name)
    {
        return this.subdirs.First(w => w.Name == name);
    }

    public void AddFile(AocFile file)
    {
        this.cachedSize.Reset();

        this.files.Add(file);
    }
}

class AocFile
{
    public long Size { get; init; }
    public string Name { get; init; }

    public AoCDirectory Parent { get; init; }
}