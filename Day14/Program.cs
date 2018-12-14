using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day14
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");
        static readonly int NoOfRecipes = int.Parse(Input.First());

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var recipes = new List<int>(30000000) { 3, 7 };
            var targetSequence = Input.First().Select(c => int.Parse(c.ToString())).ToList();
            int elf1Pos = 0, elf2Pos = 1, sequenceIndex = -1, maxCount = NoOfRecipes + 10, size = targetSequence.Count;
            while (recipes.Count < maxCount || sequenceIndex == -1)
            {
                int elf1Score = recipes[elf1Pos], elf2Score = recipes[elf2Pos], sum = elf1Score + elf2Score;
                if (sum > 9)
                {
                    recipes.Add(sum / 10);
                    sequenceIndex = TryGetSequenceIndex(recipes, targetSequence, size);
                    if (sequenceIndex != -1)
                    {
                        break;
                    }
                }
                recipes.Add(sum % 10);
                sequenceIndex = TryGetSequenceIndex(recipes, targetSequence, size);

                elf1Pos = (elf1Pos + elf1Score + 1) % recipes.Count;
                elf2Pos = (elf2Pos + elf2Score + 1) % recipes.Count;
            }

            var firstSequence = string.Join("", recipes.Skip(NoOfRecipes).Take(10));
            Console.WriteLine($"Completed: {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part 1: {firstSequence}");
            Console.WriteLine($"Part 2: {sequenceIndex}");
        }

        private static int TryGetSequenceIndex(List<int> recipes, List<int> targetSequence, int size)
        {
            var startIndex = recipes.Count - size;
            if (startIndex > 0
                && recipes[startIndex] == targetSequence[0]
                && recipes[startIndex + 1] == targetSequence[1])
            {
                var sequence = recipes.GetRange(startIndex, size);
                if (sequence.SequenceEqual(targetSequence))
                {
                    return startIndex;
                }
            }

            return -1;
        }
    }
}
