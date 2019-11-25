using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Rect = AdventOfCodeTools.Rectangle<int>;

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
                for (var i = (int) rect.xMin; i <= rect.xMax; i++)
                {
                    for (var j = (int) rect.yMin; j <= rect.yMax; j++)
                    {
                        matrix[i * maxSize + j]++;
                    }
                }
            }

            return matrix.Where(x => x > 1).Count();
        }

        public static Rect ParseRectangle(string line)
        {
            var values = IO.Match(regex, line);
            return new Rect
            {
                meta = int.Parse(values[0]),
                xMin = int.Parse(values[1]),
                yMin = int.Parse(values[2]),
                xLength = int.Parse(values[3]),
                yLength = int.Parse(values[4]),
            };
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day3\Input.txt");

            var handledRectangles = new HashSet<Rect>();
            var currentNotOverlapping = new List<Rect>(lines.Count());
            foreach (var line in lines)
            {
                var rect = ParseRectangle(line);

                currentNotOverlapping.RemoveAll(r => r.Overlaps(rect));
                if (handledRectangles.Where(r => r.Overlaps(rect)).Count() == 0)
                    currentNotOverlapping.Add(rect);

                handledRectangles.Add(rect);
            }

            return currentNotOverlapping[0].meta;
        }
    }
}