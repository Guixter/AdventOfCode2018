using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;

namespace AdventOfCode
{
    class Day3
    {
        private static readonly Regex regex = new Regex(@"#(\d*) @ (\d*),(\d*): (\d*)x(\d*)");

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day3\Input.txt");

            var maxSize = 1000;
            var matrix = new int[maxSize * maxSize];
            foreach (var line in lines)
            {
                var rect = ParseRectangle(line);
                for (var i = (int) rect.Item1.min.x; i <= rect.Item1.max.x; i++)
                {
                    for (var j = (int) rect.Item1.min.y; j <= rect.Item1.max.y; j++)
                    {
                        matrix[i * maxSize + j]++;
                    }
                }
            }

            return matrix.Where(x => x > 1).Count();
        }

        public static (Rectangle, int) ParseRectangle(string line)
        {
            var values = IO.Match(regex, line);

            var min = new float2(int.Parse(values[1]), int.Parse(values[2]));
            var length = new float2(int.Parse(values[3]), int.Parse(values[4]));
            return (
                new Rectangle {
                    min = min,
                    length = length
                },

                int.Parse(values[0])
            );
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day3\Input.txt");

            var handledRectangles = new HashSet<Rectangle>();
            var currentNotOverlapping = new List<(Rectangle, int)>(lines.Count());
            foreach (var line in lines)
            {
                var rect = ParseRectangle(line);

                currentNotOverlapping.RemoveAll(r => r.Item1.Overlaps(rect.Item1));
                if (handledRectangles.Where(r => r.Overlaps(rect.Item1)).Count() == 0)
                    currentNotOverlapping.Add(rect);

                handledRectangles.Add(rect.Item1);
            }

            return currentNotOverlapping[0].Item2;
        }
    }
}