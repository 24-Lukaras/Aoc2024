using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aoc2024;

public static class Solutions
{
    public static int Solution_1_0(IEnumerable<string> input)
    {
        int result = 0;

        List<int> left = new List<int>();
        List<int> right = new List<int>();

        foreach (string line in input)
        {
            var arr = line.Split("   ");
            left.Add(int.Parse(arr[0]));
            right.Add(int.Parse(arr[1]));
        }


        left = left.Order().ToList();
        right = right.Order().ToList();

        for (int i = 0; i < left.Count; i++)
        {
            result += Math.Abs(left[i] - right[i]);
        }

        return result;
    }

    public static int Solution_1_1(IEnumerable<string> input)
    {
        int result = 0;

        List<int> left = new List<int>();
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (string line in input)
        {
            var arr = line.Split("   ");
            left.Add(int.Parse(arr[0]));
            var right = int.Parse(arr[1]);
            counts.TryGetValue(right, out var count);
            counts[right] = count + 1;
        }

        foreach (var item in left)
        {
            if (counts.TryGetValue(item, out var count))
            {
                result += count * item;
            }
        }

        return result;
    }

    public static int Solution_2_0(IEnumerable<string> input)
    {
        var result = 0;

        foreach (string line in input)
        {
            bool safe = true;
            int direction = 0;
            var arr = line.Split(' ').Select(int.Parse).ToArray();
            int last = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == 1)
                {
                    if (last == arr[i])
                    {
                        safe = false;
                        break;
                    }

                    direction = last > arr[i] ? 1 : -1;
                }
                if (i > 0)
                {
                    var difference = (last - arr[i]) * direction;
                    if (difference < 1 || difference > 3)
                    {
                        safe = false;
                        break;
                    }
                }
                last = arr[i];
            }
            if (safe)
            {
                result++;
            }
        }

        return result;
    }

    public static int Solution_2_1(IEnumerable<string> input)
    {
        var result = 0;

        Func<int, int, int, bool> isOkay = (direction, x, y) =>
        {
            var result = (x - y) * direction;
            return !(result < 1 || result > 3);
        };
        Func<int[], bool> validateSequence = (int[] sequence) =>
        {
            int direction = 0;
            int last = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (i == 1)
                {
                    if (last == sequence[i])
                    {
                        return false;
                    }

                    direction = last > sequence[i] ? 1 : -1;
                }
                if (i > 0)
                {
                    var difference = (last - sequence[i]) * direction;
                    if (!isOkay(direction, last, sequence[i]))
                    {
                        return false;
                    }
                }
                last = sequence[i];
            }
            return true;
        };

        foreach (string line in input)
        {
            var arr = line.Split(' ').Select(int.Parse).ToArray();
            bool safe = false;

            if (validateSequence(arr))
                safe = true;

            if (!safe)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var list = arr.ToList();
                    list.RemoveAt(i);
                    if (validateSequence(list.ToArray()))
                    {
                        safe = true;
                        break;
                    }
                }
            }

            if (safe)
                result++;

        }

        return result;
    }

    public static int Solution_3_0(string input)
    {
        int result = 0;
        Regex regex = new Regex(@"mul\((\d+),(\d+)\)");

        var matches = regex.Matches(input);
        foreach (Match match in matches)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);

            result += x * y;
        }

        return result;
    }
    public static int Solution_3_1(string input)
    {
        int result = 0;

        Regex regex = new Regex(@"mul\((\d+),(\d+)\)");
        Regex instructionRegex = new Regex(@"do\(\)|don't\(\)");

        var instructionMatches = instructionRegex.Matches(input).OrderBy(x => x.Index).ToList();
        var matches = regex.Matches(input);

        int nextIndex = instructionMatches[0].Index;
        int instructionIndex = 0;
        bool enabled = true;

        foreach (Match match in matches)
        {
            if (match.Index > nextIndex)
            {
                enabled = instructionMatches[instructionIndex].Value != "don't()";
                instructionIndex++;
                if (instructionIndex < instructionMatches.Count)
                {
                    nextIndex = instructionMatches[instructionIndex].Index;
                }
                else
                {
                    nextIndex = int.MaxValue;
                }
            }

            if (enabled)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);

                result += x * y;
            }
        }

        return result;
    }
}
