using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day17
    {

        private static readonly Position waterSpring = new Position() {
            x = 500,
            y = 0,
        };
        private static readonly int yMin = 0;
        private static readonly int yMax = 13;

        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day17\\test.txt");

            var clays = ParseClays(lines);
            var grid = BuildGrid(clays, waterSpring);

            Print(grid);

            return 0;
        }

        private static Tile[,] BuildGrid(List<Position> clays, Position waterSpring)
        {
            var xMin = clays
                .Select(x => x.x)
                .Min();
            var xMax = clays
                .Select(x => x.x)
                .Max();

            var grid = new Tile[xMax - xMin + 3, yMax - yMin + 3];

            // Init the grid
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = new Tile() {
                        type = Tile.Type.Sand,
                        x = i + xMin,
                        y = j + yMin,
                    };
                }
            }

            // Add the clay to the grid
            foreach (var clay in clays)
            {
                grid[clay.x - xMin + 1, clay.y - yMin].type = Tile.Type.Clay;
            }

            // Add the waterSpring to the grid
            grid[waterSpring.x - xMin + 1, waterSpring.y - yMin].type = Tile.Type.WaterSpring;

            return grid;
        }

        private static void Print(Tile[,] grid)
        {
            Console.Write("  ");
            for (var j = 0; j < grid.GetLength(0); j++)
            {
                Console.Write(j % 10 + " ");
            }
            Console.WriteLine();

            for (var i = 0; i < grid.GetLength(1); i++)
            {
                Console.Write(i % 10 + " ");
                for (var j = 0; j < grid.GetLength(0); j++)
                {
                    grid[j, i].Print();
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static List<Position> ParseClays(string[] lines)
        {
            var clays = new List<Position>();
            foreach (var line in lines)
            {
                clays.AddRange(Position.ParseLine(line));
            }
            return clays;
        }

        private struct Position
        {
            public int x;
            public int y;

            private static readonly Regex firstTypeRegex = new Regex(@"x=(.*), y=(.*)\.\.(.*)");
            private static readonly Regex secondTypeRegex = new Regex(@"y=(.*), x=(.*)\.\.(.*)");

            public static List<Position> ParseLine(string line)
            {
                var list = new List<Position>();
                var firstMatch = firstTypeRegex.Match(line);
                if (firstMatch.Success)
                {
                    var x = int.Parse(firstMatch.Groups[1].Value);
                    var yMin = int.Parse(firstMatch.Groups[2].Value);
                    var yMax = int.Parse(firstMatch.Groups[3].Value);

                    for (var i = yMin; i <= yMax; i++)
                    {
                        list.Add(new Position() {
                            x = x,
                            y = i,
                        });
                    }
                }
                else
                {
                    var secondMatch = secondTypeRegex.Match(line);
                    if (secondMatch.Success)
                    {
                        var y = int.Parse(secondMatch.Groups[1].Value);
                        var xMin = int.Parse(secondMatch.Groups[2].Value);
                        var xMax = int.Parse(secondMatch.Groups[3].Value);

                        for (var i = xMin; i <= xMax; i++)
                        {
                            list.Add(new Position() {
                                x = i,
                                y = y,
                            });
                        }
                    }
                }

                return list;
            }
        }

        private class Tile
        {
            public Type type;
            public int x;
            public int y;

            public void Print()
            {
                switch (type) {
                    case Type.Clay:
                        Console.Write("#");
                        break;
                    case Type.Sand:
                        Console.Write(".");
                        break;
                    case Type.WaterSpring:
                        Console.Write("+");
                        break;
                }
            }

            public enum Type
            {
                Sand,
                Clay,
                WaterSpring,
            }
        }
    }
}
