using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day19
{
    class Program
    {
        private static readonly string[] Input = File.ReadAllLines("input.txt");
        private static int[] Reg;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var ip = int.Parse(Input.First().Substring(4));
            var instructions = ParseInstructions();

            Reg = new[] { 0, 0, 0, 0, 0, 0 };
            Execute(instructions, ip);
            Console.WriteLine($"Part 1: {Reg[0]} ({timer.ElapsedMilliseconds}ms)");

            Reg = new[] { 1, 0, 0, 0, 0, 0 };
            Execute(instructions, ip, 500000);
            var prev = Reg.ToArray();
            Execute(instructions, ip, 1000000);
            var res = SumOfFactors(Reg.Intersect(prev).Max());
            Console.WriteLine($"Part 2: {res} ({timer.ElapsedMilliseconds}ms)");
        }

        private static void Execute((string opcode, int[] args)[] instructions, int ip, int iterations = -1)
        {
            while (iterations-- != 0)
            {
                var instruction = instructions[Reg[ip]];
                Reg = Operations[instruction.opcode](Reg, instruction.args);

                if (Reg[ip] + 1 >= 0 && Reg[ip] + 1 < instructions.Length)
                {
                    Reg[ip]++;
                }
                else
                {
                    break;
                }
            }
        }

        private static int SumOfFactors(int n)
        {
            return Enumerable.Range(1, (int)Math.Sqrt(n) + 1)
                .Where(i => n % i == 0)
                .SelectMany(i => new[] { i, n / i })
                .Distinct()
                .Sum();
        }

        private static (string opcode, int[] args)[] ParseInstructions()
        {
            return Input.Skip(1).Select(line => (
                opcode: line.Substring(0, line.IndexOf(' ')),
                args: line.Split(' ').Skip(1).Select(int.Parse).ToArray()
            )).ToArray();
        }

        private static Func<int[], int[], int[]> Apply(Func<int[], int[], int> f) => (input, instr) =>
            input.Select((v, i) => i == instr[2] ? f(input, instr) : v).ToArray();

        private static readonly Dictionary<string, Func<int[], int[], int[]>> Operations =
            new Dictionary<string, Func<int[], int[], int[]>>
            {
                { "addr", Apply((input, instr) => input[instr[0]] + input[instr[1]]) },
                { "addi", Apply((input, instr) => input[instr[0]] + instr[1]) },
                { "mulr", Apply((input, instr) => input[instr[0]] * input[instr[1]]) },
                { "muli", Apply((input, instr) => input[instr[0]] * instr[1]) },
                { "banr", Apply((input, instr) => input[instr[0]] & input[instr[1]]) },
                { "bani", Apply((input, instr) => input[instr[0]] & instr[1]) },
                { "borr", Apply((input, instr) => input[instr[0]] | input[instr[1]]) },
                { "bori", Apply((input, instr) => input[instr[0]] | instr[1]) },
                { "setr", Apply((input, instr) => input[instr[0]]) },
                { "seti", Apply((input, instr) => instr[0]) },
                { "gtir", Apply((input, instr) => instr[0] > input[instr[1]] ? 1 : 0) },
                { "gtri", Apply((input, instr) => input[instr[0]] > instr[1] ? 1 : 0) },
                { "gtrr", Apply((input, instr) => input[instr[0]] > input[instr[1]] ? 1 : 0) },
                { "eqir", Apply((input, instr) => instr[0] == input[instr[1]] ? 1 : 0) },
                { "eqri", Apply((input, instr) => input[instr[0]] == instr[1] ? 1 : 0) },
                { "eqrr", Apply((input, instr) => input[instr[0]] == input[instr[1]] ? 1 : 0) }
            };
    }
}
