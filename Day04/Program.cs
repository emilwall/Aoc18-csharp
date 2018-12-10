using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day4
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var events = GetEvents();
            var shifts = GetShifts(events);

            var maxSleep1 = GetMax(shifts, 1);
            var guardId1 = Array.IndexOf(maxSleep1.Value, maxSleep1.Value.Max());
            Console.WriteLine("Part 1: " + guardId1 * maxSleep1.Key);

            var maxSleep2 = GetMax(shifts, 2);
            var guardId2 = Array.IndexOf(maxSleep2.Value, maxSleep2.Value.Max());
            Console.WriteLine("Part 2: " + guardId2 * maxSleep2.Key);
        }

        private enum EventType
        {
            BeginsShift = 1,
            FallsAsleep = 2,
            WakesUp = 3
        }

        private class Event
        {
            public DateTime Time { get; set; }
            public EventType Type { get; set; }
            public int Id { get; set; }
        }

        private static List<Event> GetEvents()
        {
            var events = Input
                .Select(ParseEvent)
                .OrderBy(e => e.Time)
                .ToList();

            return events;
        }

        private static Event ParseEvent(string line)
        {
            var year = int.Parse(line.Substring(1, 4));
            var month = int.Parse(line.Substring(6, 2));
            var day = int.Parse(line.Substring(9, 2));
            var hour = int.Parse(line.Substring(12, 2));
            var minute = int.Parse(line.Substring(15, 2));
            var time = new DateTime(year, month, day, hour, minute, 0);
            var type = line[19] == 'G'
                ? EventType.BeginsShift
                : line[19] == 'f'
                    ? EventType.FallsAsleep
                    : EventType.WakesUp;

            return new Event
            {
                Time = time,
                Type = type,
                Id = type == EventType.BeginsShift ? int.Parse(line.Substring(26, 4)) : 0
            };
        }

        private static Dictionary<int, int[]> GetShifts(List<Event> events)
        {
            var shifts = new Dictionary<int, int[]>();
            var currentGuard = 0;
            var prevTime = DateTime.MinValue;
            foreach (var @event in events)
            {
                switch (@event.Type)
                {
                    case EventType.BeginsShift:
                        currentGuard = @event.Id;
                        shifts[currentGuard] = shifts.ContainsKey(currentGuard) ? shifts[currentGuard] : new int[60];
                        prevTime = @event.Time;
                        break;
                    case EventType.FallsAsleep:
                        prevTime = @event.Time;
                        break;
                    case EventType.WakesUp:
                        var minutesAsleep = (@event.Time - prevTime).Minutes;
                        for (var i = 0; i < minutesAsleep; i++)
                        {
                            shifts[currentGuard][(prevTime.Minute + i) % 60]++;
                        }

                        break;
                }
            }

            return shifts;
        }

        private static KeyValuePair<int, int[]> GetMax(Dictionary<int, int[]> shifts, int part)
        {
            var maxSleep = shifts.First();
            foreach (var shift in shifts)
            {
                if (part == 1 && shift.Value.Sum() > maxSleep.Value.Sum() ||
                    part == 2 && shift.Value.Max() > maxSleep.Value.Max())
                {
                    maxSleep = shift;
                }
            }

            return maxSleep;
        }
    }
}
