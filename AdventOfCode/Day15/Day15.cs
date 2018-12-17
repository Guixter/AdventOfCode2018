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

            while (ProceedRound(grid, units)) {
                Print(grid);
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
                // Compute reachable squares
                var reachableSquares = inRangeSquares
                    .Where(s => IsReachable(s));

                if (reachableSquares.Count() == 0)
                    return false;

                // Find the nearest square
                foreach (var tile in reachableSquares)
                {
                    tile.distanceFlag = DistanceTo(tile.x, tile.y);
                }

                var nearestSquare = reachableSquares
                    .OrderBy(s => s.distanceFlag)
                    .First();

                // Take first priority nearest quare
                var nearSquares = reachableSquares
                    .Where(s => s.distanceFlag == nearestSquare.distanceFlag)
                    .ToList();
                nearSquares.Sort();

                // TODO : Perform a single step towards the chosen square
                // Note : don't do it if already in range !

                return true;
            }

            private int DistanceTo(int x, int y)
            {
                // TODO (right now we ignore obstacles)
                return Math.Abs(this.x - x) + Math.Abs(this.y - y);
            }

            private bool IsReachable(Tile t)
            {
                // TODO
                return true;
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