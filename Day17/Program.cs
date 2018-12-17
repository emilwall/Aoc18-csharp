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

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var grid = GetGrid();
            var waterTiles = grid.SelectMany(c => c).Count(c => new[] {'|', '~'}.Contains(c));
            var stillWater = grid.SelectMany(c => c).Count(c => c == '~');

            var finishTime = timer.ElapsedMilliseconds;
            PrintGrid(grid);
            Console.WriteLine($"Finished in {finishTime}ms");
            Console.WriteLine($"Part 1: {waterTiles}");
            Console.WriteLine($"Part 2: {stillWater}");
        }

        private static char[][] GetGrid()
        {
            var waterPos = 500;
            var clay = GetClayLocations();
            int xMin = clay.Min(c => c.Key) - 1,
                xMax = clay.Max(c => c.Key) + 1,
                yMin = clay.Min(c => c.Value.Min()),
                yMax = clay.Max(c => c.Value.Max());

            var grid = Enumerable.Range(yMin, yMax - yMin + 1).Select(y =>
                Enumerable.Range(xMin, xMax - xMin + 1).Select(x =>
                    clay.ContainsKey(x) && clay[x].Contains(y) ? '#' : '.'
                ).ToArray()
            ).ToArray();

            grid[0][waterPos - xMin] = '|';
            FindBottomAndFill(grid, (x: waterPos - xMin, y: 0));

            return grid;
        }

        private static void FindBottomAndFill(char[][] grid, (int x, int y) water)
        {
            while (water.y < grid.Length && grid[water.y][water.x] != '#')
            {
                grid[water.y++][water.x] = '|';
            }

            if (water.y < grid.Length)
            {
                int left, right;
                do
                {
                    grid[--water.y][water.x] = '~';
                    left = right = water.x;
                    while (left > -1
                           && (left == 0 || grid[water.y][left - 1] != '#')
                           && (water.y + 1 == grid.Length || new[] {'#', '~'}.Contains(grid[water.y + 1][left])))
                    {
                        grid[water.y][--left] = '~';
                    }

                    while (right < grid[0].Length + 1
                           && (right + 1 == grid[0].Length || grid[water.y][right + 1] != '#')
                           && (water.y + 1 == grid.Length || new[] {'#', '~'}.Contains(grid[water.y + 1][right])))
                    {
                        grid[water.y][++right] = '~';
                    }
                } while (left > 0
                         && right + 1 < grid[0].Length
                         && grid[water.y][left - 1] == '#'
                         && grid[water.y][right + 1] == '#');

                for (var i = left; i <= right; i++)
                {
                    grid[water.y][i] = '|';
                }

                if (grid[water.y + 1][left] == '.')
                {
                    FindBottomAndFill(grid, (left, water.y + 1));
                }

                if (grid[water.y + 1][right] == '.')
                {
                    FindBottomAndFill(grid, (right, water.y + 1));
                }
            }
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
                return line.StartsWith('x')
                    ? new[] { (x: first, ys: range) }
                    : range.Select(x => (x: x, ys: new List<int> { first }));
            }).GroupBy(pair => pair.x)
                .ToDictionary(
                    group => group.Key,
                    group => new HashSet<int>(group.SelectMany(pair => pair.ys))
                );
        }

        private static void PrintGrid(char[][] grid)
        {
            foreach (var row in grid)
            {
                Console.WriteLine(string.Join("", row));
            }
        }
    }
}
