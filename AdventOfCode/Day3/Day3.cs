using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day3
    {

        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day3\\Input.txt");

            var maxSize = 1000;
            var matrix = new int[maxSize * maxSize];
            var regex = new Regex(@"#(\d*) @ (\d*),(\d*): (\d*)x(\d*)");
            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var id = int.Parse(match.Groups[1].Value);
                    var left = int.Parse(match.Groups[2].Value);
                    var top = int.Parse(match.Groups[3].Value);
                    var width = int.Parse(match.Groups[4].Value);
                    var height = int.Parse(match.Groups[5].Value);

                    for (var i = left; i < (left + width); i++)
                    {
                        for (var j = top; j < (top + height); j++)
                        {
                            matrix[i * maxSize + j]++;
                        }
                    }
                }
            }

            return matrix.Where(x => x > 1).Count();
        }
    }
}