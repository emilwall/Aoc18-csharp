using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var parsed = Input.Select(int.Parse).ToArray();
            Console.WriteLine("Part 1: " + parsed.Sum());

            var i = 0;
            var sum = 0;
            var seen = new HashSet<int>();
            while (!seen.Contains(sum))
            {
                seen.Add(sum);
                sum += parsed[i++ % parsed.Length];
            }
            Console.WriteLine("Part 2: " + sum);
        }
    }
}
