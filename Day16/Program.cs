using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day16
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");
        private static int[] Reg = { 0, 0, 0, 0 };

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var samples = GetSamples();
            var sampleMatches = samples.ToDictionary(sample => sample.instruction, GetMatches);
            var ambiguityCount = sampleMatches.Count(matches => matches.Value.Count >= 3);

            var mapping = GetOpcodeMapping(sampleMatches);
            var testProgram = Input.Skip(3162).Select(line => line.Split(' ').Select(int.Parse).ToArray());
            foreach (var instruction in testProgram)
            {
                Reg = Operations[mapping[instruction[0]]](Reg, instruction);
            }

            Console.WriteLine($"Finished in {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part 1: {ambiguityCount}");
            Console.WriteLine($"Part 2: {Reg[0]}");
        }

        enum OpcodeName
        {
            addr, addi, mulr, muli, banr, bani, borr, bori,
            setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr
        }

        private static Func<int[], int[], int[]> Apply(Func<int[], int[], int> f) => (input, instr) =>
            input.Select((v, i) => i == instr[3] ? f(input, instr) : v).ToArray();

        private static readonly Dictionary<OpcodeName, Func<int[], int[], int[]>> Operations =
            new Dictionary<OpcodeName, Func<int[], int[], int[]>>
            {
                { OpcodeName.addr, Apply((input, instr) => input[instr[1]] + input[instr[2]]) },
                { OpcodeName.addi, Apply((input, instr) => input[instr[1]] + instr[2]) },
                { OpcodeName.mulr, Apply((input, instr) => input[instr[1]] * input[instr[2]]) },
                { OpcodeName.muli, Apply((input, instr) => input[instr[1]] * instr[2]) },
                { OpcodeName.banr, Apply((input, instr) => input[instr[1]] & input[instr[2]]) },
                { OpcodeName.bani, Apply((input, instr) => input[instr[1]] & instr[2]) },
                { OpcodeName.borr, Apply((input, instr) => input[instr[1]] | input[instr[2]]) },
                { OpcodeName.bori, Apply((input, instr) => input[instr[1]] | instr[2]) },
                { OpcodeName.setr, Apply((input, instr) => input[instr[1]]) },
                { OpcodeName.seti, Apply((input, instr) => instr[1]) },
                { OpcodeName.gtir, Apply((input, instr) => instr[1] > input[instr[2]] ? 1 : 0) },
                { OpcodeName.gtri, Apply((input, instr) => input[instr[1]] > instr[2] ? 1 : 0) },
                { OpcodeName.gtrr, Apply((input, instr) => input[instr[1]] > input[instr[2]] ? 1 : 0) },
                { OpcodeName.eqir, Apply((input, instr) => instr[1] == input[instr[2]] ? 1 : 0) },
                { OpcodeName.eqri, Apply((input, instr) => input[instr[1]] == instr[2] ? 1 : 0) },
                { OpcodeName.eqrr, Apply((input, instr) => input[instr[1]] == input[instr[2]] ? 1 : 0) }
            };

        private static Dictionary<int, OpcodeName> GetOpcodeMapping(Dictionary<int[], List<OpcodeName>> sampleMatches)
        {
            var matchLookup = sampleMatches.GroupBy(x => x.Key[0]).ToDictionary(group => group.Key, group =>
                new HashSet<OpcodeName>(group.SelectMany(x => x.Value)));

            while (matchLookup.Any(pair => pair.Value.Skip(1).Any()))
            {
                foreach (var done in matchLookup.Where(c => !c.Value.Skip(1).Any()))
                {
                    foreach (var remaining in matchLookup.Where(c => c.Key != done.Key))
                    {
                        remaining.Value.Remove(done.Value.Single());
                    }
                }
            }

            return matchLookup.ToDictionary(x => x.Key, x => x.Value.Single());
        }

        private static List<OpcodeName> GetMatches((int[] input, int[] instruction, int[] output) sample)
        {
            var (input, instr, output) = sample;
            return Operations.Where(pair => output.SequenceEqual(pair.Value(input, instr)))
                .Select(pair => pair.Key)
                .ToList();
        }

        private static IEnumerable<(int[] input, int[] instruction, int[] output)> GetSamples()
        {
            var samples = new List<(int[] input, int[] instruction, int[] output)>();
            for (var i = 0; Input[i].StartsWith("Before"); i += 4)
            {
                samples.Add((
                    Input[i].Substring(9, Input[i].Length - 10).Split(", ").Select(int.Parse).ToArray(),
                    Input[i + 1].Split(' ').Select(int.Parse).ToArray(),
                    Input[i + 2].Substring(9, Input[i + 2].Length - 10).Split(", ").Select(int.Parse).ToArray()));
            }

            return samples;
        }
    }
}
