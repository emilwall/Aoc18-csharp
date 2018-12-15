using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RoyT.AStar;

namespace Day15
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

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
                //    Console.WriteLine(iterations);
                //    PrintGrid(grid);
                //}
            }

            Console.WriteLine(iterations--);
            PrintGrid(grid);
            Console.WriteLine($"Completed: {timer.ElapsedMilliseconds}ms");
            var hpSum = combatants.Sum(c => c.Value.hp);
            Console.WriteLine($"Part 1: {iterations}*{hpSum}={iterations*hpSum}");
            // 327293 too high
            // 248535 too high
            // 238476 too low
            // 247616 close
            // 240814?
            // 245280

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
            var astarGrid = GetAstarGrid(grid, pos);
            var cp = new Position(pos.x, pos.y);
            var targetPositions = combatants.Where(c => c.Value.c != combatants[pos].c)
                .Select(c => c.Key)
                .SelectMany(c => new (int x, int y)[]
                {
                    (c.x + 1, c.y), (c.x - 1, c.y), (c.x, c.y + 1), (c.x, c.y - 1)
                })
                .Where(c => grid[c.y][c.x].Value.c == '.')
                .Select(c => new Position(c.x, c.y));

            var shortestPath = GetShortestPath(targetPositions, astarGrid, grid, cp);

            return shortestPath.HasValue
                ? (x: shortestPath.Value.X, y: shortestPath.Value.Y)
                : pos;
        }

        private static Grid GetAstarGrid(
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid,
            (int x, int y) pos)
        {
            var astarGrid = new Grid(grid[0].Length, grid.Length, 1.1f);
            foreach (var pair in grid.SelectMany(c => c))
            {
                if (pair.Value.c != '.')
                {
                    astarGrid.BlockCell(new Position(pair.Key.x, pair.Key.y));
                }
                else if (pos.x == pair.Key.x + 1 && pos.y == pair.Key.y)
                {
                    astarGrid.SetCellCost(new Position(pair.Key.x, pair.Key.y), 1.1f);
                }
                else if (pos.x == pair.Key.x - 1 && pos.y == pair.Key.y)
                {
                    astarGrid.SetCellCost(new Position(pair.Key.x, pair.Key.y), 1.2f);
                }
                else if (pos.x == pair.Key.x && pos.y == pair.Key.y - 1)
                {
                    astarGrid.SetCellCost(new Position(pair.Key.x, pair.Key.y), 1.3f);
                }
            }

            return astarGrid;
        }

        private static Position? GetShortestPath(
            IEnumerable<Position> targetPositions,
            Grid astarGrid,
            KeyValuePair<(int x, int y), (char c, int hp)>[][] grid,
            Position cp)
        {
            var paths = targetPositions.AsParallel()
                .Select(p => astarGrid.GetPath(cp, p, MovementPatterns.LateralOnly))
                .Select(path => path.Skip(1).ToList())
                .Where(path => path.Any() && grid[path.First().Y][path.First().X].Value.c == '.')
                .ToList();

            var shortestPath = paths
                .OrderBy(path => path.Count + path.Count(p => grid[p.Y][p.X].Value.c != '.') * grid.Length)
                .ThenBy(path => path.Any() ? path.First().Y : int.MaxValue)
                .ThenBy(path => path.Any() ? path.First().X : int.MaxValue)
                .FirstOrDefault();

            return shortestPath?.Any() ?? false ? shortestPath.First() : (Position?)null;
        }

        private static bool IsCombatant(char c)
        {
            return c == 'E' || c == 'G';
        }
    }
}