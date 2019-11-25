using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCodeTools;

namespace AdventOfCode
{
    class Day10
    {
        public static void Run()
        {
            Console.WriteLine(Part1_2());
        }

        public static int Part1_2()
        {
            var lines = IO.GetStringLines(@"Day10\Input.txt");

            var stars = new Star[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                stars[i].Parse(lines[i]);
            }

            var second = 0;
            var lastBox = new Box() {
                deltaX = float.PositiveInfinity,
                deltaY = float.PositiveInfinity,
            };
            while (true)
            {
                var box = ComputeBoxSize(stars, second);
                if (box.deltaX > lastBox.deltaX || box.deltaY > lastBox.deltaY)
                {
                    Display(stars, second - 1, lastBox);
                    break;
                }

                lastBox = box;
                second++;
            }

            return second - 1;
        }

        private static void Display(Star[] stars, int second, Box boundingBox)
        {
            var grid = new bool[(int) boundingBox.deltaX, (int) boundingBox.deltaY];
            foreach (var star in stars)
            {
                star.GetPositionAt(second, out var x, out var y);
                grid[x - boundingBox.x, y - boundingBox.y] = true;
            }

            for (var i = 0; i < grid.GetLength(1); i++)
            {
                for (var j = 0; j < grid.GetLength(0); j++)
                {
                    Console.Write(grid[j,i] ? "#" : "'");
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        private static Box ComputeBoxSize(Star[] stars, int second)
        {
            var xMin = float.PositiveInfinity;
            var xMax = float.NegativeInfinity;
            var yMin = float.PositiveInfinity;
            var yMax = float.NegativeInfinity;
            for (var i = 0; i < stars.Length; i++)
            {
                stars[i].GetPositionAt(second, out var x, out var y);
                if (x < xMin)
                    xMin = x;
                if (x > xMax)
                    xMax = x;
                if (y < yMin)
                    yMin = y;
                if (y > yMax)
                    yMax = y;
            }

            return new Box()
            {
                x = (int) xMin,
                y = (int) yMin,
                deltaX = (int) (xMax - xMin + 1),
                deltaY = (int) (yMax - yMin + 1),
            };
            
        }

        private struct Star
        {
            public int x;
            public int y;
            public int xVelocity;
            public int yVelocity;

            private static readonly Regex regex = new Regex(@"position=<(.*),(.*)> velocity=<(.*),(.*)>");

            public void Parse(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    x = int.Parse(match.Groups[1].Value);
                    y = int.Parse(match.Groups[2].Value);
                    xVelocity = int.Parse(match.Groups[3].Value);
                    yVelocity = int.Parse(match.Groups[4].Value);
                }
            }

            public void GetPositionAt(int second, out int x, out int y)
            {
                x = this.x + second * xVelocity;
                y = this.y + second * yVelocity;
            }
        }

        private struct Box
        {
            public int x;
            public int y;
            public float deltaX;
            public float deltaY;
        }
    }
}
