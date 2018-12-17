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

        private static readonly HashSet<(int x, int y)> RunningWater = new HashSet<(int x, int y)>();
        private static readonly HashSet<(int x, int y)> StillWater = new HashSet<(int x, int y)>();
        private static Dictionary<int, List<(int left, int right)>> _clayLocations;
        private static int _yMax, _xMax, _yMin = int.MaxValue, _xMin = int.MaxValue;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Execute();

            var finishTime = timer.ElapsedMilliseconds;
            //Visualize();
            Console.WriteLine($"Finished in {finishTime}ms");
            Console.WriteLine($"Part 1: {StillWater.Count + RunningWater.Count}");
            Console.WriteLine($"Part 2: {StillWater.Count}");
        }

        private static void Visualize()
        {
            var grid = Enumerable.Range(_yMin, _yMax - _yMin + 1).Select(y =>
                Enumerable.Range(_xMin, _xMax - _xMin + 1).Select(x =>
                    RunningWater.Contains((x, y)) ? '|'
                    : StillWater.Contains((x, y)) ? '~'
                    : _clayLocations.ContainsKey(y) && _clayLocations[y]
                          .Any(range => x >= range.left && x <= range.right) ? '#' : '.'
                ).ToArray()
            ).ToArray();
            Console.WriteLine(string.Join('\n', grid.Select(string.Concat<char>)));
        }

        private static void Execute()
        {
            _clayLocations = GetClayLocations();
            _xMin--;
            _xMax++;

            FindBottomAndFill((x: 500, y: _yMin));
            RunningWater.ExceptWith(StillWater);
        }

        private static Dictionary<int, List<(int left, int right)>> GetClayLocations()
        {
            return Input.AsParallel().SelectMany(line =>
            {
                int end1 = line.IndexOf(','), end2 = line.IndexOf('.'),
                    first = int.Parse(line.Substring(2, end1 - 2)),
                    second = int.Parse(line.Substring(end1 + 4, end2 - end1 - 4)),
                    third = int.Parse(line.Substring(end2 + 2));
                var range = Enumerable.Range(second, third - second + 1).ToList();
                if (line.StartsWith('y'))
                {
                    _yMin = Math.Min(first, _yMin);
                    _yMax = Math.Max(first, _yMax);
                    _xMin = Math.Min(second, _xMin);
                    _xMax = Math.Max(third, _xMax);
                    return new[] {(y: first, xs: (second, third))};
                }
                _xMin = Math.Min(first, _xMin);
                _xMax = Math.Max(first, _xMax);
                _yMin = Math.Min(second, _yMin);
                _yMax = Math.Max(third, _yMax);
                return range.Select(y => (y: y, xs: (first, first)));
            }).GroupBy(pair => pair.y)
            .ToDictionary(
                group => group.Key,
                group => group.Select(pair => pair.xs).ToList()
            );
        }

        private static void FindBottomAndFill((int x, int y) water)
        {
            var initialY = Math.Max(water.y, _yMin);
            while (water.y <= _yMax
                   && (!_clayLocations.ContainsKey(water.y)
                    || !_clayLocations[water.y].Any(range => water.x >= range.left && water.x <= range.right)))
            {
                water.y++;
            }
            RunningWater.UnionWith(Enumerable.Range(initialY, water.y - initialY).Select(i => (water.x, i)));

            if (water.y > _yMax)
            {
                return;
            }

            var (left, right, y) = Fill(water);

            if ((!_clayLocations.ContainsKey(y)
                 || !_clayLocations[y].Any(range => left >= range.left && left <= range.right))
                && !StillWater.Contains((left, y))
                && !RunningWater.Contains((left, y)))
            {
                FindBottomAndFill((left, y));
            }

            if ((!_clayLocations.ContainsKey(y) || !_clayLocations[y]
                     .Any(range => right >= range.left && right <= range.right))
                && !StillWater.Contains((right, y))
                && !RunningWater.Contains((right, y)))
            {
                FindBottomAndFill((right, y));
            }
        }

        private static (int, int, int) Fill((int x, int y) water)
        {
            int left, right, y;
            do
            {
                water.y--;
                left = right = water.x;
                var rowClayLocations = _clayLocations.ContainsKey(water.y) ? _clayLocations[water.y] : null;
                while (left >= 0
                       && (!rowClayLocations?.Any(range => left > range.left && left - 1 <= range.right) ?? true)
                       && (StillWater.Contains((left, water.y + 1))
                           || (_clayLocations.ContainsKey(water.y + 1)
                               && _clayLocations[water.y + 1]
                                   .Any(range => left >= range.left && left <= range.right))))
                {
                    left--;
                }

                while (right <= _xMax
                       && (!rowClayLocations?.Any(range => right + 1 >= range.left && right < range.right) ?? true)
                       && (StillWater.Contains((right, water.y + 1))
                           || (_clayLocations.ContainsKey(water.y + 1)
                               && _clayLocations[water.y + 1]
                                   .Any(range => right >= range.left && right <= range.right))))
                {
                    right++;
                }

                if (left <= 0
                    || right + 1 >= _xMax
                    || (!rowClayLocations?.Any(range => left > range.left && left - 1 <= range.right) ?? true)
                    || !rowClayLocations.Any(range => right + 1 >= range.left && right < range.right))
                {
                    break;
                }

                y = water.y;
                StillWater.UnionWith(Enumerable.Range(left, right - left + 1).Select(x => (x, y)));
                RunningWater.Remove((water.x, y));
            } while (true);

            y = water.y;
            RunningWater.UnionWith(Enumerable.Range(left, right - left + 1).Select(x => (x, y)));

            return (left, right, ++y);
        }
    }
}
