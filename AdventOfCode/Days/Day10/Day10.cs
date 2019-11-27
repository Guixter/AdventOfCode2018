using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCodeTools;
using Unity.Mathematics;

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
            var lastBox = new Rectangle(
                new float2(1, 1) * float.NegativeInfinity,
                new float2(1, 1) * float.PositiveInfinity
            );

            while (true)
            {
                var rectangle = ComputeRectangle(stars, second);
                if ((rectangle.length > lastBox.length).All())
                {
                    Display(stars, second - 1, lastBox);
                    break;
                }

                lastBox = rectangle;
                second++;
            }

            return second - 1;
        }

        private static void Display(Star[] stars, int second, Rectangle boundingBox)
        {
            var grid = new Grid<bool>((int) boundingBox.length.x, (int) boundingBox.length.y);
            foreach (var star in stars)
            {
                star.GetPositionAt(second, out var x, out var y);
                grid[(int) (x - boundingBox.min.x), (int) (y - boundingBox.min.y)] = true;
            }

            grid.Print((value, x, y) => {
                return new PrintData
                {
                    text = value ? "# " : "' ",
                    color = value ? ConsoleColor.Green : ConsoleColor.White
                };
            });
        }

        private static Rectangle ComputeRectangle(Star[] stars, int second)
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

            return new Rectangle(
                new float2(xMin, yMin),
                new float2(xMax - xMin + 1, yMax - yMin + 1)
            );
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
    }
}
