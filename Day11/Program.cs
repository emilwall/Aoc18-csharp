using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var timer = Stopwatch.StartNew();
            var serialNumber = int.Parse(Input.First());
            var grid = Enumerable.Range(1, gridSize).AsParallel()
                .Select(x => Enumerable.Range(1, gridSize)
                    .Select(y => 
                    {
                        var rackId = x + 10;
                        var powerLevel = rackId * y;
                        powerLevel += serialNumber;
                        powerLevel *= rackId;
                        return powerLevel % 1000 / 100 - 5;
                    }).ToArray()
                ).ToArray();

            var squareSizes = Enumerable.Range(3, 30).AsParallel().ToDictionary(size => size, size => {
                int x = 1, largest = int.MinValue, prevTotal = int.MinValue;
                var targetPoint = new Point(1, 1);
                do
                {
                    int y = 1, total = prevTotal;
                    var p = new Point(x, y);
                    if (total == int.MinValue) {
                        total = Enumerable.Range(x, size).Select(x2 => Enumerable.Range(y, size).Sum(y2 => grid[x2-1][y2-1])).Sum();
                    }
                    else
                    {
                        total -= Enumerable.Range(p.Y, size).Sum(y2 => grid[x - 2][y2 - 1]);
                        total += Enumerable.Range(p.Y, size).Sum(y2 => grid[x + size - 2][y2 - 1]);
                    }
                    if (total > largest)
                    {
                        (targetPoint, largest) = (p, total);
                    }
                    prevTotal = total;
                    do
                    {
                        p = new Point(x, y + 1);
                        total -= Enumerable.Range(p.X, size).Sum(x2 => grid[x2 - 1][y - 1]);
                        total += Enumerable.Range(p.X, size).Sum(x2 => grid[x2 - 1][y + size - 1]);
                        if (total > largest)
                        {
                            (targetPoint, largest) = (p, total);
                        }
                    } while (++y < gridSize - size);
                } while (++x < gridSize - size);

                return (targetPoint, largest);
            });

            var max1 = squareSizes[3];
            Console.WriteLine($"Part 1: {max1.Item1.X},{max1.Item1.Y} (power: {max1.Item2}, time: {timer.ElapsedMilliseconds}ms)");

            var max2 = squareSizes.Aggregate((agg, s) => agg.Value.Item2 < s.Value.Item2 ? s : agg);
            Console.WriteLine($"Part 2: {max2.Value.Item1.X},{max2.Value.Item1.Y},{max2.Key} (power: {max2.Value.Item2}, time: {timer.ElapsedMilliseconds}ms)");
        }
    }
}
