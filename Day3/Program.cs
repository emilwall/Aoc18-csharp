using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var rectangles = input.Select(line =>
            {
                var xIndex = line.IndexOf('@') + 2;
                var yIndex = line.IndexOf(',') + 1;
                var wIndex = line.IndexOf(':') + 2;
                var hIndex = line.IndexOf('x') + 1;
                var x = int.Parse(line.Substring(xIndex, line.IndexOf(',') - xIndex));
                var y = int.Parse(line.Substring(yIndex, line.IndexOf(':') - yIndex));
                var w = int.Parse(line.Substring(wIndex, line.IndexOf('x') - wIndex));
                var h = int.Parse(line.Substring(hIndex, line.Length - hIndex));
                return new Rectangle(x, y, w, h);
            }).ToList();

            var points = rectangles.SelectMany(GetPoints).ToList();
            var overlappingCount = points.GroupBy(p => p).Count(group => group.Count() > 1);
            Console.WriteLine("Part 1: " + overlappingCount);

            var noOverlap = rectangles.FindIndex(r => HasNoIntersection(rectangles, r)) + 1;
            Console.WriteLine("Part 2: " + noOverlap);
        }

        private static IEnumerable<Point> GetPoints(Rectangle r)
        {
            return Enumerable.Range(r.Left, r.Width)
                .SelectMany(x => Enumerable.Range(r.Top, r.Height)
                    .Select(y => new Point(x, y)));
        }

        private static bool HasNoIntersection(IEnumerable<Rectangle> rectangles, Rectangle r)
        {
            return rectangles.All(r2 => r2 == r || !r2.IntersectsWith(r));
        }
    }
}
