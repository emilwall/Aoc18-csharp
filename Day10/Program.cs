using System;
using System.IO;
using System.Linq;

namespace Day10
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static readonly Pos[] Parsed = ParseInput();

        static void Main(string[] args)
        {
            const int startIteration = 10000; // Earn some ms by starting at iteration 10000
            int height = int.MaxValue, prevHeight = int.MaxValue;
            UpdateGrid(startIteration);
            for (var i = startIteration; prevHeight >= height; i++)
            {
                prevHeight = height;
                UpdateGrid(1);
                height = Parsed.Select(pos => pos.Y).Max() - Parsed.Select(pos => pos.Y).Min();
                if (prevHeight < height)
                {
                    Console.WriteLine("Part 1:");
                    UpdateGrid(-1);
                    DisplayGrid();
                    Console.WriteLine("Part 2: " + i);
                }
            }
        }

        private class Pos
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int VelX { get; set; }
            public int VelY { get; set; }
        }

        private static Pos[] ParseInput()
        {
            return Input.Select(line =>
            {
                int n1 = line.IndexOf('<'), n2 = line.IndexOf(','), n3 = line.IndexOf('>'),
                    n4 = line.IndexOf("y=<", StringComparison.InvariantCulture) + 3,
                    n5 = n4 + line.Substring(n4).IndexOf(',');

                return new Pos
                {
                    X = int.Parse(line.Substring(n1 + 1, n2 - n1 - 1)),
                    Y = int.Parse(line.Substring(n2 + 2, n3 - n2 - 2)),
                    VelX = int.Parse(line.Substring(n4, n5 - n4)),
                    VelY = int.Parse(line.Substring(n5 + 1, line.Length - n5 - 2))
                };
            }).ToArray();
        }

        private static void UpdateGrid(int multiplier)
        {
            foreach (var pos in Parsed)
            {
                pos.X += pos.VelX * multiplier;
                pos.Y += pos.VelY * multiplier;
            }
        }

        private static void DisplayGrid()
        {
            var xMin = Parsed.Select(pos => pos.X).Min();
            var xMax = Parsed.Select(pos => pos.X).Max();
            var yMin = Parsed.Select(pos => pos.Y).Min();
            var yMax = Parsed.Select(pos => pos.Y).Max();
            var grid = new bool[xMax - xMin + 1, yMax - yMin + 1];
            foreach (var pos in Parsed)
            {
                grid[pos.X - xMin, pos.Y - yMin] = true;
            }

            Enumerable.Range(0, yMax - yMin + 1).ToList().ForEach(y =>
            {
                Console.WriteLine(string.Join("",
                    Enumerable.Range(0, xMax - xMin + 1).Select(x => grid[x, y] ? 'X' : ' ')));
            });
        }
    }
}
