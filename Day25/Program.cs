using System.Text;

var snafuInput = new StringInputProvider("Input.txt").ToList();

var numbers = snafuInput.Select(ConvertSnafuToDecimal).ToList();

var sum = numbers.Sum();
var sumSnafu = ConvertDecimalToSnafu(sum);

// Double check conversion before giving answer
if (ConvertSnafuToDecimal(sumSnafu) != sum)
    throw new Exception();

Console.WriteLine($"Part 1: {sumSnafu}");

static long ConvertSnafuToDecimal(string snafu)
{
    long baseMultiplier = 1;

    long number = 0;

    for (int i = snafu.Length - 1; i >= 0; i--)
    {
        var c = snafu[i];

        number += c switch
        {
            '2' => 2 * baseMultiplier,
            '1' => baseMultiplier,
            '0' => 0,
            '-' => -baseMultiplier,
            '=' => -2 * baseMultiplier,
            _ => throw new Exception()
        };
        
        baseMultiplier *= 5;
    }

    // use input to also test conversion in the oposite direction
    if (ConvertDecimalToSnafu(number) != snafu)
        throw new Exception();

    return number;
}

static string ConvertDecimalToSnafu(long input)
{
    var builder = new StringBuilder();

    long baseMultiplier = 1;

    while (input > 2 * baseMultiplier)
    {
        baseMultiplier *= 5;
    }

    long remainder = input;

    while (baseMultiplier >= 1)
    {
        if (remainder > 0)
        {
            int digit = 0;

            while (remainder > MaxForNextSmallerBase(baseMultiplier))
            {
                digit++;
                remainder -= baseMultiplier;
            }

            if (digit == 2) builder.Append('2');
            else if (digit == 1) builder.Append('1');
            else if (digit == 0) builder.Append('0');
            else throw new Exception();
        }
        else if (remainder < 0)
        {
            remainder *= -1;
            int digit = 0;

            while (remainder > MaxForNextSmallerBase(baseMultiplier))
            {
                digit++;
                remainder -= baseMultiplier;
            }

            remainder *= -1;

            if (digit == 2) builder.Append('=');
            else if (digit == 1) builder.Append('-');
            else if (digit == 0) builder.Append('0');
            else throw new Exception();
        }
        else builder.Append('0');

        baseMultiplier /= 5;
    }

    // handle corner case when number starts with leading zeroes
    while (builder[0] == '0')
        builder.Remove(0, 1);

    var resultString = builder.ToString();

    return resultString;

    static long MaxForNextSmallerBase(long currentBase)
    {
        long max = 0;

        currentBase /= 5;

        while (currentBase >= 1)
        {
            max += 2 * currentBase;
            currentBase /= 5;
        }

        return max;
    }
}