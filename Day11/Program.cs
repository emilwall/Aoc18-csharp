using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day11
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            const int gridSize = 300;
            var serialNumber = int.Parse(Input.First());
            var grid = Enumerable.Range(1, gridSize)
                .SelectMany(x => Enumerable.Range(1, gridSize)
                    .Select(y => new Point(x, y))
                    .ToArray())
                .ToArray();

            var powerLevels = grid.ToDictionary(p => p, p =>
            {
                var rackId = p.X + 10;
                var powerLevel = rackId * p.Y;
                powerLevel += serialNumber;
                powerLevel *= rackId;
                return powerLevel % 1000 / 100 - 5;
            });

            var powers = new Dictionary<Point, List<int>>();
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    var p = grid[i * gridSize + j];
                    powers[p] = new List<int> { powerLevels[p] };
                    var indexBounds = Math.Min(gridSize - i, gridSize - j);
                    var maxSquareSize = Math.Min(30, indexBounds); // Law of large numbers
                    for (var k = 1; k < maxSquareSize; k++)
                    {
                        powers[p].Add(
                            powers[p].Last()
                            + Enumerable.Range(0, k + 1).Sum(r => powerLevels[new Point(p.X + k, p.Y + r)])
                            + Enumerable.Range(0, k + 1).Sum(r => powerLevels[new Point(p.X + r, p.Y + k)])
                            - powerLevels[new Point(p.X + k, p.Y + k)]
                        );
                    }
                }

                if (i % 20 == 0)
                {
                    Console.Write($"Completed: {i}/{gridSize}  ");
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            }

            var max1 = powers.Max(p => p.Value.Count > 2 ? p.Value[2] : 0);
            var item1 = powers.First(p => p.Value.Count > 2 && p.Value[2] == max1);
            Console.WriteLine($"Part 1: {item1.Key.X},{item1.Key.Y} (power: {max1})");

            var max2 = powers.Max(p => p.Value.Max());
            var item2 = powers.First(p => p.Value.Any(power => power == max2));
            var itemIndex = item2.Value.FindIndex(power => power == max2) + 1;
            Console.WriteLine($"Part 2: {item2.Key.X},{item2.Key.Y},{itemIndex} (power: {max2})");
        }
    }
}
