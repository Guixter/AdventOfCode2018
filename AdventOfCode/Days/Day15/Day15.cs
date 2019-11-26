using AdventOfCodeTools;
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
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day15\Input.txt");

            var i = SimulateFight(lines, out var grid, out var units);

            Console.WriteLine(i);
            PrintGrid(grid);

            return i * units.Sum(x => x.hitPoints);
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day15\Input.txt");

            int i;
            Grid<Tile> grid;
            List<Unit> units;
            var startingAttack = Unit.defaultAttackPower;
            do
            {
                i = SimulateFight(lines, out grid, out units, startingAttack++);
            } while (units.Where(x => x.type == Unit.Type.ELF && x.hitPoints == 0).Count() > 0);

            Console.WriteLine(i + " / " + (startingAttack - 1));
            PrintGrid(grid);

            return i * units.Sum(x => x.hitPoints);
        }

        private static int SimulateFight(string[] lines, out Grid<Tile> grid, out List<Unit> units, int elvesAttackPower = -1)
        {
            ParseGrid(lines, out grid, out units, elvesAttackPower);

            var i = 0;
            while (ProceedRound(grid, units))
            {
                i++;
            }

            return i;
        }

        private static bool ProceedRound(Grid<Tile> grid, List<Unit> units)
        {
            units.Sort();

            foreach (var unit in units)
            {
                if (!unit.TakeTurn(grid, units))
                    return false;
            }

            return true;
        }

        private static void PrintGrid(Grid<Tile> grid)
        {
            for (var y = 0; y < grid.yLength; y++)
            {
                for (var x = 0; x < grid.xLength; x++)
                {
                    grid[x, y].Print();
                }
                Console.Write("  ");
                for (var x = 0; x < grid.xLength; x++)
                {
                    if (grid[x,y].scoreFlag == -1)
                        Console.Write("# ");
                    else
                        Console.Write(grid[x,y].scoreFlag % 10 + " ");
                }
                Console.Write("  ");
                for (var x = 0; x < grid.xLength; x++)
                {
                    if (grid[x,y].unit == null)
                        Console.Write("# ");
                    else
                        Console.Write(grid[x,y].unit.hitPoints % 10 + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void ParseGrid(string[] lines, out Grid<Tile> grid, out List<Unit> units, int elvesAttackPower = -1)
        {
            units = new List<Unit>();
            grid = new Grid<Tile>(lines[0].Length, lines.Length);

            for (var x = 0; x < grid.xLength; x++)
            {
                for (var y = 0; y < grid.yLength; y++)
                {
                    var character = lines[y][x];
                    grid[x, y] = Tile.Parse(character, x, y);

                    var unit = Unit.Parse(character, x, y, grid[x,y], elvesAttackPower);
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

            public int scoreFlag;

            public int DistanceTo(Tile tile)
            {
                // TODO (obstacles)
                return Math.Abs(x - tile.x) + Math.Abs(y - tile.y);
            }

            public IEnumerable<Tile> GetAdjacentSquares(Grid<Tile> grid)
            {
                var tiles = new List<Tile>();
                if (x > 0)
                    tiles.Add(grid[x - 1, y]);
                if (x < grid.xLength - 1)
                    tiles.Add(grid[x + 1, y]);
                if (y > 0)
                    tiles.Add(grid[x, y - 1]);
                if (y < grid.yLength - 1)
                    tiles.Add(grid[x, y + 1]);

                return tiles;
            }

            public static void ComputeScoreFlag(IEnumerable<Tile> tiles, Func<Tile, int> selector)
            {
                foreach (var tile in tiles)
                {
                    tile.scoreFlag = selector(tile);
                }
            }

            public static void ComputeDistances(Grid<Tile> grid, Tile from)
            {
                // Initialization
                for (var x = 0; x < grid.xLength; x++)
                {
                    for (var y = 0; y < grid.yLength; y++)
                    {
                        grid[x, y].scoreFlag = -1;
                    }
                }

                // Iterate through the elements
                var queue = new Queue<Tuple<Tile, int>>();
                queue.Enqueue(new Tuple<Tile, int>(from, 0));
                while (queue.Count() > 0)
                {
                    var current = queue.Dequeue();
                    
                    if (current.Item1.scoreFlag == -1)
                    {
                        current.Item1.scoreFlag = current.Item2;

                        var availableAdjacentSquares = current.Item1
                            .GetAdjacentSquares(grid)
                            .Where(x => x.type == Type.EMPTY && x.unit == null);

                        foreach (var square in availableAdjacentSquares)
                        {
                            queue.Enqueue(new Tuple<Tile, int>(square, current.Item2 + 1));
                        }
                    }
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
            public int hitPoints;
            public int attackPower;
            public bool dead;

            private IEnumerable<Unit> possibleTargets;
            private IEnumerable<Tile> inRangeSquares;

            public static readonly int defaultHitPoints = 200;
            public static readonly int defaultAttackPower = 3;

            // Return false when the fight ends
            public bool TakeTurn(Grid<Tile> grid, List<Unit> units)
            {
                if (!dead)
                {
                    if (!IdentifyTargets(grid, units))
                    return false;

                    if (!ComputeInRangeSquares(grid, units))
                        return true;

                    if (!IsInRange(grid, units) && !Move(grid, units))
                        return true;

                    Attack(grid, units);
                }
                return true;
            }

            // Return false when there is no enemy target
            private bool IdentifyTargets(Grid<Tile> grid, List<Unit> units)
            {
                possibleTargets = units
                    .Where(x => !x.dead && x.type != type);
                return possibleTargets.Count() > 0;
            }

            // Return false when there is no in range square
            private bool ComputeInRangeSquares(Grid<Tile> grid, List<Unit> units)
            {
                inRangeSquares = possibleTargets
                    .Select(t => t.tile.GetAdjacentSquares(grid))
                    .SelectMany(s => s)
                    .Where(s => s.type == Tile.Type.EMPTY && (s.unit == null || s.unit == this));
                return inRangeSquares.Count() > 0;
            }

            private bool IsInRange(Grid<Tile> grid, List<Unit> units)
            {
                return inRangeSquares
                    .Where(s => s.unit == this)
                    .Count() > 0;
            }

            // Return false when there is no reachable square
            private bool Move(Grid<Tile> grid, List<Unit> units)
            {
                Tile.ComputeDistances(grid, tile);

                var reachableSquares = inRangeSquares
                    .Where(s => s.scoreFlag >= 0);

                if (reachableSquares.Count() == 0)
                    return false;

                var nearestSquare = FindBestSquare(reachableSquares);

                PerformStepTowards(grid, nearestSquare);

                return true;
            }

            // Compute the best scored square, and selects the read-first if tie
            private static Tile FindBestSquare(IEnumerable<Tile> squares)
            {
                if (squares.Count() == 0)
                    return null;
                // Find the nearest square
                var nearestSquare = squares
                    .OrderBy(s => s.scoreFlag)
                    .First();

                // Take read order first priority nearest quare
                var nearSquares = squares
                    .Where(s => s.scoreFlag == nearestSquare.scoreFlag)
                    .ToList();
                nearSquares.Sort();
                return nearSquares.First();
            }

            private void PerformStepTowards(Grid<Tile> grid, Tile target)
            {
                if (target.scoreFlag > 0)
                {
                    var adjacentSquares = tile
                        .GetAdjacentSquares(grid)
                        .Where(s => s.type == Tile.Type.EMPTY && s.unit == null);
                    Tile.ComputeDistances(grid, target);
                    adjacentSquares = adjacentSquares
                        .Where(s => s.scoreFlag != -1);

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

            private void Attack(Grid<Tile> grid, List<Unit> units)
            {
                var adjacentTargetsSquares = tile
                    .GetAdjacentSquares(grid)
                    .Where(x => x.unit != null && x.unit.type != type);

                Tile.ComputeScoreFlag(adjacentTargetsSquares, t => t.unit.hitPoints);

                if (adjacentTargetsSquares.Count() > 0)
                    FindBestSquare(adjacentTargetsSquares).unit.UndergoAttack(this);
            }

            private void UndergoAttack(Unit attacker)
            {
                hitPoints -= attacker.attackPower;
                if (hitPoints <= 0)
                {
                    hitPoints = 0;
                    dead = true;
                    tile.unit = null;
                }
            }

            public static Unit Parse(char character, int x, int y, Tile tile, int attackPower = -1)
            {
                if (character == 'G' || character == 'E')
                {
                    tile.unit = new Unit()
                    {
                        x = x,
                        y = y,
                        type = character == 'G' ? Type.GOBLIN : Type.ELF,
                        tile = tile,
                        attackPower = (attackPower == -1 || character == 'G') ? defaultAttackPower : attackPower,
                        hitPoints = defaultHitPoints,
                        dead = false,
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