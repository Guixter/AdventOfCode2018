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
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day3\\Input.txt");

            var maxSize = 1000;
            var matrix = new int[maxSize * maxSize];
            foreach (var line in lines)
            {
                var rect = Rect.Parse(line);
                for (var i = rect.left; i <= rect.right; i++)
                {
                    for (var j = rect.top; j <= rect.bottom; j++)
                    {
                        matrix[i * maxSize + j]++;
                    }
                }
            }

            return matrix.Where(x => x > 1).Count();
        }

        public static int Part2()
        {
            var lines = Program.GetLines(".\\Day3\\Input.txt");

            var handledRectangles = new HashSet<Rect>();
            var currentNotOverlapping = new List<Rect>(lines.Length);
            foreach (var line in lines)
            {
                var rect = Rect.Parse(line);

                currentNotOverlapping.RemoveAll(r => r.Overlaps(rect));
                if (handledRectangles.Where(r => r.Overlaps(rect)).Count() == 0)
                    currentNotOverlapping.Add(rect);

                handledRectangles.Add(rect);
            }

            return currentNotOverlapping[0].id;
        }

        struct Rect
        {
            public int id;
            public int left;
            public int top;
            public int width;
            public int height;

            public int right
            {
                get {
                    return left + width - 1;
                }
            }

            public int bottom
            {
                get {
                    return top + height - 1;
                }
            }

            private static readonly Regex regex = new Regex(@"#(\d*) @ (\d*),(\d*): (\d*)x(\d*)");

            public static Rect Parse(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    return new Rect() {
                        id = int.Parse(match.Groups[1].Value),
                        left = int.Parse(match.Groups[2].Value),
                        top = int.Parse(match.Groups[3].Value),
                        width = int.Parse(match.Groups[4].Value),
                        height = int.Parse(match.Groups[5].Value),
                    };
                }
                else
                {
                    return new Rect();
                }
            }

            public bool Contains(int i, int j)
            {
                return i >= left
                    && i <= right
                    && j >= top
                    && j <= bottom;
            }

            public bool Contains(Rect other)
            {
                return Contains(other.left, other.top)
                    || Contains(other.left, other.bottom)
                    || Contains(other.right, other.top)
                    || Contains(other.right, other.bottom);
            }

            public bool Overlaps(Rect other)
            {
                return Contains(other) || other.Contains(this);
            }
        }
    }
}