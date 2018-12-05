using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day5
    {
        private static readonly int uppercaseDelta = 32;
        private static readonly int nbCharacters = 26;


        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var line = Program.GetLines(".\\Day5\\Input.txt")[0];

            while (React(ref line));

            return line.Length;
        }

        public static int Part2()
        {
            var line = Program.GetLines(".\\Day5\\Input.txt")[0];

            var reactLength = new int[nbCharacters];
            var maxChar = 'a' + nbCharacters;
            for (char c = 'a'; c < maxChar; c++)
            {
                var cleared = new string(line
                    .Where(x => (x != c) && (x != c - uppercaseDelta))
                    .ToArray()
                );
                
                while (React(ref cleared));
                reactLength[c - 'a'] = cleared.Length;
            }

            return reactLength.OrderBy(x => x).First();
        }

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
