using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day6
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static readonly Point[] Points = Input
            .Select(line => line.Split(", ").Select(int.Parse).ToList())
            .Select(p => new Point(p.First(), p.Last()))
            .ToArray();

        static void Main(string[] args)
        {
            var xMin = Points.Select(p => p.X).Min();
            var xMax = Points.Select(p => p.X).Max();
            var yMin = Points.Select(p => p.Y).Min();
            var yMax = Points.Select(p => p.Y).Max();

            var grid = Enumerable.Range(xMin, xMax)
                .Select(x => Enumerable.Range(yMin, yMax)
                    .Select(y => CalculateClosest(x, y))
                    .ToArray())
                .ToArray();
            var filteredGrid = GetCellsNotTouchingEdges(grid);
            var largestArea = CalculateLargestArea(filteredGrid);
            Console.WriteLine("Part 1: " + largestArea);

            var totalDistances = Enumerable.Range(xMin, xMax)
                .Select(x => Enumerable.Range(yMin, yMax)
                    .Select(y => CalculateTotalDistance(x, y))
                    .ToArray())
                .ToArray();
            var areaSize = totalDistances.SelectMany(d => d).Count(d => d < 10000);
            Console.WriteLine("Part 2: " + areaSize);
        }

        private static int CalculateClosest(int x, int y)
        {
            var distances = new int[Points.Length];
            for (var i = 0; i < distances.Length; i++)
            {
                distances[i] = CalculateManhattan(Points[i].X, Points[i].Y, x, y);
            }

            var smallest = distances.Min();
            var multiple = distances.Count(d => d == smallest) > 1;
            return multiple ? -1 : Array.IndexOf(distances, smallest);
        }

        private static IEnumerable<int> GetCellsNotTouchingEdges(int[][] grid)
        {
            var cellsTouchingEdges = new HashSet<int>();
            for (var x = 0; x < grid.Length; x++)
            {
                foreach (var y in new[] { 0, grid[x].Length - 1 })
                {
                    cellsTouchingEdges.Add(grid[x][y]);
                }
            }
            for (var y = 0; y < grid[0].Length; y++)
            {
                foreach (var x in new[] { 0, grid.Length - 1 })
                {
                    cellsTouchingEdges.Add(grid[x][y]);
                }
            }

            return grid.SelectMany(c => c).Where(c => !cellsTouchingEdges.Contains(c));
        }

        private static int CalculateLargestArea(IEnumerable<int> grid)
        {
            return grid.Where(c => c != -1)
                .GroupBy(x => x)
                .Select(g => g.Count())
                .Max();
        }

        private static int CalculateTotalDistance(int x, int y)
        {
            return Points.Sum(pair => CalculateManhattan(pair.X, pair.Y, x, y));
        }

        private static int CalculateManhattan(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        }
    }
}
