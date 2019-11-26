using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day17
    {
        private static readonly int printRadius = 30;
        private static readonly int printStep = 1;
        private static readonly int firstPrintStep = 0;

        private static readonly Position waterSpring = new Position() {
            x = 500,
            y = 0,
        };

        public static void Run()
        {
            //IO.SetOutputFile(@"Day17\Output.txt");
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day17\Input.txt");

            var clays = ParseClays(lines);
            var grid = Grid.Build(clays, waterSpring);

            Compute(grid);
            //grid.Print();

            return grid
                .Flatten()
                .Where(x => x.type == Tile.Type.WetSand)
                .Count();
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day17\Input.txt");

            var clays = ParseClays(lines);
            var grid = Grid.Build(clays, waterSpring);

            Compute(grid, false);
            DryWater(grid, true);

            return grid
                .Flatten()
                .Where(x => x.type == Tile.Type.WetSand)
                .Count();
        }

        private static void Compute(Grid grid, bool debug = false)
        {
            var stack = new Stack<Tile>();
            stack.Push(grid.Get(waterSpring));

            var i = 0;
            var currentStep = printStep;
            while (stack.Count() > 0)
            {
                var current = stack.Pop();
                current.type = Tile.Type.WetSand;

                var below = current.GetBelowTile(grid);
                if (below != null)
                {
                    if (below.type == Tile.Type.Sand)
                    {
                        // Classic fall
                        stack.Push(below);
                        current.direction = Tile.Direction.Down;
                    }
                    else if (below.type == Tile.Type.Clay || (below.type == Tile.Type.WetSand && below.direction == Tile.Direction.Up))
                    {
                        var left = FillInDirection(grid, current, true, stack);
                        var right = FillInDirection(grid, current, false, stack);
                        if (left && right)
                        {
                            current.direction = Tile.Direction.Up;
                            stack.Push(current.GetUpperTile(grid));
                        }
                    }
                    else if (below.type == Tile.Type.WetSand)
                    {
                        current.direction = Tile.Direction.Down;
                        stack.Push(current.GetBelowTile(grid));
                    }
                }
                else {
                    current.direction = Tile.Direction.Down;
                }

                i++;
                // Precise debugging
                if (debug && i >= firstPrintStep && i % currentStep == 0)
                {
                    Console.WriteLine(i);
                    grid.Print(current, printRadius);
                    var input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        currentStep = int.Parse(input);
                    }
                }

                // Debug with animation
                //System.Threading.Thread.Sleep(50);
                //grid.Print(current, printRadius);
            }
        }

        private static bool FillInDirection(Grid grid, Tile start, bool left, Stack<Tile> stack)
        {
            var current = start;
            while (current != null && current.type != Tile.Type.Clay)
            {
                current.type = Tile.Type.WetSand;
                current.direction = left ? Tile.Direction.Left : Tile.Direction.Right;
                current = left ? current.GetLeftTile(grid) : current.GetRightTile(grid);
                var currentBelow = current.GetBelowTile(grid);

                if (currentBelow.type == Tile.Type.Sand)
                {
                    if (!stack.Contains(current))
                        stack.Push(current);
                    return false;
                }
                else if (currentBelow.type == Tile.Type.WetSand && currentBelow.direction == Tile.Direction.Down)
                {
                    current.type = Tile.Type.WetSand;
                    current.direction = Tile.Direction.Down;
                    return false;
                }
            }
            return true;
        }

        private static void DryWater(Grid grid, bool debug = false)
        {
            var wetTiles = grid
                .Flatten()
                .Where(x => x.type == Tile.Type.WetSand);

            foreach (var tile in wetTiles)
            {
                if (!HasUpTileInSameLine(grid, tile, true) && !HasUpTileInSameLine(grid, tile, false))
                    tile.type = Tile.Type.DriedSand;
            }
        }

        private static bool HasUpTileInSameLine(Grid grid, Tile tile, bool left)
        {
            var current = tile;
            while (current != null && current.type == Tile.Type.WetSand)
            {
                if (current.direction == Tile.Direction.Up)
                    return true;

                current = left ? current.GetLeftTile(grid) : current.GetRightTile(grid);
            }
            return false;
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
            public Direction direction;
            public int x;
            public int y;

            public Tile GetBelowTile(Grid grid)
            {
                return grid.Get(x, y + 1);
            }

            public Tile GetUpperTile(Grid grid)
            {
                return grid.Get(x, y - 1);
            }

            public Tile GetLeftTile(Grid grid)
            {
                return grid.Get(x - 1, y);
            }

            public Tile GetRightTile(Grid grid)
            {
                return grid.Get(x + 1, y);
            }

            public void Print(StringBuilder builder)
            {
                switch (type) {
                    case Type.Clay:
                        builder.Append("#");
                        break;
                    case Type.Sand:
                        builder.Append(".");
                        break;
                    case Type.DriedSand:
                        builder.Append("|");
                        break;
                    case Type.WetSand:
                        switch (direction)
                        {
                            case Direction.Up:
                                builder.Append("^");
                                break;
                            case Direction.Down:
                                builder.Append("v");
                                break;
                            case Direction.Left:
                                builder.Append("<");
                                break;
                            case Direction.Right:
                                builder.Append(">");
                                break;
                        }
                        break;
                }
            }

            public enum Type
            {
                Sand,
                Clay,
                WetSand,
                DriedSand,
            }

            public enum Direction
            {
                Up,
                Left,
                Right,
                Down,
            }
        }

        private class Grid
        {
            public Grid<Tile> values;
            public int x;
            public int y;

            public int minClayY;

            public static Grid Build(List<Position> clays, Position waterSpring)
            {
                var xMin = clays
                    .Select(x => x.x)
                    .Min();
                var xMax = clays
                    .Select(x => x.x)
                    .Max();

                var yMin = clays
                    .Select(x => x.y)
                    .Min();
                var yMax = clays
                    .Select(x => x.y)
                    .Max();

                var finalX = Math.Min(waterSpring.x, xMin);
                var finalY = Math.Min(waterSpring.y, yMin);

                var grid = new Grid<Tile>(xMax - finalX + 3, yMax - finalY + 1);

                // Init the grid
                for (var x = 0; x < grid.xLength; x++)
                {
                    for (var y = 0; y < grid.yLength; y++)
                    {
                        grid[x, y] = new Tile() {
                            type = Tile.Type.Sand,
                            x = x + finalX - 1,
                            y = y + finalY,
                        };
                    }
                }

                // Add the clay to the grid
                foreach (var clay in clays)
                {
                    grid[clay.x - finalX + 1, clay.y - finalY].type = Tile.Type.Clay;
                }

                // Add the waterSpring to the grid
                grid[waterSpring.x - finalX + 1, waterSpring.y - finalY].type = Tile.Type.Sand;

                return new Grid() {
                    values = grid,
                    x = finalX,
                    y = finalY,
                    minClayY = yMin,
                };
            }

            public Tile Get(int x, int y)
            {
                if (x - this.x < -1
                    || x - this.x >= values.xLength - 1
                    || y - this.y < 0
                    || y - this.y >= values.yLength)
                {
                    return null;
                }
                return values[x - this.x + 1, y - this.y];
            }

            public Tile Get(Position p)
            {
                return Get(p.x, p.y);
            }

            public Tile[] Flatten()
            {
                var xSize = values.xLength;

                var nbUselessY = minClayY - y;
                var ySize = values.yLength - nbUselessY;
                var flat = new Tile[xSize * ySize];

                for (var i = 0; i < xSize; i++)
                {
                    for (var j = 0; j < ySize; j++)
                    {
                        flat[i * ySize + j] = values[i, j + nbUselessY];
                    }
                }
                return flat;
            }

            public void Print(Tile current = null, int radius = -1)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;

                var xMin = radius == -1 ? 0 : Math.Max(current.x - radius - x, 0);
                var xMax = radius == -1 ? values.xLength : Math.Min(current.x + radius - x, values.xLength);

                var yMin = radius == -1 ? y : Math.Max(current.y - radius - y, 0);
                var yMax = radius == -1 ? values.yLength : Math.Min(current.y + radius - y, values.yLength);
                var builder = new StringBuilder();
                builder.Append("  ");
                for (var j = xMin; j < xMax; j++)
                {
                    builder.Append((j + x - 1) % 10);
                }
                Console.WriteLine(builder.ToString());

                for (var i = yMin; i < yMax; i++)
                {
                    builder.Clear();
                    builder.Append((i + y) % 10);
                    builder.Append(" ");
                    for (var j = xMin; j < xMax; j++)
                    {
                        if (values[j, i] == current)
                        {
                            builder.Append("x");
                        }
                        else
                        {
                            values[j, i].Print(builder);
                        }
                    }
                    Console.WriteLine(builder.ToString());
                }
                Console.WriteLine();
            }
        }
    }
}
