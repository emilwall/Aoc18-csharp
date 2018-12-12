using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Day12
{
    class Program
    {
        static readonly bool IsTest = false;
        static readonly string[] Input = File.ReadAllLines(IsTest ? "example.txt" : "input.txt");
        private static long center = 4;

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

            for (long i = 20; i < 5000; i++)
            {
                state = new string(Enumerable.Range(2, state.Length - 4).Select(j => notes.Select(note => note(j, state)).FirstOrDefault(c => c != null) ?? '.').ToArray());
                state = AddPadding(state);
                if (i == 4999)
                {
                }
            }
            Console.WriteLine($"   5000: {state} (center at {center})");
            Console.WriteLine($"  50000: {state} (center at {center-45000})");
            Console.WriteLine($" 500000: {state} (center at {center-45000-450000})");
            Console.WriteLine($"5000000: {state} (center at {center-45000-450000-4500000})");
            // 4 - (-49999999896) + 5 - (-49999999896) + ... + 197 - (-49999999896)
            // = 49999999900 + 49999999901 + ... + 500000000093
            // = (50000000093 * 50000000094) / 2 - (49999999899 * 49999999900) / 2
            // = 9699999999321

            Console.WriteLine($"Part 1: {sum1}, time: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine($"Part 2: {9699999999321}, time: {timer.ElapsedMilliseconds}ms");
        }

        private static string AddPadding(string state)
        {
            var sb = new StringBuilder(state.Length + 8);
            var prefix = new string('.', 4 - Math.Min(state.IndexOf('#'), 4));
            sb.Append(prefix);
            sb.Append(state);
            sb.Append(new string('.', 4 - Math.Min(state.Length - state.LastIndexOf('#') - 1, 4)));
            center += prefix.Length - 2;
            return sb.ToString();
        }
    }
}
