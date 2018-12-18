using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day18
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");
        private static char[][] _grid = Input.Select(line => line.ToCharArray()).ToArray();
        private static int _minute;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Execute(10);
            var res1 = GetResourceValue();

            TravelInTime();
            Execute(1000000000);
            var res2 = GetResourceValue();

            var finishTime = timer.ElapsedMilliseconds;
            Visualize(_grid);
            Console.WriteLine($"Finished in {finishTime}ms");
            Console.WriteLine($"Part 1: {res1}");
            Console.WriteLine($"Part 2: {res2}");
        }

        private static void TravelInTime()
        {
            var foundSoFar = new Dictionary<int, char[][]>();
            var prevValue = GetResourceValue();
            while (!foundSoFar.ContainsKey(prevValue) ||
                   !_grid.Select((row, i) => row.SequenceEqual(foundSoFar[prevValue][i])).All(x => x))
            {
                foundSoFar[prevValue] = _grid;
                Execute();
                prevValue = GetResourceValue();
            }

            var configurations = new List<char[][]>();
            var newValue = -1;
            while (newValue != prevValue)
            {
                configurations.Add(_grid);
                Execute();
                newValue = GetResourceValue();
            }

            _minute = 999999999 - ++_minute % configurations.Count;
        }

        private static int GetResourceValue()
        {
            return _grid.SelectMany(c => c).Count(c => c == '|') *
                   _grid.SelectMany(c => c).Count(c => c == '#');
        }

        private static void Visualize(char[][] grid)
        {
            Console.WriteLine(string.Join('\n', grid.Select(string.Concat<char>)));
        }

        private static void Execute(int targetMinute = 0)
        {
            do
            {
                _grid = _grid.AsParallel().Select((row, i) =>
                    row.Select((c, k) =>
                        Evolve(_grid, i, k)
                    ).ToArray()
                ).ToArray();
            } while (++_minute < targetMinute);
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
