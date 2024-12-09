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

    public static int Solution_5_0(IEnumerable<string> input)
    {
        int result = 0;

        HashSet<(int, int)> rules = new HashSet<(int, int)>();
        bool creatingRules = true;
        foreach (var line in input)
        {
            if (line == string.Empty)
            {
                creatingRules = false;
                continue;
            }
            if (creatingRules)
            {
                var arr = line.Split('|');
                int @for = int.Parse(arr[0]);
                int comparedValue = int.Parse(arr[1]);
                rules.Add((@for, comparedValue));
            }
            else
            {
                var coll = line.Split(',').Select(int.Parse).ToArray();
                bool ruleBroken = false;
                for (int i = 0; i < coll.Length; i++)
                {
                    int @for = coll[i];
                    for (int j = i + 1; j < coll.Length; j++)
                    {
                        int comparedValue = coll[j];
                        if (rules.TryGetValue((comparedValue, @for), out _))
                        {
                            ruleBroken = true;
                            break;
                        }
                    }
                    if (ruleBroken)
                    {
                        break;
                    }
                }
                if (!ruleBroken)
                {
                    result += coll[coll.Length / 2];
                }
            }
        }


        return result;
    }

    public static int Solution_5_1(IEnumerable<string> input)
    {
        int result = 0;

        List<int[]> incorrectLines = new List<int[]>();
        HashSet<(int, int)> rules = new HashSet<(int, int)>();
        bool creatingRules = true;
        foreach (var line in input)
        {
            if (line == string.Empty)
            {
                creatingRules = false;
                continue;
            }
            if (creatingRules)
            {
                var arr = line.Split('|');
                int @for = int.Parse(arr[0]);
                int comparedValue = int.Parse(arr[1]);
                rules.Add((@for, comparedValue));
            }
            else
            {
                var coll = line.Split(',').Select(int.Parse).ToArray();
                bool ruleBroken = false;
                for (int i = 0; i < coll.Length; i++)
                {
                    int @for = coll[i];
                    for (int j = i + 1; j < coll.Length; j++)
                    {
                        int comparedValue = coll[j];
                        if (rules.TryGetValue((comparedValue, @for), out _))
                        {
                            ruleBroken = true;
                            break;
                        }
                    }
                    if (ruleBroken)
                    {
                        break;
                    }
                }
                if (ruleBroken)
                {
                    incorrectLines.Add(coll);
                }
            }
        }
        foreach (var line in incorrectLines)
        {
            List<int> correctOrder = new List<int>();
            for (int i = 0; i < line.Length; i++)
            {
                int number = line[i];
                int j = 0;
                for (; j < correctOrder.Count; j++)
                {
                    if (rules.TryGetValue((correctOrder[j], number), out _))
                    {
                        break;
                    }
                }
                correctOrder.Insert(j, number);
            }
            result += correctOrder[correctOrder.Count / 2];
        }


        return result;
    }

    public static int Solution_6_0(List<string> input)
    {
        var result = 0;

        (int x, int y) position = (0, 0);
        (int x, int y) vector = (0, -1);

        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '^')
                {
                    position = (x, y);
                    break;
                }
            }
            if (position != (0, 0))
                break;
        }

        bool outOfBounds = false;
        while (!outOfBounds)
        {
            var charArr = input[position.y].ToCharArray();
            charArr[position.x] = 'X';
            input[position.y] = new string(charArr);

            (int x, int y) targetPosition = (position.x + vector.x, position.y + vector.y);
            if (targetPosition.x >= input[0].Length
                || targetPosition.y >= input.Count
                || targetPosition.x < 0
                || targetPosition.y < 0)
            {
                outOfBounds = true;
            }
            else if (input[targetPosition.y][targetPosition.x] == '#')
            {
                vector = vector switch
                {
                    (0, -1) => (1, 0),
                    (1, 0) => (0, 1),
                    (0, 1) => (-1, 0),
                    (-1, 0) => (0, -1),
                    _ => (0, 0)
                };
            }
            else
            {
                position = targetPosition;
            }
        }

        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == 'X')
                {
                    result++;
                }
            }
        }

        return result;
    }

    public static int Solution_6_1(List<string> input)
    {
        (int x, int y) position = (0, 0);
        (int x, int y) vector = (0, -1);

        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '^')
                {
                    position = (x, y);
                    break;
                }
            }
            if (position != (0, 0))
                break;
        }

        var startPosition = (position.x, position.y);
        HashSet<(int x, int y)> customBlocks = new HashSet<(int x, int y)>();
        int i = 0;
        bool outOfBounds = false;
        while (!outOfBounds)
        {
            (int x, int y) tempPosition = (startPosition.x, startPosition.y);
            (int x, int y) tempVector = (0, -1);
            HashSet<(int x, int y, int vX, int vY)> entries = new HashSet<(int x, int y, int vX, int vY)>();
            bool isComplete = false;
            var customBlock = (position.x + vector.x, position.y + vector.y);
            while (!isComplete)
            {
                isComplete = Traverse_6(input, ref tempPosition, ref tempVector, customBlock, entries, out var isLoop);
                if (isLoop) customBlocks.Add(customBlock);
            }

            outOfBounds = Traverse_6(input, ref position, ref vector, (-1, -1), null, out _);
        }        

        return customBlocks.Count;
    }

    private static bool Traverse_6(List<string> input, ref (int x, int y) position, ref (int x, int y) vector, (int x, int y) customBlock, HashSet<(int x, int y, int vX, int vY)>? entries, out bool isLoop)
    {
        isLoop = false;
        (int x, int y) targetPosition = (position.x + vector.x, position.y + vector.y);
        if (targetPosition.x >= input[0].Length
            || targetPosition.y >= input.Count
            || targetPosition.x < 0
            || targetPosition.y < 0)
        {
            return true;
        }
        else if (input[targetPosition.y][targetPosition.x] == '#' || targetPosition == customBlock)
        {
            if (entries is not null)
            {
                var entry = (targetPosition.x, targetPosition.y, vector.x, vector.y);
                if (entries.Contains(entry))
                {
                    isLoop = true;
                    return true;
                }
                entries.Add(entry);
            }

            vector = vector switch
            {
                (0, -1) => (1, 0),
                (1, 0) => (0, 1),
                (0, 1) => (-1, 0),
                (-1, 0) => (0, -1),
                _ => (0, 0)
            };
        }
        else
        {
            position = targetPosition;
        }
        return false;
    }

    public static ulong Solution_7_0(List<string> input)
    {
        ulong result = 0;

        foreach (var line in input)
        {
            var arr = line.Split(' ');
            ulong expected = ulong.Parse(arr[0].Substring(0, arr[0].Length - 1));
            ulong[] sequence = arr[1..].Select(ulong.Parse).ToArray();
            if (ProcessTreeNode_7_0(sequence[0], 1, sequence, expected))
                result += expected;
        }

        return result;
    }
    private static bool ProcessTreeNode_7_0(ulong current, int index, ulong[] numbers, ulong expected)
    {
        if (current == expected)
            return true;
        if (index >= numbers.Length || current > expected)
            return false;

        if (ProcessTreeNode_7_0(current + numbers[index], index + 1, numbers, expected))
            return true;

        if (ProcessTreeNode_7_0(current * numbers[index], index + 1, numbers, expected))
            return true;

        return false;
    }

    public static int Solution_8_0(List<string> input)
    {
        int maxX = input[0].Length;
        int maxY = input.Count;

        Dictionary<char, List<(int x, int y)>> antennas = new Dictionary<char, List<(int x, int y)>>() {};
        HashSet<(int x, int y)> antinodes = new HashSet<(int x, int y)>();
        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] != '.')
                {
                    if (!antennas.TryGetValue(input[y][x], out var coll))
                    {
                        coll = new List<(int x, int y)>();
                        antennas[input[y][x]] = coll;
                    }
                    coll.Add((x, y));
                }
            }
        }
        foreach (var group in antennas.Values)
        {
            for (int i = 0; i < group.Count; i++)
            {
                var antennaA = group[i];
                for (int j = i + 1; j < group.Count; j++)
                {
                    var antennaB = group[j];

                    (int x, int y) vector = (antennaA.x - antennaB.x, antennaA.y - antennaB.y);

                    antinodes.Add((antennaA.x + vector.x, antennaA.y + vector.y));
                    antinodes.Add((antennaB.x - vector.x, antennaB.y - vector.y));
                }
            }
        }

        return antinodes.Where(x => x.x >= 0 && x.y >= 0 && x.x < maxX && x.y < maxY).Count();
    }

    public static int Solution_8_1(List<string> input)
    {
        int maxX = input[0].Length;
        int maxY = input.Count;

        Dictionary<char, List<(int x, int y)>> antennas = new Dictionary<char, List<(int x, int y)>>() { };
        HashSet<(int x, int y)> antinodes = new HashSet<(int x, int y)>();
        for (int y = 0; y < input.Count; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] != '.')
                {
                    if (!antennas.TryGetValue(input[y][x], out var coll))
                    {
                        coll = new List<(int x, int y)>();
                        antennas[input[y][x]] = coll;
                    }
                    coll.Add((x, y));
                }
            }
        }
        foreach (var group in antennas.Values)
        {
            for (int i = 0; i < group.Count; i++)
            {
                var antennaA = group[i];
                antinodes.Add(antennaA);
                for (int j = i + 1; j < group.Count; j++)
                {
                    var antennaB = group[j];

                    (int x, int y) vector = (antennaA.x - antennaB.x, antennaA.y - antennaB.y);
                    (int x, int y) currentPosition = (antennaA.x + vector.x, antennaA.y + vector.y);
                    while (currentPosition.x >= 0 && currentPosition.y >= 0 && currentPosition.x < maxX && currentPosition.y < maxY)
                    {
                        antinodes.Add(currentPosition);
                        currentPosition = (currentPosition.x + vector.x, currentPosition.y + vector.y);
                    }
                    currentPosition = (antennaB.x - vector.x, antennaB.y - vector.y);
                    while (currentPosition.x >= 0 && currentPosition.y >= 0 && currentPosition.x < maxX && currentPosition.y < maxY)
                    {
                        antinodes.Add(currentPosition);
                        currentPosition = (currentPosition.x - vector.x, currentPosition.y - vector.y);
                    }
                }
            }
        }

        return antinodes.Count();
    }

    public static long Solution_9_0(string input)
    {
        long result = 0;

        List<(int id, int startIndex, int endIndex)> entries = new List<(int id, int startIndex, int endIndex)>();

        int currentIndex = 0;
        int currentId = 0;
        bool isEmpty = false;
        foreach (char c in input)
        {
            int value = c switch
            {
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                _ => 0
            };
            int nextIndex = currentIndex + value;
            if (!isEmpty)
            {
                entries.Add((currentId, currentIndex, nextIndex - 1));
                currentId++;
            }

            currentIndex = nextIndex;
            isEmpty = !isEmpty;
        }
        (int id, int startIndex, int endIndex)? currentEntry = null;
        for (int i = 0; i < entries.Count - 1; i++)
        {            
            int space = entries[i + 1].startIndex - (entries[i].endIndex + 1);
            while (space != 0)
            {
                if (currentEntry == null)
                {
                    currentEntry = entries[^1];
                }
                int spaceRequired = currentEntry.Value.endIndex - currentEntry.Value.startIndex + 1;
                if (spaceRequired <= space)
                {
                    entries.RemoveAt(entries.Count - 1);
                    entries.Insert(i + 1, (currentEntry.Value.id, entries[i].endIndex + 1, entries[i].endIndex + spaceRequired));
                    i++;
                    space = space - spaceRequired;
                    currentEntry = null;
                }
                else
                {
                    currentEntry = (currentEntry.Value.id, currentEntry.Value.startIndex, currentEntry.Value.endIndex - space);
                    entries[^1] = currentEntry.Value;
                    entries.Insert(i + 1, (currentEntry.Value.id, entries[i].endIndex + 1, entries[i].endIndex + space));
                    space = 0;
                }
            }
        }
        foreach (var entry in entries)
        {
            for (int i = entry.startIndex; i <= entry.endIndex; i++)
            {
                result += entry.id * i;
            }
        }

        return result;
    }

    public static long Solution_9_1(string input)
    {
        long result = 0;

        List<(int id, int startIndex, int endIndex)> entries = new List<(int id, int startIndex, int endIndex)>();

        int currentIndex = 0;
        int currentId = 0;
        bool isEmpty = false;
        foreach (char c in input)
        {
            int value = c switch
            {
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                _ => 0
            };
            int nextIndex = currentIndex + value;
            if (!isEmpty)
            {
                entries.Add((currentId, currentIndex, nextIndex - 1));
                currentId++;
            }

            currentIndex = nextIndex;
            isEmpty = !isEmpty;
        }
        
        for (int i = entries.Count - 1; i >= 0;)
        {
            var entry = entries[i];
            int spaceRequired = entry.endIndex - entry.startIndex + 1;
            bool inserted = false;
            for (int j = 0; j < entries.Count - 1 && j < i; j++)
            {
                int space = entries[j + 1].startIndex - (entries[j].endIndex + 1);
                if (space >= spaceRequired)
                {
                    entries.RemoveAt(i);
                    entries.Insert(j + 1, (entry.id, entries[j].endIndex + 1, entries[j].endIndex + spaceRequired));
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                i--;
            }
        }
        foreach (var entry in entries)
        {
            for (int i = entry.startIndex; i <= entry.endIndex; i++)
            {
                result += entry.id * i;
            }
        }

        return result;
    }
}
