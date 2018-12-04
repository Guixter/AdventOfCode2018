using System;
using System.Collections.Generic;

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
            var lines = System.IO.File.ReadAllLines(Program.GetPath(".\\Day1\\Input.txt"));

            var result = 0;
            foreach (var line in lines)
            {
                result += int.Parse(line);
            }

            return result;
        }

        public static int Part2()
        {
            var set = new HashSet<int>();
            var lines = System.IO.File.ReadAllLines(Program.GetPath(".\\Day1\\Input.txt"));

            var currentFrequency = 0;
            while(true)
            {
                foreach (var line in lines)
                {
                    if (set.Contains(currentFrequency))
                        return currentFrequency;
                    set.Add(currentFrequency);
                    currentFrequency += int.Parse(line);
                }
            }
        }
    }
}
