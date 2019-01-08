using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day20
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var line = Utils.GetLines(".\\Day20\\test5.txt")[0];

            Parse(line, out var root, out var allNodes);
            var grid = ToGrid(allNodes);
            Print(grid);

            return 0;
        }

        private static void Parse(string line, out Room root, out HashSet<Room> allNodes)
        {
            root = new Room();
            var map = new Dictionary<Tuple<int, int>, Room>();
            allNodes = new HashSet<Room>();

            var currentNodes = new Queue<Tuple<Room, int>>();
            currentNodes.Enqueue(new Tuple<Room, int>(root, 1));
            map.Add(new Tuple<int, int>(0, 0), root);
            while (currentNodes.Count > 0)
            {
                var tuple = currentNodes.Dequeue();
                var current = tuple.Item1;
                var i = tuple.Item2;
                var character = line[i];
                allNodes.Add(current);

                if (character == '$') {
                    continue;
                }
                else if (character == '(')
                {
                    currentNodes.Enqueue(new Tuple<Room, int>(current, i + 1));
                    var j = i + 1;
                    var nbBranches = 1;
                    while (nbBranches > 0)
                    {
                        if (line[j] == '|' && nbBranches == 1)
                            currentNodes.Enqueue(new Tuple<Room, int>(current, j + 1));
                        if (line[j] == '(')
                            nbBranches++;
                        if (line[j] == ')')
                            nbBranches--;
                        j++;
                    }
                }
                else if (character == ')')
                {
                    currentNodes.Enqueue(new Tuple<Room, int>(current, i + 1));
                }
                else if (character == '|')
                {
                    var j = i + 1;
                    var nbBranches = 1;
                    while (nbBranches > 0)
                    {
                        if (line[j] == '(')
                            nbBranches++;
                        if (line[j] == ')')
                            nbBranches--;
                        j++;
                    }

                    currentNodes.Enqueue(new Tuple<Room, int>(current, j));
                }
                else
                {
                    // Get the new position
                    var newX = current.x;
                    var newY = current.y;

                    switch(character)
                    {
                        case 'N':
                            newY--;
                            break;
                        case 'S':
                            newY++;
                            break;
                        case 'W':
                            newX--;
                            break;
                        case 'E':
                            newX++;
                            break;
                    }

                    // Get the new tile
                    var newPosition = new Tuple<int, int>(newX, newY);
                    if (!map.TryGetValue(newPosition, out var newTile))
                    {
                        newTile = new Room() {
                            x = newX,
                            y = newY,
                            debug = map.Count,
                        };
                        map.Add(newPosition, newTile);
                    }

                    // Update the links
                    switch(character)
                    {
                        case 'N':
                            current.north = newTile;
                            newTile.south = current;
                            break;
                        case 'S':
                            current.south = newTile;
                            newTile.north = current;
                            break;
                        case 'W':
                            current.west = newTile;
                            newTile.east = current;
                            break;
                        case 'E':
                            current.east = newTile;
                            newTile.west = current;
                            break;
                    }

                    currentNodes.Enqueue(new Tuple<Room, int>(newTile, i + 1));
                }
            }
        }

        private static Room[,] ToGrid(HashSet<Room> allNodes)
        {
            var xMin = allNodes.Select(x => x.x).Min();
            var xMax = allNodes.Select(x => x.x).Max();
            var yMin = allNodes.Select(x => x.y).Min();
            var yMax = allNodes.Select(x => x.y).Max();

            var lengthX = xMax - xMin + 1;
            var lengthY = yMax - yMin + 1;
            var result = new Room[lengthX, lengthY];

            foreach (var node in allNodes)
            {
                result[node.x - xMin, node.y - yMin] = node;
            }

            return result;
        }

        private static void Print(Room[,] grid)
        {
            // First row
            Console.Write('#');
            for (var j = 0; j < grid.GetLength(0); j++)
            {
                Console.Write("##");
            }
            Console.WriteLine();

            for (var i = 0; i < grid.GetLength(1); i++)
            {
                Console.Write('#');
                for (var j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j,i].x == 0 && grid[j,i].y == 0)
                        Console.Write('X');
                    else
                        Console.Write('.');
                    if (grid[j, i] != null && grid[j,i].east != null)
                    {
                        Console.Write('|');
                    }
                    else
                    {
                        Console.Write('#');
                    }
                }
                Console.WriteLine();

                Console.Write('#');
                for (var j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j, i] != null && grid[j, i].south != null)
                    {
                        Console.Write('-');
                    }
                    else
                    {
                        Console.Write('#');
                    }
                    Console.Write('#');
                }
                Console.WriteLine();
            }
        }

        private class Room
        {
            public int x;
            public int y;

            public int debug;

            public Room north;
            public Room south;
            public Room east;
            public Room west;

            public List<Room> GetConnectedRooms()
            {
                var list = new List<Room>();

                if (north != null)
                    list.Add(north);
                if (south != null)
                    list.Add(south);
                if (west != null)
                    list.Add(west);
                if (east != null)
                    list.Add(east);

                return list;
            }
        }
    }
}