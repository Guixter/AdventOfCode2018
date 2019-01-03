using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day18
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Utils.GetLines(".\\Day18\\Input.txt");
            var grid = ComputeStraight(lines, 10);
            return ComputeProduct(grid);
        }

        public static int Part2()
        {
            var lines = Utils.GetLines(".\\Day18\\Input.txt");
            var grid = ComputeWithCycleDetection(lines, 1000000000, 50);
            return ComputeProduct(grid);
        }

        private static Tile[,] ComputeStraight(string[] lines, int nbMinutes)
        {
            var grid = ParseGrid(lines);

            for (var t = 0; t < nbMinutes; t++)
            {
                grid = ComputeStep(grid);
            }

            return grid;
        }

        private static Tile[,] ComputeWithCycleDetection(string[] lines, long nbMinutes, int cycleDetectionCacheSize)
        {
            var grid = ParseGrid(lines);

            var cycleDetectionCache = new string[cycleDetectionCacheSize];
            var cdCursor = -1;

            long t = 0;
            var cyclePeriod = -1;
            for (t = 0; t < nbMinutes && cyclePeriod == -1; t++)
            {
                var signature = ComputeSignature(grid);
                cdCursor = (cdCursor + 1) % cycleDetectionCacheSize;
                cycleDetectionCache[cdCursor] = signature;

                for (var i = 1; i < cycleDetectionCacheSize; i++)
                {
                    var index = (cdCursor - i + cycleDetectionCacheSize) % cycleDetectionCacheSize;

                    if (cycleDetectionCache[index] == null)
                        break;

                    if (cycleDetectionCache[index].Equals(signature))
                    {
                        cyclePeriod = i;
                        break;
                    }
                }

                grid = ComputeStep(grid);
            }

            if (t < nbMinutes)
            {
                // Cycle detected
                var remainingSteps = (nbMinutes - t) % cyclePeriod;
                for (var i = 0; i < remainingSteps; i++)
                {
                    grid = ComputeStep(grid);
                }
            }

            return grid;
        }

        private static Tile[,] ComputeStep(Tile[,] current)
        {
            var newGrid = new Tile[current.GetLength(0), current.GetLength(1)];

            for (var i = 0; i < current.GetLength(0); i++)
            {
                for (var j = 0; j < current.GetLength(1); j++)
                {
                    newGrid[i, j] = current[i, j].ComputeStep(current);
                }
            }

            return newGrid;
        }

        private static Tile[,] ParseGrid(string[] lines)
        {
            var result = new Tile[lines.Length, lines[0].Length];

            for (var i = 0; i < result.GetLength(0); i++)
            {
                for (var j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = Tile.Parse(lines[i][j], i, j);
                }
            }

            return result;
        }

        private static int ComputeProduct(Tile[,] grid)
        {
            var flat = Utils.Flatten(grid);
            var nbTrees = flat
                .Where(x => x.type == Tile.Type.Tree)
                .Count();
            var nbLumberyards = flat
                .Where(x => x.type == Tile.Type.Lumberyard)
                .Count();
            return nbTrees * nbLumberyards;
        }

        private static void PrintGrid(Tile[,] grid)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    builder.Append(grid[i, j].Print());
                }
                builder.AppendLine();
            }
            var character = builder.ToString().ToCharArray();
            Console.Clear();
            Console.Write(character);
        }

        // Compute a signature to easily compare two steps
        private static string ComputeSignature(Tile[,] grid)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                var nbOpen = 0;
                var nbTrees = 0;

                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i,j].type == Tile.Type.Open)
                        nbOpen++;
                    if (grid[i,j].type == Tile.Type.Tree)
                        nbTrees++;
                }

                builder.Append(nbOpen);
                builder.Append(nbTrees);
            }

            return builder.ToString();
        }

        private class Tile
        {
            public Type type;
            public int x;
            public int y;

            public Tile ComputeStep(Tile[,] current)
            {
                var result = new Tile() {
                    x = x,
                    y = y,
                    type = type,
                };

                var neighbours = GetNeighbours(current);


                switch (type)
                {
                    case Type.Open:
                        if (neighbours.Where(x => x.type == Type.Tree).Count() >= 3)
                            result.type = Type.Tree;
                        break;
                    case Type.Tree:
                        if (neighbours.Where(x => x.type == Type.Lumberyard).Count() >= 3)
                            result.type = Type.Lumberyard;
                        break;
                    case Type.Lumberyard:
                        var nbLumberyards = neighbours
                            .Where(x => x.type == Type.Lumberyard)
                            .Count();
                        var nbTrees = neighbours
                            .Where(x => x.type == Type.Tree)
                            .Count();
                        if (!(nbLumberyards >= 1 && nbTrees >= 1))
                            result.type = Type.Open;
                        break;
                }

                return result;
            }

            public IEnumerable<Tile> GetNeighbours(Tile[,] current)
            {
                var result = new List<Tile>(8);

                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        var nX = x + i;
                        var nY = y + j;
                        if (nX >= 0
                            && nX < current.GetLength(0)
                            && nY >= 0
                            && nY < current.GetLength(1))
                        {
                            result.Add(current[nX, nY]);
                        }
                    }
                }

                return result;
            }

            public static Tile Parse(char character, int x, int y)
            {
                var result = new Tile() {
                    x = x,
                    y = y,
                };

                switch (character)
                {
                    case '.':
                        result.type = Type.Open;
                        break;
                    case '|':
                        result.type = Type.Tree;
                        break;
                    case '#':
                        result.type = Type.Lumberyard;
                        break;
                }

                return result;
            }

            public char Print()
            {
                switch (type)
                {
                    case Type.Open:
                        return '.';
                    case Type.Tree:
                        return '|';
                    case Type.Lumberyard:
                        return '#';
                }
                return '0';
            }

            public enum Type
            {
                Open,
                Tree,
                Lumberyard,
            }
        }
    }
}