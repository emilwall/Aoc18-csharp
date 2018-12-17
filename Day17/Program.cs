using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day17
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");

        private static Dictionary<char, char[]> Source;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var grid = GetGrid();
            var runningWater = grid.SelectMany(c => c).Count(c => c == '|');
            var stillWater = grid.SelectMany(c => c).Count(c => c == '~');

            var finishTime = timer.ElapsedMilliseconds;
            //Console.WriteLine(string.Join('\n', grid.Select(string.Concat<char>)));
            Console.WriteLine($"Finished in {finishTime}ms");
            Console.WriteLine($"Part 1: {stillWater + runningWater}");
            Console.WriteLine($"Part 2: {stillWater}");
        }

        private static char[][] GetGrid()
        {
            var waterPos = 500;
            var clay = GetClayLocations();
            int yMin = clay.Min(c => c.Key),
                yMax = clay.Max(c => c.Key),
                xMin = clay.Min(c => c.Value.Min()) - 1,
                xMax = clay.Max(c => c.Value.Max()) + 1;

            Source = new[] { '~', '|', '.', '#' }
                .ToDictionary(c => c, c => Enumerable.Repeat(c, yMax - yMin + 1).ToArray());

            var grid = Enumerable.Range(yMin, yMax - yMin + 1).ToList().AsParallel().Select(y =>
                clay.ContainsKey(y)
                    ? Enumerable.Range(xMin, xMax - xMin + 1)
                        .Select(x => clay[y].Contains(x) ? '#' : '.')
                        .ToArray()
                    : Source['.']).ToArray();
            grid[0][waterPos - xMin] = '|';
            FindBottomAndFill(grid, (x: waterPos - xMin, y: 0));

            return grid;
        }

        private static Dictionary<int, HashSet<int>> GetClayLocations()
        {
            return Input.SelectMany(line =>
                {
                    int end1 = line.IndexOf(','), end2 = line.IndexOf('.'),
                        first = int.Parse(line.Substring(2, end1 - 2)),
                        second = int.Parse(line.Substring(end1 + 4, end2 - end1 - 4)),
                        third = int.Parse(line.Substring(end2 + 2));
                    var range = Enumerable.Range(second, third - second + 1).ToList();
                    return line.StartsWith('y')
                        ? new[] { (y: first, xs: range) }
                        : range.Select(y => (y: y, xs: new List<int> { first }));
                }).GroupBy(pair => pair.y)
                .ToDictionary(
                    group => group.Key,
                    group => new HashSet<int>(group.SelectMany(pair => pair.xs))
                );
        }

        private static void FindBottomAndFill(char[][] grid, (int x, int y) water)
        {
            while (water.y < grid.Length && grid[water.y][water.x] != '#')
            {
                grid[water.y++][water.x] = '|';
            }

            if (water.y >= grid.Length)
            {
                return;
            }

            var (left, right) = Fill(grid, ref water);

            if (grid[water.y + 1][left] == '.')
            {
                FindBottomAndFill(grid, (left, water.y + 1));
            }

            if (grid[water.y + 1][right] == '.')
            {
                FindBottomAndFill(grid, (right, water.y + 1));
            }
        }

        private static (int, int) Fill(char[][] grid, ref (int x, int y) water)
        {
            int left, right;
            do
            {
                water.y--;
                left = right = water.x;
                while (left >= 0
                       && (left == 0 || grid[water.y][left - 1] != '#')
                       && grid[water.y + 1][left] != '.'
                       && grid[water.y + 1][left] != '|')
                {
                    left--;
                }

                while (right <= grid[0].Length
                       && (right == grid[0].Length || grid[water.y][right + 1] != '#')
                       && grid[water.y + 1][right] != '.'
                       && grid[water.y + 1][right] != '|')
                {
                    right++;
                }

                if (left <= 0
                    || right + 1 >= grid[0].Length
                    || grid[water.y][left - 1] != '#'
                    || grid[water.y][right + 1] != '#')
                {
                    break;
                }

                SetArraySegment(grid[water.y], left, right, '~');
            } while (true);

            SetArraySegment(grid[water.y], left, right, '|');

            return (left, right);
        }

        private static void SetArraySegment(char[] grid, int begin, int end, char c)
        {
            Array.Copy(Source[c], 0, grid, begin, end - begin + 1);
        }
    }
}
