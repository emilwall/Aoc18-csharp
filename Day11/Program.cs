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
            var serialNumber = int.Parse(Input.First());
            var grid = Enumerable.Range(1, 300)
                .Select(x => Enumerable.Range(1, 300)
                    .Select(y => new Point(x, y))
                    .ToArray())
                .ToArray();

            var powers = new Dictionary<Point, int>();
            for (var i = 0; i < grid.Length - 2; i++)
            {
                for (var k = 0; k < grid[i].Length - 2; k++)
                {
                    var square = new[]
                    {
                        grid[i][k], grid[i + 1][k], grid[i + 2][k],
                        grid[i][k + 1], grid[i + 1][k + 1], grid[i + 2][k + 1],
                        grid[i][k + 2], grid[i + 1][k + 2], grid[i + 2][k + 2]
                    };
                    var powerLevels = square.Select(p =>
                    {
                        var rackId = p.X + 10;
                        var powerLevel = rackId * p.Y;
                        powerLevel += serialNumber;
                        powerLevel *= rackId;
                        return powerLevel % 1000 / 100 - 5;
                    });
                    powers.Add(grid[i][k], powerLevels.Sum());
                }
            }

            var max = powers.Max(p => p.Value);
            var item = powers.First(p => p.Value == max);
            Console.WriteLine($"Part 1: {item.Key.X},{item.Key.Y}");

            Console.WriteLine("Part 2: ");
        }
    }
}
