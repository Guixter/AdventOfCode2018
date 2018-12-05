using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day5
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var line = Program.GetLines(".\\Day5\\Input.txt")[0];

            while (React(ref line));

            return line.Length;
        }

        private static readonly int uppercaseDelta = 32;

        private static bool React(ref string s)
        {
            for (var i = 0; i < s.Length - 1; i++)
            {
                var firstChar = s[i];
                var secondChar = s[i + 1];

                if (Math.Abs(secondChar - firstChar) == uppercaseDelta)
                {
                    s = s.Remove(i, 2);
                    return true;
                }
            }
            return false;
        }
    }
}
