using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day4
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day4\\Input.txt");

            // Sort the events
            var orderedEvents = lines
                .Select(line => Event.Parse(line))
                .ToList();
            orderedEvents.Sort();

            // Find the most asleep guard
            //   sleepKeyTimes stores for each guard its ordered key times, so that an even index is a falling asleep,
            //   and an odd index is a waking up
            var sleepKeyTimes = new Dictionary<int, List<DateTime>>();
            var sleepTimePerGuard = new Dictionary<int, int>();
            var currentGuard = -1;
            var lastFallAsleep = new DateTime();
            foreach (var e in orderedEvents)
            {
                if (e.beginShift)
                {
                    currentGuard = e.beginShiftGuard;
                }
                else if (e.fallsAsleep)
                {
                    lastFallAsleep = e.datetime;
                    if (!sleepKeyTimes.ContainsKey(currentGuard))
                        sleepKeyTimes[currentGuard] = new List<DateTime>();
                    sleepKeyTimes[currentGuard].Add(e.datetime);
                }
                else if (e.wakesUp)
                {
                    if (!sleepTimePerGuard.ContainsKey(currentGuard))
                        sleepTimePerGuard[currentGuard] = 0;
                    sleepTimePerGuard[currentGuard] += (int)(e.datetime - lastFallAsleep).TotalMinutes;
                    sleepKeyTimes[currentGuard].Add(e.datetime);
                }
            }
            var mostAsleepGuard = sleepTimePerGuard.OrderByDescending(x => x.Value).First().Key;

            // Find the most asleep minute for this guard
            var guardKeyTimes = sleepKeyTimes[mostAsleepGuard];
            var numberSleepPerMinute = new Dictionary<int, int>();
            var nbIntervals = guardKeyTimes.Count() / 2;
            for (var i = 0; i < nbIntervals; i++)
            {
                var start = guardKeyTimes[2 * i];
                var end = guardKeyTimes[2 * i + 1];

                for (int j = start.Minute; j < end.Minute; j++)
                {
                    if (!numberSleepPerMinute.ContainsKey(j))
                        numberSleepPerMinute[j] = 0;
                    numberSleepPerMinute[j]++;
                }
            }

            var mostAsleepMinute = numberSleepPerMinute.OrderByDescending(x => x.Value).First().Key;

            return mostAsleepGuard * mostAsleepMinute;
        }

        public static int Part2()
        {
            var lines = Program.GetLines(".\\Day4\\Input.txt");

            // Sort the events
            var orderedEvents = lines
                .Select(line => Event.Parse(line))
                .ToList();
            orderedEvents.Sort();

            // Store the guard/minute "matrix"
            //   the first index of sleepAmount is the GuardID, the second is the minute.
            var sleepAmount = new Dictionary<int, Dictionary<int, int>>();
            var currentGuard = -1;
            var lastFallAsleep = new DateTime();
            foreach (var e in orderedEvents)
            {
                if (e.beginShift)
                {
                    currentGuard = e.beginShiftGuard;
                }
                else if (e.fallsAsleep)
                {
                    lastFallAsleep = e.datetime;
                }
                else if (e.wakesUp)
                {
                    if (!sleepAmount.ContainsKey(currentGuard))
                        sleepAmount[currentGuard] = new Dictionary<int, int>();

                    for (var i = lastFallAsleep.Minute; i < e.datetime.Minute; i++)
                    {
                        if (!sleepAmount[currentGuard].ContainsKey(i))
                            sleepAmount[currentGuard][i] = 0;
                        sleepAmount[currentGuard][i]++;
                    }
                }
            }

            // Sort by the biggest minute amount for each guard
            var keys = sleepAmount.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++)
            {
                sleepAmount[keys[i]] = sleepAmount[keys[i]].OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }

            var mostAsleepGuard = sleepAmount.OrderByDescending(x => x.Value.First().Value).First().Key;
            var mostAsleepMinute = sleepAmount[mostAsleepGuard].First().Key;

            return mostAsleepGuard * mostAsleepMinute;
        }

        private struct Event : IComparable
        {
            public DateTime datetime;
            public string message;

            public bool beginShift { get
                {
                    return messageRegex.Match(message).Success;
                }
            }

            public int beginShiftGuard { get
                {
                    return int.Parse(messageRegex.Match(message).Groups[1].Value);
                }
            }

            public bool fallsAsleep { get
                {
                    return message.Equals(fallAsleepMessage);
                }
            }

            public bool wakesUp { get
                {
                    return message.Equals(wakeUpMessage);
                }
            }

            private static readonly Regex eventRegex = new Regex(@"\[(.*)\] (.*)");
            private static readonly Regex messageRegex = new Regex(@"Guard #(\d*) begins shift");
            private static readonly string fallAsleepMessage = "falls asleep";
            private static readonly string wakeUpMessage = "wakes up";

            public int CompareTo(object obj)
            {
                return datetime.CompareTo(((Event) obj).datetime);
            }

            public static Event Parse(string line)
            {
                var match = eventRegex.Match(line);
                if (match.Success)
                {
                    return new Event() {
                        datetime = DateTime.Parse(match.Groups[1].Value),
                        message = match.Groups[2].Value,
                    };
                }
                else
                {
                    return new Event();
                }
            }
        }
    }
}
