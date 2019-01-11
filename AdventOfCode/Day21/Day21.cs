using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day21
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static long Part1()
        {
            var lines = Utils.GetLines(".\\Day21\\Input.txt");

            Day19.Command.Parse(lines, out var binding, out var commands);
            var registers = new long[6] { 0, 0, 0, 0, 0, 0 };

            // When looking at the code, we can see that A never changes,
            // and that the code exits whenever F == A at ip 28 (see pass_3.txt).
            // So the correct answer is the first value of F when we reach ip 28.
            Day19.Compute(binding, commands, registers, false, (ip, reg) => {
                return ip != 28;
            });

            return registers[5];
        }

        public static long Part2()
        {
            var lines = Utils.GetLines(".\\Day21\\Input.txt");

            Day19.Command.Parse(lines, out var binding, out var commands);
            var registers = new long[6] { 0, 0, 0, 0, 0, 0 };

            var hashSet = new HashSet<long>();

            Day19.Compute(binding, commands, registers, false, (ip, reg) => {
                if (ip == 28)
                {
                    if (hashSet.Contains(reg[5]))
                        return false;
                    hashSet.Add(reg[5]);
                }
                
                return true;
            });

            return hashSet.Max();
        }
    }
}