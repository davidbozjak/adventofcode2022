using System.Data;
using System.Text.RegularExpressions;

var parser = new MultiLineParser<PacketPairBuilder>(() => new PacketPairBuilder(), (builder, str) => builder.AddLine(str));
var wholeStringInput = new StringInputProvider("Input.txt") { EndAtEmptyLine = false };

var builders = parser.AddRange(wholeStringInput);

var packetPairs = builders.Select(w => w.Build()).ToList();

var sumOfIndices = Enumerable.Range(0, packetPairs.Count)
    .Select(w => new { Index = w + 1, ResultOfCompare = ComparePacketPair(packetPairs[w]) })
    .Where(w => w.ResultOfCompare == -1)
    .Sum(w => w.Index);

Console.WriteLine($"Part 1: {sumOfIndices}");

var allPackets = packetPairs.SelectMany(w => new[] { w.Packet1, w.Packet2 }).ToList();

string divider1 = "[[2]]";

string divider2 = "[[6]]";

allPackets.Add(divider1);
allPackets.Add(divider2);

allPackets.Sort((e1, e2) => ComparePacketPair(new PacketPair(e1, e2)));

var divider1Index = allPackets.IndexOf(divider1) + 1;
var divider2Index = allPackets.IndexOf(divider2) + 1;

Console.WriteLine($"Part 2: {divider1Index * divider2Index}");


static int ComparePacketPair(PacketPair pair)
{
    var leftMembers = GetValuesAndListsOnOneLevel(pair.Packet1);
    var rightMembers = GetValuesAndListsOnOneLevel(pair.Packet2);

    var maxLength = new int[] { leftMembers.Count, rightMembers.Count }.Max();

    for (int i = 0; i < maxLength; i++)
    {
        if (leftMembers.Count == i) return -1;
        if (rightMembers.Count == i) return 1;

        bool isLeftAnInt = IsPacketInt(leftMembers[i]);
        bool isRightAnInt = IsPacketInt(rightMembers[i]);

        if (isLeftAnInt && isRightAnInt)
        {
            int leftValue = int.Parse(leftMembers[i]);
            int rightValue = int.Parse(rightMembers[i]);

            if (leftValue < rightValue) return -1;
            else if (leftValue > rightValue) 
                return 1;
        }
        else if (!isLeftAnInt && !isRightAnInt)
        {
            var subPair = new PacketPair(leftMembers[i], rightMembers[i]);

            var orderOfSubPair = ComparePacketPair(subPair);

            if (orderOfSubPair != 0)
                return orderOfSubPair;
        }
        else
        {
            if (isLeftAnInt) leftMembers[i] = ConvertIntPacketToList(leftMembers[i]);
            if (isRightAnInt) rightMembers[i] = ConvertIntPacketToList(rightMembers[i]);
            i--;
            continue;
        }
    }

    return 0;
}

static bool IsPacketInt(string str)
{
    return str[0] != '[';
}

static string ConvertIntPacketToList(string str)
{
    if (str.Contains(',')) throw new Exception();

    return $"[{str}]";
}

static int FindEndOfPacket(string str, int startIndex)
{
    int level = 0;

    if (str[startIndex] != '[') throw new Exception();

    for (int i = startIndex; i < str.Length; i++)
    {
        if (str[i] == '[') level++;
        else if (str[i] == ']')
        {
            level--;

            if (level == 0) 
                return i + 1;
        }
    }

    throw new Exception();
}

static List<string> GetValuesAndListsOnOneLevel(string packet)
{
    if (packet[0] != '[') throw new Exception();
    if (packet[^1] != ']') throw new Exception();

    var list = new List<string>();

    for (int i = 1; i < packet.Length - 1; i++)
    {
        if (packet[i] == '[')
        {
            int end = FindEndOfPacket(packet, i);
            list.Add(packet[i..end]);
            i = end;
        }
        else if (packet[i] == ',')
        {
            continue;
        }
        else
        {
            int indexOfFirstComa = packet.IndexOf(',', i);
            int indexOfEndOfList = packet.IndexOf(']', i);

            indexOfFirstComa = indexOfFirstComa == -1 ? int.MaxValue : indexOfFirstComa;
            indexOfEndOfList = indexOfEndOfList == -1 ? int.MaxValue : indexOfEndOfList;

            int endOfInt = indexOfFirstComa < indexOfEndOfList ? indexOfFirstComa : indexOfEndOfList;

            list.Add(packet[i..endOfInt]);
            i = endOfInt;
        }
    }

    return list;
}

class PacketPairBuilder
{
    string? part1;
    string? part2;
    int partCount = 0;

    public void AddLine(string line)
    {
        if (partCount == 0)
        {
            part1 = line;
        }
        else if (partCount == 1)
        {
            part2 = line;
        }
        else throw new Exception();

        partCount++;
    }

    public PacketPair Build()
    {
        if (partCount != 2) throw new Exception();

        return new PacketPair(part1 ?? throw new Exception(), part2 ?? throw new Exception());
    }
}

record PacketPair(string Packet1, string Packet2);