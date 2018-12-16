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

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var samples = GetSamples();
            var sampleMatches = samples.ToDictionary(sample => sample.instruction, GetMatches);
            var ambiguityCount = sampleMatches.Count(matches => matches.Value.Count >= 3);

            Console.WriteLine($"Part 1: {ambiguityCount} ({timer.ElapsedMilliseconds}ms)");

            var operations = GetOpcodes(sampleMatches).ToDictionary(x => x.Key, x => x.Value.Single());
            var testProgram = GetTestProgram();
            var result = Execute(testProgram, operations);

            Console.WriteLine($"Part 2: {result[0]} ({timer.ElapsedMilliseconds}ms)");
        }

        private static int[] Execute(List<int[]> testProgram, Dictionary<int, OpcodeName> operations)
        {
            var input = new[] { 0, 0, 0, 0 };
            foreach (var instruction in testProgram)
            {
                input = Execute(instruction, input, operations);
            }

            return input;
        }

        private static int[] Execute(int[] instruction, int[] input, Dictionary<int, OpcodeName> operations)
        {
            switch (operations[instruction[0]])
            {
                case OpcodeName.addr:
                    return Addr(input, instruction);
                case OpcodeName.addi:
                    return Addi(input, instruction);
                case OpcodeName.mulr:
                    return Mulr(input, instruction);
                case OpcodeName.muli:
                    return Muli(input, instruction);
                case OpcodeName.banr:
                    return Banr(input, instruction);
                case OpcodeName.bani:
                    return Bani(input, instruction);
                case OpcodeName.borr:
                    return Borr(input, instruction);
                case OpcodeName.bori:
                    return Bori(input, instruction);
                case OpcodeName.setr:
                    return Setr(input, instruction);
                case OpcodeName.seti:
                    return Seti(input, instruction);
                case OpcodeName.gtir:
                    return Gtir(input, instruction);
                case OpcodeName.gtri:
                    return Gtri(input, instruction);
                case OpcodeName.gtrr:
                    return Gtrr(input, instruction);
                case OpcodeName.eqir:
                    return Eqir(input, instruction);
                case OpcodeName.eqri:
                    return Eqri(input, instruction);
                case OpcodeName.eqrr:
                    return Eqrr(input, instruction);
                default:
                    throw new ArgumentException("Invalid opcode, or missing from operations", nameof(instruction));
            }
        }

        private static List<int[]> GetTestProgram()
        {
            var i = Input.Length - 1;
            while (!Input[i].StartsWith("After"))
            {
                i--;
            }
            return Input.Skip(i + 1).SkipWhile(string.IsNullOrEmpty)
                .Select(line => line.Split(' ').Select(int.Parse).ToArray())
                .ToList();
        }

        private static Dictionary<int, HashSet<OpcodeName>> GetOpcodes(Dictionary<int[], List<OpcodeName>> sampleMatches)
        {
            var matchLookup = sampleMatches.ToLookup(pair => pair.Key[0], pair => pair.Value);
            var opcodeCandidates = Enumerable.Range(0, 16).ToDictionary(x => x, x =>
                new HashSet<OpcodeName>(matchLookup[x].SelectMany(matches => matches)));

            foreach (var matches in sampleMatches)
            {
                Update(opcodeCandidates, matches);
            }

            while (opcodeCandidates.Any(pair => pair.Value.Count > 1))
            {
                for (var i = 0; i < 16; i++)
                {
                    Update(i, opcodeCandidates[i], opcodeCandidates);
                }
            }

            return opcodeCandidates;
        }

        private static void Update(
            Dictionary<int, HashSet<OpcodeName>> opcodeCandidates,
            KeyValuePair<int[], List<OpcodeName>> sampleMatches)
        {
            opcodeCandidates[sampleMatches.Key[0]].IntersectWith(sampleMatches.Value);
        }

        private static void Update(int current, HashSet<OpcodeName> candidates, Dictionary<int, HashSet<OpcodeName>> opcodeCandidates)
        {
            if (candidates.Count == 1)
            {
                foreach (var pair in opcodeCandidates)
                {
                    if (pair.Key != current)
                    {
                        pair.Value.Remove(candidates.Single());
                    }
                }
            }
        }

        private static List<OpcodeName> GetMatches((int[] input, int[] instruction, int[] output) sample)
        {
            var matches = new List<OpcodeName>();
            var (input, instr, output) = sample;
            if (output.SequenceEqual(Addr(input, instr)))
            {
                matches.Add(OpcodeName.addr);
            }
            if (output.SequenceEqual(Addi(input, instr)))
            {
                matches.Add(OpcodeName.addi);
            }
            if (output.SequenceEqual(Mulr(input, instr)))
            {
                matches.Add(OpcodeName.mulr);
            }
            if (output.SequenceEqual(Muli(input, instr)))
            {
                matches.Add(OpcodeName.muli);
            }
            if (output.SequenceEqual(Banr(input, instr)))
            {
                matches.Add(OpcodeName.banr);
            }
            if (output.SequenceEqual(Bani(input, instr)))
            {
                matches.Add(OpcodeName.bani);
            }
            if (output.SequenceEqual(Borr(input, instr)))
            {
                matches.Add(OpcodeName.borr);
            }
            if (output.SequenceEqual(Bori(input, instr)))
            {
                matches.Add(OpcodeName.bori);
            }
            if (output.SequenceEqual(Setr(input, instr)))
            {
                matches.Add(OpcodeName.setr);
            }
            if (output.SequenceEqual(Seti(input, instr)))
            {
                matches.Add(OpcodeName.seti);
            }
            if (output.SequenceEqual(Gtir(input, instr)))
            {
                matches.Add(OpcodeName.gtir);
            }
            if (output.SequenceEqual(Gtri(input, instr)))
            {
                matches.Add(OpcodeName.gtri);
            }
            if (output.SequenceEqual(Gtrr(input, instr)))
            {
                matches.Add(OpcodeName.gtrr);
            }
            if (output.SequenceEqual(Eqir(input, instr)))
            {
                matches.Add(OpcodeName.eqir);
            }
            if (output.SequenceEqual(Eqri(input, instr)))
            {
                matches.Add(OpcodeName.eqri);
            }
            if (output.SequenceEqual(Eqrr(input, instr)))
            {
                matches.Add(OpcodeName.eqrr);
            }
            return matches;
        }

        private static int[] Addr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] + input[instr[2]];
            return res;
        }

        private static int[] Addi(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] + instr[2];
            return res;
        }

        private static int[] Mulr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] * input[instr[2]];
            return res;
        }

        private static int[] Muli(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] * instr[2];
            return res;
        }

        private static int[] Banr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] & input[instr[2]];
            return res;
        }

        private static int[] Bani(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] & instr[2];
            return res;
        }

        private static int[] Borr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] | input[instr[2]];
            return res;
        }

        private static int[] Bori(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] | instr[2];
            return res;
        }

        private static int[] Setr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]];
            return res;
        }

        private static int[] Seti(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = instr[1];
            return res;
        }

        private static int[] Gtir(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = instr[1] > input[instr[2]] ? 1 : 0;
            return res;
        }

        private static int[] Gtri(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] > instr[2] ? 1 : 0;
            return res;
        }

        private static int[] Gtrr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] > input[instr[2]] ? 1 : 0;
            return res;
        }

        private static int[] Eqir(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = instr[1] == input[instr[2]] ? 1 : 0;
            return res;
        }

        private static int[] Eqri(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] == instr[2] ? 1 : 0;
            return res;
        }

        private static int[] Eqrr(int[] input, int[] instr)
        {
            var res = input.ToArray();
            res[instr[3]] = input[instr[1]] == input[instr[2]] ? 1 : 0;
            return res;
        }

        enum OpcodeName
        {
            addr, addi,       // add register, add immediate
            mulr, muli,       // multiply register, multiply immediate
            banr, bani,       // bitwise AND register, bitwise AND immediate
            borr, bori,       // bitwise OR register, bitwise OR immediate
            setr, seti,       // Assignment (set register, set immediate)
            gtir, gtri, gtrr, // greater-than immediate/register, register/immediate, register/register
            eqir, eqri, eqrr  // equality test immediate/register, register/immediate, register/register
        }

        private static List<(int[] input, int[] instruction, int[] output)> GetSamples()
        {
            var samples = new List<(int[] input, int[] instruction, int[] output)>();
            var index = 0;
            while (Input[index].StartsWith("Before"))
            {
                var input = Input[index].Substring(9, Input[index].Length - 10)
                    .Split(", ").Select(int.Parse).ToArray();

                var instruction = Input[index + 1].Split(' ').Select(int.Parse).ToArray();

                var output = Input[index + 2].Substring(9, Input[index + 2].Length - 10)
                    .Split(", ").Select(int.Parse).ToArray();

                samples.Add((input, instruction, output));
                index += 4;
            }

            return samples;
        }
    }
}
