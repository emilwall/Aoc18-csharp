using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day17
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("example.txt");

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var waterPos = 500;
            var clay = GetClayLocations();
            int xMin = clay.Min(c => c.x.Min()) - 1, xMax = clay.Max(c => c.x.Max()) + 1,
                yMin = clay.Min(c => c.y.Min()), yMax = clay.Max(c => c.y.Max());

            var grid = Enumerable.Range(yMin, yMax - yMin + 1).Select(y =>
                Enumerable.Range(xMin, xMax - xMin + 1).Select(x =>
                    clay.Any(c => c.x.Contains(x) && c.y.Contains(y)) ? '#' : '.'
                ).ToArray()
            ).ToArray();

            grid[0][waterPos - xMin] = '|';
            FindBottomAndFill(grid, (x: waterPos - xMin, y: 0));
            var waterTiles = grid.SelectMany(c => c).Count(c => new[] {'|', '~'}.Contains(c));

            Console.WriteLine($"Finished in {timer.ElapsedMilliseconds}ms");
            PrintGrid(grid);
            Console.WriteLine($"Part 1: {waterTiles}");
            Console.WriteLine($"Part 2: {0}");
        }

        private static void FindBottomAndFill(char[][] grid, (int x, int y) water)
        {
            var initial = water.y;
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
                    left = water.x;
                    while (grid[water.y][left - 1] != '#' && new[] {'#', '~'}.Contains(grid[water.y + 1][left]))
                    {
                        grid[water.y][--left] = '~';
                    }

                    right = water.x;
                    while (grid[water.y][right + 1] != '#' && new[] {'#', '~'}.Contains(grid[water.y + 1][right]))
                    {
                        grid[water.y][++right] = '~';
                    }
                } while (grid[water.y][left - 1] == '#' && grid[water.y][right + 1] == '#');

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

        private static List<(List<int> x, List<int> y)> GetClayLocations()
        {
            return Input.Select(line =>
            {
                int xStart = 2, xEnd = line.IndexOf(','),
                    y1Start = xEnd + 4, y1End = line.IndexOf('.'),
                    y2Start = y1End + 2;
                var x = int.Parse(line.Substring(xStart, xEnd - xStart));
                var y1 = int.Parse(line.Substring(y1Start, y1End - y1Start));
                var y2 = int.Parse(line.Substring(y2Start));
                var single = Enumerable.Range(x, 1).ToList();
                var range = Enumerable.Range(y1, y2 - y1 + 1).ToList();
                return line.StartsWith('x') ? (single, range) : (range, single);
            }).ToList();
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
