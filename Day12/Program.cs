using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        static readonly bool IsTest = false;
        static readonly string[] Input = File.ReadAllLines(IsTest ? "example.txt" : "input.txt");
        private static int center = 4; 

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var state = "...." + Input.First().Substring(15) + "....";
            var notes = Input.Skip(2).Select<string, Func<int, string, char?>>(line =>
                (i, s) => line.Substring(0, 5) == s.Substring(i - 2, 5) ? (char?)line.ElementAt(9) : null
            ).ToList();

            Console.WriteLine($" 0: {state} (center at {center})");
            for (var i = 0; i < 20; i++)
            {
                state = new string(Enumerable.Range(2, state.Length - 4).Select(j => notes.Select(note => note(j, state)).FirstOrDefault(c => c != null) ?? '.').ToArray());
                state = AddPadding(state);
                Console.WriteLine($"{i+1,2}: {state} (center at {center})");
            }

            var sum1 = state.Select((c, i) => c == '#' ? i - center : 0).Sum();
            Console.WriteLine($"Part 1: {sum1}, time: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine($"Part 2: , time: {timer.ElapsedMilliseconds}ms");
        }

        private static string AddPadding(string state)
        {

            var prefix = new string('.', 4 - Math.Min(state.IndexOf('#'), 4));
            var suffix = new string('.', 4 - Math.Min(state.Length - state.LastIndexOf('#') - 1, 4));
            center += prefix.Length - 2;
            return prefix + state + suffix;
        }
    }
}
