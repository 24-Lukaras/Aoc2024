

using Aoc2024;

using (StreamReader input = new StreamReader(@"C:\Users\Lukin\Documents\temp\aoc2024\17_test.txt"))
{    
    List<string> lines = new List<string>();
    string line;
    while ((line = input.ReadLine()) is not null)
    {
        lines.Add(line);
    }    
    var solution = Solutions.Solution_17_1(lines);
}
