using System;
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

            var parsed = Input.Select(line => line).ToList();

            Console.WriteLine($"Finished in {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part 1: {0}");
            Console.WriteLine($"Part 2: {0}");
        }
    }
}
