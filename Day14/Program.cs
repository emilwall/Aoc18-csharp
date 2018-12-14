using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day14
{
    class Program
    {
        private const int Input = 540391;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var recipes = new List<int>(30000000) { 3, 7 };
            var targetSequence = Input.ToString().Select(c => c - '0').ToList();
            int pos1 = 0, pos2 = 1, sequenceIndex = -1, size = targetSequence.Count;
            while (sequenceIndex < 0 || recipes.Count < Input + 10)
            {
                int score1 = recipes[pos1], score2 = recipes[pos2], sum = score1 + score2, startIndex = recipes.Count - size;
                if (sum > 9)
                {
                    recipes.Add(sum / 10);
                    if (++startIndex >= 0
                        && recipes[startIndex] == targetSequence[0]
                        && recipes[startIndex + 1] == targetSequence[1])
                    {
                        var sequence = recipes.GetRange(startIndex, size);
                        if (sequence.SequenceEqual(targetSequence))
                        {
                            sequenceIndex = startIndex;
                        }
                    }
                }

                recipes.Add(sum % 10);
                if (++startIndex >= 0
                    && recipes[startIndex] == targetSequence[0]
                    && recipes[startIndex + 1] == targetSequence[1])
                {
                    var sequence = recipes.GetRange(startIndex, size);
                    if (sequence.SequenceEqual(targetSequence))
                    {
                        sequenceIndex = startIndex;
                    }
                }

                pos1 = (pos1 + score1 + 1) % recipes.Count;
                pos2 = (pos2 + score2 + 1) % recipes.Count;
            }

            var firstSequence = string.Join("", recipes.GetRange(Input, 10));
            Console.WriteLine($"Completed: {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part 1: {firstSequence}");
            Console.WriteLine($"Part 2: {sequenceIndex}");
        }
    }
}
