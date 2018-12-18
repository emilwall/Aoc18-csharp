using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day18
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");
        private static readonly char[][] Even = Input.Select(line => line.ToCharArray()).ToArray();
        private static readonly char[][] Odd = Input.Select(line => line.ToCharArray()).ToArray();
        private static int minute;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Execute(10);
            var res1 = Even.SelectMany(c => c).Count(c => c == '|') *
                       Even.SelectMany(c => c).Count(c => c == '#');

            Execute(608); // Found through manual inspection and some math, will differ depending on input
            var res2 = Even.SelectMany(c => c).Count(c => c == '|') *
                       Even.SelectMany(c => c).Count(c => c == '#');

            var finishTime = timer.ElapsedMilliseconds;
            Visualize(Odd);
            Console.WriteLine($"Finished in {finishTime}ms");
            Console.WriteLine($"Part 1: {res1}");
            Console.WriteLine($"Part 2: {res2}");
        }

        private static void Visualize(char[][] grid)
        {
            Console.WriteLine(string.Join('\n', grid.Select(string.Concat<char>)));
        }

        private static void Execute(int targetMinute)
        {
            while (minute++ <= targetMinute)
            {
                var next = minute % 2 == 0 ? Even : Odd;
                var prev = next == Even ? Odd : Even;
                for (var i = 0; i < next.Length; i++)
                {
                    for (var k = 0; k < next[0].Length; k++)
                    {
                        next[i][k] = Evolve(prev, i, k);
                    }
                }
            }
        }

        private static char Evolve(char[][] prev, int i, int k)
        {
            switch (prev[i][k])
            {
                case '.': return Count('|', prev, i, k) >= 3 ? '|' : '.';
                case '|': return Count('#', prev, i, k) >= 3 ? '#' : '|';
                case '#': return Count('#', prev, i, k) >= 1 &&
                                 Count('|', prev, i, k) >= 1 ? '#' : '.';
                default: throw new ArgumentException($"invalid: {prev[i][k]}");
            }
        }

        private static int Count(char c, char[][] prev, int i, int k)
        {
            var count = 0;

            if (i > 0 && k > 0)
            {
                count += prev[i - 1][k - 1] == c ? 1 : 0;
            }

            if (i > 0)
            {
                count += prev[i - 1][k] == c ? 1 : 0;
            }

            if (k > 0)
            {
                count += prev[i][k - 1] == c ? 1 : 0;
            }

            if (i < prev.Length - 1 && k < prev[0].Length - 1)
            {
                count += prev[i + 1][k + 1] == c ? 1 : 0;
            }

            if (i < prev.Length - 1)
            {
                count += prev[i + 1][k] == c ? 1 : 0;
            }

            if (k < prev[0].Length - 1)
            {
                count += prev[i][k + 1] == c ? 1 : 0;
            }

            if (i > 0 && k < prev[0].Length - 1)
            {
                count += prev[i - 1][k + 1] == c ? 1 : 0;
            }

            if (k > 0 && i < prev.Length - 1)
            {
                count += prev[i + 1][k - 1] == c ? 1 : 0;
            }

            return count;
        }
    }
}
