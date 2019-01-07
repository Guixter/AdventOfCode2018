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
            var line = Utils.GetLines(".\\Day20\\test2.txt")[0];

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

            var current = root;
            map.Add(new Tuple<int, int>(0, 0), current);
            for (var i = 1; i < line.Length - 1; i++)
            {
                allNodes.Add(current);

                // Get the new position
                var newX = current.x;
                var newY = current.y;

                switch(line[i])
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
                switch(line[i])
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

                current = newTile;
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