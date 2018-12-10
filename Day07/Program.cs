using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day7
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            var ticks = -1;
            var tasks = GetTasks();

            var workers = Enumerable.Range(0, 5)
                .Select(_ => new Worker(null, 0))
                .ToHashSet();

            while (tasks.Any() || workers.Any(w => w.Remaining > 0))
            {
                ticks++;
                workers = workers.Select(w => new Worker(w.Task, w.Remaining - 1)).ToHashSet();
                tasks = ProcessFinishedTasks(workers, tasks, sb);

                var availableTasks = GetAvailableTasks(tasks);
                var availableWorkers = GetAvailableWorkers(workers).Take(availableTasks.Count);
                AssignTasks(availableTasks, workers, availableWorkers, tasks);
            }

            Console.WriteLine("Part 1: " + sb);
            Console.WriteLine("Part 2: " + ticks);
        }

        class Worker
        {
            public Worker(string task, int remaining)
            {
                Task = task;
                Remaining = remaining;
            }

            public string Task { get; }
            public int Remaining { get; }
        }

        private static Dictionary<string, List<string>> GetTasks()
        {
            var tasks = Input.GroupBy(line => line.Substring(36, 1), line => line.Substring(5, 1))
                .ToDictionary(group => group.Key, group => group.ToList());

            tasks = tasks.Values.SelectMany(t => t)
                .Union(tasks.Keys)
                .Distinct()
                .ToDictionary(t => t, t => tasks.ContainsKey(t) ? tasks[t] : new List<string>());

            return tasks;
        }

        private static Dictionary<string, List<string>> ProcessFinishedTasks(ISet<Worker> workers, Dictionary<string, List<string>> tasks, StringBuilder sb)
        {
            var finishedWorkers = workers.Where(w => w.Remaining == 0).ToList();
            foreach (var finishedWorker in finishedWorkers)
            {
                sb.Append(finishedWorker.Task);
                workers.Remove(finishedWorker);
                workers.Add(new Worker(null, 0));
                tasks = tasks.ToDictionary(
                    t => t.Key,
                    t => t.Value.Where(v => v != finishedWorker.Task).ToList()
                );
            }

            return tasks;
        }

        private static IEnumerable<Worker> GetAvailableWorkers(IEnumerable<Worker> workers)
        {
            return workers.Where(w => w.Remaining <= 0).ToList();
        }

        private static Stack<string> GetAvailableTasks(IDictionary<string, List<string>> tasks)
        {
            return new Stack<string>(tasks.Where(task => task.Value.Count == 0).Select(task => task.Key).OrderBy(task => task));
        }

        private static void AssignTasks(Stack<string> availableTasks, ISet<Worker> workers, IEnumerable<Worker> availableWorkers, IDictionary<string, List<string>> tasks)
        {
            foreach (var worker in availableWorkers)
            {
                var task = availableTasks.Pop();
                workers.Remove(worker);
                workers.Add(new Worker(task, 61 + task[0] - 'A'));
                tasks.Remove(task);
            }
        }
    }
}
