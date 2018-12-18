using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day15
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var lines = Program.GetLines(".\\Day15\\testMove.txt");

            ParseGrid(lines, out var grid, out var units);
            Print(grid);

            for (var i = 0; i < 1; i++)
            //while (ProceedRound(grid, units))
            {
                ProceedRound(grid, units);
            }

            return 0;
        }

        private static bool ProceedRound(Tile[,] grid, List<Unit> units)
        {
            units.Sort();

            foreach (var unit in units)
            {
                if (!unit.TakeTurn(grid, units))
                    return false;

                Print(grid);
            }

            return true;
        }

        private static void Print(Tile[,] grid)
        {
            for (var i = 0; i < grid.GetLength(1); i++)
            {
                for (var j = 0; j < grid.GetLength(0); j++)
                {
                    grid[j, i].Print();
                }
                Console.WriteLine();
            }
        }

        private static void ParseGrid(string[] lines, out Tile[,] grid, out List<Unit> units)
        {
            units = new List<Unit>();
            grid = new Tile[lines[0].Length, lines.Length];

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    var character = lines[j][i];
                    grid[i, j] = Tile.Parse(character, i, j);

                    var unit = Unit.Parse(character, i, j, grid[i,j]);
                    if (unit != null)
                        units.Add(unit);
                }
            }
        }

        private class Tile : IComparable<Tile>
        {
            public int x;
            public int y;
            public Type type;
            public Unit unit;

            public int distanceFlag;

            public int DistanceTo(Tile tile)
            {
                // TODO (obstacles)
                return Math.Abs(x - tile.x) + Math.Abs(y - tile.y);
            }

            public IEnumerable<Tile> GetAdjacentSquares(Tile[,] grid)
            {
                var tiles = new List<Tile>();
                if (x > 0)
                    tiles.Add(grid[x - 1, y]);
                if (x < grid.GetLength(0) - 1)
                    tiles.Add(grid[x + 1, y]);
                if (y > 0)
                    tiles.Add(grid[x, y - 1]);
                if (y < grid.GetLength(1) - 1)
                    tiles.Add(grid[x, y + 1]);

                return tiles;
            }

            public static void ComputeDistances(IEnumerable<Tile> tiles, Tile target)
            {
                foreach (var tile in tiles)
                {
                    tile.distanceFlag = tile.DistanceTo(target);
                }
            }

            public static Tile Parse(char character, int x, int y)
            {
                return new Tile() {
                    x = x,
                    y = y,
                    type = character == '#' ? Type.WALL : Type.EMPTY,
                };
            }

            public int CompareTo(Tile other)
            {
                var compareY = y.CompareTo(other.y);
                return compareY == 0 ? x.CompareTo(other.x) : compareY;
            }

            public void Print()
            {
                if (unit != null)
                {
                    unit.Print();
                }
                else
                {
                    if (type == Type.EMPTY)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Console.Write("#");
                    }
                }
            }

            public enum Type
            {
                EMPTY,
                WALL,
            }
        }

        private class Unit : IComparable<Unit>
        {
            public int x;
            public int y;
            public Type type;
            public Tile tile;

            private IEnumerable<Unit> possibleTargets;
            private IEnumerable<Tile> inRangeSquares;

            // Return false when the fight ends
            public bool TakeTurn(Tile[,] grid, List<Unit> units)
            {
                if (!IdentifyTargets(grid, units))
                    return false;

                if (!ComputeInRangeSquares(grid, units))
                    return true;

                if (!IsInRange(grid, units) && !Move(grid, units))
                    return true;

                Attack(grid, units);
                return true;
            }

            // Return false when there is no enemy target
            private bool IdentifyTargets(Tile[,] grid, List<Unit> units)
            {
                possibleTargets = units
                    .Where(x => x.type != type);
                return possibleTargets.Count() > 0;
            }

            // Return false when there is no in range square
            private bool ComputeInRangeSquares(Tile[,] grid, List<Unit> units)
            {
                inRangeSquares = possibleTargets
                    .Select(t => new Tile[] {
                        grid[t.x + 1, t.y],
                        grid[t.x - 1, t.y],
                        grid[t.x, t.y + 1],
                        grid[t.x, t.y - 1],
                    })
                    .SelectMany(s => s)
                    .Where(s => s.type == Tile.Type.EMPTY && (s.unit == null || s.unit == this));
                return inRangeSquares.Count() > 0;
            }

            private bool IsInRange(Tile[,] grid, List<Unit> units)
            {
                return inRangeSquares
                    .Where(s => s.unit == this)
                    .Count() > 0;
            }

            // Return false when there is no reachable square
            private bool Move(Tile[,] grid, List<Unit> units)
            {
                Tile.ComputeDistances(inRangeSquares, tile);

                var reachableSquares = inRangeSquares
                    .Where(s => s.distanceFlag >= 0);

                if (reachableSquares.Count() == 0)
                    return false;

                var nearestSquare = FindBestSquare(reachableSquares);

                PerformStepTowards(grid, nearestSquare);

                return true;
            }

            private static Tile FindBestSquare(IEnumerable<Tile> squares)
            {
                // Find the nearest square
                var nearestSquare = squares
                    .OrderBy(s => s.distanceFlag)
                    .First();

                // Take read order first priority nearest quare
                var nearSquares = squares
                    .Where(s => s.distanceFlag == nearestSquare.distanceFlag)
                    .ToList();
                nearSquares.Sort();
                return nearSquares.First();
            }

            private void PerformStepTowards(Tile[,] grid, Tile target)
            {
                if (target.distanceFlag > 1)
                {
                    var adjacentSquares = tile.GetAdjacentSquares(grid);
                    Tile.ComputeDistances(adjacentSquares, target);

                    var bestSquare = FindBestSquare(adjacentSquares);

                    MoveTo(bestSquare);
                }
            }

            private void MoveTo(Tile target)
            {
                if (target.unit != null)
                    throw new Exception("Destination target already has a unit");

                target.unit = this;
                tile.unit = null;

                x = target.x;
                y = target.y;

                tile = target;
            }

            private void Attack(Tile[,] grid, List<Unit> units)
            {

            }

            public static Unit Parse(char character, int x, int y, Tile tile)
            {
                if (character == 'G' || character == 'E')
                {
                    tile.unit = new Unit()
                    {
                        x = x,
                        y = y,
                        type = character == 'G' ? Type.GOBLIN : Type.ELF,
                        tile = tile,
                    };
                    return tile.unit;
                }
                else
                {
                    return null;
                }
            }

            public void Print()
            {
                if (type == Type.ELF)
                {
                    Console.Write("E");
                }
                else
                {
                    Console.Write("G");
                }
            }

            public int CompareTo(Unit other)
            {
                var compareY = y.CompareTo(other.y);
                return compareY == 0 ? x.CompareTo(other.x) : compareY;
            }

            public enum Type
            {
                ELF,
                GOBLIN,
            }
        }
    }
}