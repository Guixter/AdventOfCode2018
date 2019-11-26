using AdventOfCodeTools;
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
            var lines = IO.GetStringLines(@"Day18\Input.txt");
            var grid = ComputeStraight(lines, 10);
            return ComputeProduct(grid);
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day18\Input.txt");
            var grid = ComputeWithCycleDetection(lines, 1000000000, 50);
            return ComputeProduct(grid);
        }

        private static Grid<Tile> ComputeStraight(string[] lines, int nbMinutes)
        {
            var grid = ParseGrid(lines);

            for (var t = 0; t < nbMinutes; t++)
            {
                grid = ComputeStep(grid);
            }

            return grid;
        }

        private static Grid<Tile> ComputeWithCycleDetection(string[] lines, long nbMinutes, int cycleDetectionCacheSize)
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

        private static Grid<Tile> ComputeStep(Grid<Tile> current)
        {
            var newGrid = new Grid<Tile>(current.xLength, current.yLength);

            for (var x = 0; x < current.xLength; x++)
            {
                for (var y = 0; y < current.yLength; y++)
                {
                    newGrid[x, y] = current[x, y].ComputeStep(current);
                }
            }

            return newGrid;
        }

        private static Grid<Tile> ParseGrid(string[] lines)
        {
            var result = new Grid<Tile>(lines.Length, lines[0].Length);

            for (var x = 0; x < result.xLength; x++)
            {
                for (var y = 0; y < result.yLength; y++)
                {
                    result[x, y] = Tile.Parse(lines[x][y], x, y);
                }
            }

            return result;
        }

        private static int ComputeProduct(Grid<Tile> grid)
        {
            var flat = grid.Flatten();
            var nbTrees = flat
                .Where(x => x.type == Tile.Type.Tree)
                .Count();
            var nbLumberyards = flat
                .Where(x => x.type == Tile.Type.Lumberyard)
                .Count();
            return nbTrees * nbLumberyards;
        }

        private static void PrintGrid(Grid<Tile> grid)
        {
            var builder = new StringBuilder();
            for (var x = 0; x < grid.xLength; x++)
            {
                for (var y = 0; y < grid.yLength; y++)
                {
                    builder.Append(grid[x, y].Print());
                }
                builder.AppendLine();
            }
            var character = builder.ToString().ToCharArray();
            Console.Clear();
            Console.Write(character);
        }

        // Compute a signature to easily compare two steps
        private static string ComputeSignature(Grid<Tile> grid)
        {
            var builder = new StringBuilder();

            for (var x = 0; x < grid.xLength; x++)
            {
                var nbOpen = 0;
                var nbTrees = 0;

                for (var y = 0; y < grid.yLength; y++)
                {
                    if (grid[x,y].type == Tile.Type.Open)
                        nbOpen++;
                    if (grid[x,y].type == Tile.Type.Tree)
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

            public Tile ComputeStep(Grid<Tile> current)
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

            public IEnumerable<Tile> GetNeighbours(Grid<Tile> current)
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
                            && nX < current.xLength
                            && nY >= 0
                            && nY < current.yLength)
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