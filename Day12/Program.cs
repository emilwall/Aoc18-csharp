using System;
using System.Diagnostics;
using System.IO;

namespace Day12
{
    class Program
    {
        static readonly bool IsTest = true;
        static readonly string[] Input = File.ReadAllLines(IsTest ? "example.txt" : "input.txt");

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            Console.WriteLine($"Part 1: , time: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine($"Part 2: , time: {timer.ElapsedMilliseconds}ms");
        }
    }
}
