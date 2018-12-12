using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        static readonly bool IsTest = true;
        static readonly string[] Input = File.ReadAllLines(IsTest ? "example.txt" : "input.txt");

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var state = "...." + Input.First().Substring(16) + "....";
            var notes = Input.Skip(2).Select<string, Func<int, string, char?>>(line =>
                (i, s) => line.Substring(0, 5) == s.Substring(i - 2, 5) ? (char?)line.ElementAt(9) : null
            ).ToList();

            for (var i = 0; i < 30; i++)
            {
                state = new string(Enumerable.Range(2, state.Length - 4).Select(j => notes.Select(note => note(j, state)).FirstOrDefault(c => c != null) ?? '.').ToArray());
                // TODO Ensure minimum two . at both ends and keep track of indices
                Console.WriteLine(state);
            }

            Console.WriteLine($"Part 1: , time: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine($"Part 2: , time: {timer.ElapsedMilliseconds}ms");
        }
    }
}
