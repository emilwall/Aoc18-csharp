using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Day16
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var samples = GetSamples();
            var ambiguityCount = samples.Select(CountMatches).Count(count => count >= 3);

            Console.WriteLine($"Part 1: {ambiguityCount} ({timer.ElapsedMilliseconds}ms)");

            var opcodeCandidates = Enumerable.Range(0, 16).ToDictionary(x => x, _ =>
                new HashSet<OpcodeName>(Enum.GetValues(typeof(OpcodeName)).Cast<OpcodeName>()));

            foreach (var sample in samples)
            {
                Update(opcodeCandidates, sample);
            }

            Console.WriteLine($"Part 2:  ({timer.ElapsedMilliseconds}ms)");
        }

        private static int CountMatches((int[] input, int[] instruction, int[] output) sample)
        {
            var count = 0;
            var (input, instr, output) = sample;
            if (input[instr[1]] + input[instr[2]] == output[instr[3]]) // addr
            {
                count++;
            }
            if (input[instr[1]] + instr[2] == output[instr[3]]) // addi
            {
                count++;
            }
            if (input[instr[1]] * input[instr[2]] == output[instr[3]]) // mulr
            {
                count++;
            }
            if (input[instr[1]] * instr[2] == output[instr[3]]) // muli
            {
                count++;
            }
            if ((input[instr[1]] & input[instr[2]]) == output[instr[3]]) // banr
            {
                count++;
            }
            if ((input[instr[1]] & instr[2]) == output[instr[3]]) // bani
            {
                count++;
            }
            if ((input[instr[1]] | input[instr[2]]) == output[instr[3]]) // borr
            {
                count++;
            }
            if ((input[instr[1]] | instr[2]) == output[instr[3]]) // bori
            {
                count++;
            }
            if (input[instr[1]] == output[instr[3]]) // setr
            {
                count++;
            }
            if (instr[1] == output[instr[3]]) // seti
            {
                count++;
            }
            if ((instr[1] > input[instr[2]] ? 1 : 0) == output[instr[3]]) // gtir
            {
                count++;
            }
            if ((input[instr[1]] > instr[2] ? 1 : 0) == output[instr[3]]) // gtri
            {
                count++;
            }
            if ((input[instr[1]] > input[instr[2]] ? 1 : 0) == output[instr[3]]) // gtrr
            {
                count++;
            }
            if ((instr[1] == input[instr[2]] ? 1 : 0) == output[instr[3]]) // eqir
            {
                count++;
            }
            if ((input[instr[1]] == instr[2] ? 1 : 0) == output[instr[3]]) // eqri
            {
                count++;
            }
            if ((input[instr[1]] == input[instr[2]] ? 1 : 0) == output[instr[3]]) // eqrr
            {
                count++;
            }
            return count;
        }

        private static void Update(
            Dictionary<int, HashSet<OpcodeName>> opcodeCandidates,
            (int[] input, int[] instruction, int[] output) sample)
        {
            
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
