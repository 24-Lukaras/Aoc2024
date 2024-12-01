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
}
