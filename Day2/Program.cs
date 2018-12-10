using System;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");
        static void Main(string[] args)
        {
            var twos = Input.Count(s => HasRepetition(s, 2));
            var threes = Input.Count(s => HasRepetition(s, 3));
            Console.WriteLine("Part 1: " + twos * threes);

            var commonLetters = Input.Select(MostCommon).OrderByDescending(s => s.Length).First();
            Console.WriteLine("Part 2: " + commonLetters);
        }

        private static bool HasRepetition(string s, int count)
        {
            return s.Any(c => s.Count(c2 => c2 == c) == count);
        }

        private static string MostCommon(string s)
        {
            return Input.Where(s2 => s2 != s)
                .Select(s2 => string.Join("", s.Select((c, i) => s2[i] == c ? c.ToString() : string.Empty)))
                .OrderByDescending(s3 => s3.Length)
                .First();
        }
    }
}
