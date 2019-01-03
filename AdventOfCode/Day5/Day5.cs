using System;
using System.Collections.Generic;
using System.Linq;

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
            var line = Utils.GetLines(".\\Day5\\Input.txt")[0].ToCharArray();
            return React(line);
        }

        public static int Part2()
        {
            var line = Utils.GetLines(".\\Day5\\Input.txt")[0].ToCharArray();

            var reactLength = new int[nbCharacters];
            var maxChar = 'a' + nbCharacters;
            for (char c = 'a'; c < maxChar; c++)
            {
                var cleared = line
                    .Where(x => (x != c) && (x != c - uppercaseDelta))
                    .ToArray();
                
                reactLength[c - 'a'] = React(cleared);
            }

            return reactLength.OrderBy(x => x).First();
        }

        private static int React(char[] s)
        {
            var stack = new Stack<char>();

            foreach (var currentChar in s)
            {
                if (stack.Count == 0)
                {
                    stack.Push(currentChar);
                    continue;
                }

                var lastChar = stack.Peek();

                if (Math.Abs(currentChar - lastChar) == uppercaseDelta)
                    stack.Pop();
                else
                    stack.Push(currentChar);
            }

            return stack.Count;
        }
    }
}
