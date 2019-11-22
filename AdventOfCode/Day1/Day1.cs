using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    class Day1
    {

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            return IO.GetIntLines(@"Day1\Input.txt")
                .Aggregate(0, (acc, x) => acc + x);
        }

        public static int Part2()
        {
            var set = new HashSet<int>();
            var lines = IO.GetIntLines(@"Day1\Input.txt");

            var currentFrequency = 0;
            while(true)
            {
                foreach (var line in lines)
                {
                    if (set.Contains(currentFrequency))
                        return currentFrequency;
                    set.Add(currentFrequency);
                    currentFrequency += line;
                }
            }
        }
    }
}
