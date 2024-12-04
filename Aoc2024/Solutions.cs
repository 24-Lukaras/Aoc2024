﻿using System.Collections.Generic;
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

    public static int Solution_4_0(List<string> input)
    {
        HashSet<(int x0, int y0, int x1, int y1)> entries = new HashSet<(int x0, int y0, int x1, int y1)>();

        Func<(int x, int y), (int x, int y), string, List<string>, bool> findWithVector = (start, vector, phrase, input) =>
        {
            (int x, int y) position = start;
            for (int i = 0; i < phrase.Length; i++)
            {
                if (input[position.y][position.x] != phrase[i])
                    return false;

                if (i < phrase.Length - 1)
                {
                    position = (position.x + vector.x, position.y + vector.y);

                    if (position.x < 0 || position.x >= input[0].Length || position.y < 0 || position.y >= input.Count)
                        return false;
                }

            }

            if (!entries.Contains((position.x, position.y, start.x, start.y)))
            {
                entries.Add((start.x, start.y, position.x, position.y));
            }
            return true;
        };

        (int x, int y)[] vectors = 
        [
            (0, -1),
            (1, -1),
            (1, 0),
            (1, 1),
            (0, 1),
            (-1, 1),
            (-1, 0),
            (-1, -1)
        ];

        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                char c = input[y][x];
                if (c == 'X')
                {
                    foreach (var vector in vectors)
                    {
                        findWithVector((x, y), vector, "XMAS", input);
                    }
                }
                else if (c == 'S')
                {
                    foreach (var vector in vectors)
                    {
                        findWithVector((x, y), vector, "SAMX", input);
                    }
                }
            }
        }

        return entries.Count;
    }

    public static int Solution_4_1(List<string> input)
    {
        int result = 0;
        char[] chars = new char[4];
        for (int y = 1; y < input.Count - 1; y++)
        {
            for (int x = 1; x < input[y].Length - 1; x++)
            {
                char c = input[y][x];
                if (c == 'A')
                {
                    chars[0] = input[y - 1][x - 1];
                    chars[1] = input[y + 1][x + 1];
                    chars[2] = input[y + 1][x - 1];
                    chars[3] = input[y - 1][x + 1];
                    if (chars[0] == chars[1] || chars[2] == chars[3])
                        continue;

                    bool invalidCharacter = false;
                    foreach (var @char in chars)
                    {
                        if (@char == 'X' || @char == 'A')
                        {
                            invalidCharacter = true;
                            break;
                        }
                    }

                    if (!invalidCharacter)
                        result++;
                }
            }
        }

        return result;
    }
}
