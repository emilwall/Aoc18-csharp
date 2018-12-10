using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    class Program
    {
        static readonly string Input = File.ReadAllText("input.txt");

        static void Main(string[] args)
        {
            var processedInput = ProcessInput(Input);
            Console.WriteLine("Part 1: " + processedInput.Count);

            var shortest = Enumerable.Range('a', 'z' - 'a')
                .Select(c => processedInput.Where(i => i != c && i != char.ToUpper((char)c)))
                .Min(i => ProcessInput(i).Count);
            Console.WriteLine("Part 2: " + shortest);
        }

        private static List<char> ProcessInput(IEnumerable<char> input)
        {
            var result = input.ToList();
            for (var i = 0; i < result.Count - 1; i++)
            {
                var c = result[i];
                var next = result[i + 1];
                if (c != next && char.ToLower(c) == char.ToLower(next))
                {
                    result.RemoveRange(i, 2);
                    i = Math.Max(i - 2, -1);
                }
            }
            return result;
        }
    }
}
