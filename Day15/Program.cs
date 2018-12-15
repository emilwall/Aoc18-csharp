using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day15
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("example.txt");

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var grid = Input.Select((line, y) => line
                .Select((c, x) => new KeyValuePair<(int x, int y), (char c, int hp)>(
                    (x, y),
                    (c, IsCombatant(c) ? 200 : 0)))
                .ToArray()
            ).ToArray();

            var combatants = grid.SelectMany(c => c)
                .Where(pair => IsCombatant(pair.Value.c))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var iterations = 0;
            PrintGrid(grid);
            while (ExecuteRounds(grid, combatants))
            {
                iterations++;
                //if (iterations % 10 == 0)
                //{
                    Console.WriteLine(iterations);
                    PrintGrid(grid);
                //}
            }

            //Console.WriteLine(iterations);
            //PrintGrid(grid);
            Console.WriteLine($"Completed: {timer.ElapsedMilliseconds}ms");
            var hpSum = combatants.Sum(c => c.Value.hp);
            Console.WriteLine($"Part 1: {iterations}*{hpSum}={iterations*hpSum}");
            // 327293 too high

            Console.WriteLine($"Part 2: {0}");
        }

        private static void PrintGrid(KeyValuePair<(int x, int y), (char c, int hp)>[][] grid)
        {
            foreach (var row in grid)
            {
                Console.Write(string.Join("", row.Select(c => c.Value.c)));
                Console.WriteLine("   " + string.Join(", ",
                    row.Select(c => c.Value).Where(c => IsCombatant(c.c)).Select(c => $"{c.c}({c.hp})")));
            }
        }

        private static bool ExecuteRounds(
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid,
            Dictionary<(int x, int y), (char c, int hp)> combatants)
        {
            var actionWasTaken = false;
            var positions = combatants.Keys.OrderBy(c => c.y).ThenBy(c => c.x).ToList();
            foreach (var pos in positions.Where(combatants.ContainsKey))
            {
                var inAttackPosition = GetTargets(pos, combatants).Any();
                if (inAttackPosition)
                {
                    actionWasTaken = true;
                    Attack(pos, combatants, grid);
                }
                else
                {
                    var newPos = Move(pos, combatants, grid);
                    actionWasTaken |= newPos.x != pos.x || newPos.y != pos.y;
                    actionWasTaken |= Attack(newPos, combatants, grid);
                }
            }

            return actionWasTaken;
        }

        private static List<KeyValuePair<(int x, int y), (char c, int hp)>> GetTargets(
            (int x, int y) pos,
            Dictionary<(int x, int y), (char c, int hp)> combatants)
        {
            var targets = combatants.AsParallel()
                .Where(c => Math.Abs(c.Key.x - pos.x) + Math.Abs(c.Key.y - pos.y) == 1)
                .Where(c => c.Value.c != combatants[pos].c)
                .OrderBy(c => c.Value.hp)
                .ThenBy(c => c.Key.y)
                .ThenBy(c => c.Key.x)
                .ToList();

            return targets;
        }

        private static (int x, int y) Move((int x, int y) pos,
            Dictionary<(int x, int y), (char c, int hp)> combatants,
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid)
        {
            var newPos = GetNewPosition(pos, combatants, grid);

            var combatant = combatants[pos];
            combatants.Remove(pos);
            combatants.Add(newPos, combatant);
            grid[pos.y][pos.x] = new KeyValuePair<(int x, int y), (char c, int hp)>(pos, ('.', 0));
            grid[newPos.y][newPos.x] = new KeyValuePair<(int x, int y), (char c, int hp)>(newPos, combatant);

            return newPos;
        }

        private static bool Attack((int x, int y) pos,
            Dictionary<(int x, int y), (char c, int hp)> combatants,
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid)
        {
            var targets = GetTargets(pos, combatants);
            if (!targets.Any())
            {
                return false;
            }

            var target = targets.First();
            if (target.Value.hp > 3)
            {
                combatants[target.Key] = (target.Value.c, target.Value.hp - 3);
                grid[target.Key.y][target.Key.x] = new KeyValuePair<(int x, int y), (char c, int hp)>(
                    target.Key,
                    (target.Value.c, target.Value.hp - 3)
                );
            }
            else
            {
                combatants.Remove(target.Key);
                grid[target.Key.y][target.Key.x] = new KeyValuePair<(int x, int y), (char c, int hp)>(
                    target.Key,
                    ('.', 0)
                );
            }

            return true;
        }

        private static (int x, int y) GetNewPosition((int x, int y) pos,
            Dictionary<(int x, int y), (char c, int hp)> combatants,
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid)
        {
            var targetPositions = combatants.Where(c => c.Value.c != combatants[pos].c)
                .Select(c => c.Key)
                .SelectMany(c => new (int x, int y)[]
                {
                    (c.x + 1, c.y), (c.x - 1, c.y), (c.x, c.y + 1), (c.x, c.y - 1)
                })
                .Where(c => grid[c.y][c.x].Value.c == '.');

            var shortestPath = GetShortestPath(targetPositions, grid, pos);

            return shortestPath ?? pos;
        }

        private static (int x, int y)? GetShortestPath(
            IEnumerable<(int x, int y)> targetPositions,
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid,
            (int x, int y) pos)
        {
            var nextStep = targetPositions.Select(target =>
            {
                var steps = grid.SelectMany(c => c)
                    .Where(c => c.Value.c == '.')
                    .ToDictionary(c => c.Key, _ => 100000);

                var proccessed = new HashSet<(int x, int y)>();
                var remaining = new Stack<(int x, int y)>();
                var current = target;
                steps[current] = 0;
                do
                {
                    proccessed.Add(current);
                    var options = new[]
                    {
                        (x: current.x + 1, y: current.y),
                        (x: current.x - 1, y: current.y),
                        (x: current.x, y: current.y + 1),
                        (x: current.x, y: current.y - 1)
                    };
                    foreach (var option in options)
                    {
                        if (steps.ContainsKey(option))
                        {
                            steps[current] = Math.Min(steps[option] + 1, steps[current]);
                            steps[option] = Math.Min(steps[option], steps[current] + 1);
                        }

                        if (!proccessed.Contains(option))
                        {
                            remaining.Push(option);
                        }
                    }

                    current = remaining.Pop();
                } while (remaining.Any() || current.x != pos.x || current.y != pos.y);

                return new[]
                {
                    (x: pos.x + 1, y: pos.y),
                    (x: pos.x - 1, y: pos.y),
                    (x: pos.x, y: pos.y + 1),
                    (x: pos.x, y: pos.y - 1)
                }.Where(option => steps.ContainsKey(option))
                .GroupBy(option => steps[option])
                .OrderBy(group => group.Key)
                .ThenBy(group => group.First().y)
                .ThenBy(group => group.First().x)
                .First();
            }).OrderBy(group => group.Key)
                .ThenBy(group => group.First().y)
                .ThenBy(group => group.First().x)
                .First().First();

            return Math.Abs(nextStep.x - pos.x) + Math.Abs(nextStep.y - pos.y) == 1
                ? nextStep
                : ((int x, int y)?)null;
        }

        private static bool IsCombatant(char c)
        {
            return c == 'E' || c == 'G';
        }
    }
}