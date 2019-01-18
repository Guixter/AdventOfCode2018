using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day22
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Utils.GetLines(".\\Day22\\Input.txt");
            var grid = Grid.Parse(lines);
            grid.ComputeTiles(0, grid.targetX, 0, grid.targetY);
            //grid.Print();

            return grid
                .Flatten()
                .Select(x => (int) x.type)
                .Sum();
        }

        public static int Part2()
        {
            var lines = Utils.GetLines(".\\Day22\\Input.txt");
            var grid = Grid.Parse(lines);

            var result = grid.ComputeShortestPath(1, 7);
            //grid.Print(result);
            return result.time;
        }

        private class Tile
        {
            public int x;
            public int y;
            public long geologicIndex;
            public long erosionLevel;
            public Type type;

            private static readonly int erosionLevelValue = 20183;

            public void UpdateValues(Grid grid)
            {
                if ((x == 0 && y == 0) || (x == grid.targetX && y == grid.targetY))
                {
                    geologicIndex = 0;
                }
                else if (y == 0)
                {
                    geologicIndex = x * 16807;
                }
                else if (x == 0)
                {
                    geologicIndex = y * 48271;
                }
                else
                {
                    geologicIndex = grid[x - 1, y].erosionLevel * grid[x, y - 1].erosionLevel;
                }

                var temp = (geologicIndex + grid.depth) % erosionLevelValue + erosionLevelValue;
                erosionLevel = temp < erosionLevelValue ? temp : temp % 20183;

                type = (Type)(erosionLevel % 3);
            }

            public void Print(Grid grid)
            {
                if (x == 0 && y == 0)
                {
                    Console.Write("M");
                }
                else if (x == grid.targetX && y == grid.targetY)
                {
                    Console.Write("T");
                }
                else
                {
                    switch (type)
                    {
                        case Type.Narrow:
                            Console.Write("|");
                            break;
                        case Type.Wet:
                            Console.Write("=");
                            break;
                        case Type.Rocky:
                            Console.Write(".");
                            break;
                        default:
                            Console.Write("A");
                            break;
                    }
                }
            }

            public IEnumerable<Tile> GetNeighbours(Grid grid)
            {
                var list = new List<Tile>();

                list.Add(grid[x + 1, y]);
                list.Add(grid[x, y + 1]);

                if (x > 0)
                    list.Add(grid[x - 1, y]);
                if (y > 0)
                    list.Add(grid[x, y - 1]);
                return list;
            }

            public enum Type
            {
                Rocky,
                Wet,
                Narrow,
            }
        }

        private class Grid : IComparer<Step>, IComparer<Tile>
        {
            public int depth;
            public Tile target;
            public Dictionary<Tuple<int, int>, Tile> grid;
            public int targetX;
            public int targetY;

            private static readonly Regex depthRegex = new Regex(@"depth: (.*)");
            private static readonly Regex targetRegex = new Regex(@"target: (.*),(.*)");

            public static Grid Parse(string[] lines)
            {
                var result = new Grid();
                var depthMatch = depthRegex.Match(lines[0]);
                result.depth = int.Parse(depthMatch.Groups[1].Value);

                var targetMatch = targetRegex.Match(lines[1]);
                result.targetX = int.Parse(targetMatch.Groups[1].Value);
                result.targetY = int.Parse(targetMatch.Groups[2].Value);

                result.grid = new Dictionary<Tuple<int, int>, Tile>();
                result.target = result[result.targetX, result.targetY];

                return result;
            }

            private Tile ComputeTile(int x, int y)
            {
                var newTile = new Tile()
                {
                    x = x,
                    y = y,
                };
                newTile.UpdateValues(this);
                this[x, y] = newTile;

                return newTile;
            }

            public void ComputeTiles(int xMin, int xMax, int yMin, int yMax)
            {
                for (var i = xMin; i <= xMax; i++)
                {
                    for (var j = yMin; j <= yMax; j++)
                    {
                        ComputeTile(i, j);
                    }
                }
            }

            public Step ComputeShortestPath(int travelTime, int changeToolTime)
            {
                var queue = new SortedSet<Step>(this);
                queue.Add(new Step()
                {
                    tile = this[0, 0],
                    time = 0,
                    tool = Step.Tool.Torch,
                });

                while (queue.Count > 0)
                {
                    var current = queue.First();
                    queue.Remove(current);

                    if (IsToolAdequate(current.tool, current.tile.type))
                    {
                        if (current.tile == target)
                        {
                            if (current.tool == Step.Tool.Torch)
                            {
                                return current;
                            }
                            else
                            {
                                queue.Add(new Step()
                                {
                                    tile = current.tile,
                                    time = current.time + changeToolTime,
                                    tool  = Step.Tool.Torch,
                                    parent = current,
                                });
                            }
                        }
                        else
                        {
                            // Move to another tile
                            foreach (var neighbour in current.tile.GetNeighbours(this))
                            {
                                var newStep = new Step()
                                {
                                    tile = neighbour,
                                    time = current.time + travelTime,
                                    tool = current.tool,
                                    parent = current,
                                };
                                var test = queue.Add(newStep);
                            }
                        }
                    }
                    else
                    {
                        // Change the tool
                        foreach (var tool in GetAdequateTools(current.tile.type))
                        {
                            queue.Add(new Step() {
                                tile = current.tile,
                                time = current.time + changeToolTime,
                                tool = tool,
                                parent = current,
                            });
                        }
                    }
                }

                return null;
            }

            private bool IsToolAdequate(Step.Tool tool, Tile.Type type)
            {
                switch (type)
                {
                    case Tile.Type.Narrow:
                        return tool != Step.Tool.ClimbingGear;
                    case Tile.Type.Rocky:
                        return tool != Step.Tool.None;
                    case Tile.Type.Wet:
                        return tool != Step.Tool.Torch;
                    default:
                        return false;
                }
            }

            private Step.Tool[] GetAdequateTools(Tile.Type type)
            {
                switch (type)
                {
                    case Tile.Type.Narrow:
                        return new Step.Tool[] { Step.Tool.None, Step.Tool.Torch };
                    case Tile.Type.Rocky:
                        return new Step.Tool[] { Step.Tool.ClimbingGear, Step.Tool.Torch };
                    case Tile.Type.Wet:
                        return new Step.Tool[] { Step.Tool.None, Step.Tool.ClimbingGear };
                    default:
                        return null;
                }
            }

            public void Print(int x, int y, int maxX, int maxY)
            {
                for (var i = 0; i <= maxY; i++)
                {
                    for (var j = 0; j <= maxX; j++)
                    {
                        if (j == x && i == y)
                            Console.Write('X');
                        else
                            this[j, i].Print(this);
                    }
                    Console.WriteLine();
                }
            }

            public void Print(int x = 0, int y = 0)
            {
                Print(x, y, targetX, targetY);
            }

            public void Print(Step step)
            {
                var list = new List<Step>();
                while (step != null)
                {
                    list.Add(step);
                    step = step.parent;
                }

                list.Reverse();


                foreach (var s in list)
                {
                    Print(s.tile.x, s.tile.y, targetX + 5, targetY + 5);
                    Console.WriteLine(s.time);
                    Console.WriteLine(s.tool.ToString());
                    Console.ReadLine();
                }
            }

            public Tile this[int x, int y]
            {
                get
                {
                    return grid.ContainsKey(new Tuple<int, int>(x, y)) ? grid[new Tuple<int, int>(x, y)] : ComputeTile(x, y);
                }
                set
                {
                    grid[new Tuple<int, int>(x, y)] = value;
                }
            }

            public IEnumerable<Tile> Flatten()
            {
                var result = new Tile[grid.Count];

                var i = 0;
                foreach (var couple in grid)
                {
                    result[i++] = couple.Value;
                }

                return result;
            }

            public int Compare(Step x, Step y)
            {
                var timeComparison = x.time.CompareTo(y.time);
                if (timeComparison != 0)
                    return timeComparison;

                var toolComparison = x.tool.CompareTo(y.tool);
                if (toolComparison != 0)
                    return toolComparison;

                return Compare(x.tile, y.tile);
            }

            public int Compare(Tile x, Tile y)
            {
                var xComparison = x.x.CompareTo(y.x);
                if (xComparison != 0)
                    return xComparison;

                return x.y.CompareTo(y.y);
            }
        }

        private class Step
        {
            public Tile tile;
            public Tool tool;
            public int time;

            public Step parent;

            public enum Tool
            {
                ClimbingGear,
                Torch,
                None,
            }
        }
    }
}