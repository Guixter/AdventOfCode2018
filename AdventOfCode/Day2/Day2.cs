using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    class Day2
    {

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day2\\Input.txt");

            var twoOccurences = 0;
            var threeOccurences = 0;
            var dictionary = new Dictionary<char, int>();
            foreach (var line in lines)
            {
                dictionary.Clear();

                foreach (var character in line)
                {
                    if (!dictionary.ContainsKey(character))
                        dictionary.Add(character, 0);

                    dictionary[character]++;
                }

                twoOccurences += dictionary.Where(x => x.Value == 2).Count() > 0 ? 1 : 0;
                threeOccurences += dictionary.Where(x => x.Value == 3).Count() > 0 ? 1 : 0;
            }

            return twoOccurences * threeOccurences;
        }

        public static string Part2()
        {
            var lines = Program.GetLines(".\\Day2\\Input.txt");

            for (var i = 0 ; i < lines.Length; i++)
            {
                var firstArray = lines[i].ToCharArray();

                for (var j = i + 1; j < lines.Length; j++)
                {
                    var secondArray = lines[j].ToCharArray();

                    var delta = firstArray.Zip(secondArray, (x, y) => new Tuple<char, char>((char) (x - y), x));
                    var commonCharacters = delta.Where(x => x.Item1 == 0);
                    var differentCharactersCount = firstArray.Length - commonCharacters.Count();

                    if (differentCharactersCount == 1)
                    {
                        return new string(commonCharacters.Select(x => x.Item2).ToArray());
                    }
                }
            }

            return string.Empty;
        }
    }
}
