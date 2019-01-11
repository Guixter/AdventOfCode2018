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
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var line = Utils.GetLines(".\\Day20\\Input.txt")[0];

            Parse(line, out var root, out var allNodes);
            var grid = ToGrid(allNodes);
            //Print(grid);

            ComputeDistances(root, allNodes);
            //Print(grid, true);

            return allNodes
                .Select(x => (int) x.distance)
                .Max();
        }

        public static int Part2()
        {
            var line = Utils.GetLines(".\\Day20\\Input.txt")[0];

            Parse(line, out var root, out var allNodes);
            var grid = ToGrid(allNodes);
            //Print(grid);

            ComputeDistances(root, allNodes);
            //Print(grid, true);

            return allNodes
                .Select(x => (int) x.distance)
                .Where(x => x >= 1000)
                .Count();
        }

        private static void Parse(string line, out Room root, out HashSet<Room> allNodes)
        {
            root = new Room();
            var map = new Dictionary<Tuple<int, int>, Room>();
            allNodes = new HashSet<Room>();

            var currentNodes = new List<Room> { root };
            var stack = new Stack<Tuple<HashSet<Room>, HashSet<Room>>>();
            map.Add(new Tuple<int, int>(0, 0), root);
            allNodes.Add(root);
            for (var i = 1; i < line.Length - 1; i++)
            {
                var character = line[i];

                if (character == '(')
                {
                    var start = new HashSet<Room>();
                    start.UnionWith(currentNodes);
                    var end = new HashSet<Room>();
                    stack.Push(new Tuple<HashSet<Room>, HashSet<Room>>(start, end));
                }
                else if (character == ')')
                {
                    var tuple = stack.Pop();
                    tuple.Item2.UnionWith(currentNodes);
                    currentNodes = tuple.Item2.ToList();
                }
                else if (character == '|')
                {
                    var tuple = stack.Peek();
                    tuple.Item2.UnionWith(currentNodes);
                    currentNodes = tuple.Item1.ToList();
                }
                else
                {
                    for (var j = 0; j < currentNodes.Count; j++)
                    {
                        currentNodes[j] = HandleDirection(currentNodes[j], character, map);
                        allNodes.Add(currentNodes[j]);
                    }
                }
            }
        }

        // Return true if the node is deleted
        private static Room HandleDirection(Room currentNode, char character, Dictionary<Tuple<int, int>, Room> map)
        {
            // Get the new position
            var newX = currentNode.x;
            var newY = currentNode.y;

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
                    currentNode.north = newTile;
                    newTile.south = currentNode;
                    break;
                case 'S':
                    currentNode.south = newTile;
                    newTile.north = currentNode;
                    break;
                case 'W':
                    currentNode.west = newTile;
                    newTile.east = currentNode;
                    break;
                case 'E':
                    currentNode.east = newTile;
                    newTile.west = currentNode;
                    break;
            }

            return newTile;
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

        private static void Print(Room[,] grid, bool debug = false)
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
                    if (debug)
                    {
                        Console.Write((int) grid[j, i].distance % 10);
                    }
                    else
                    {
                        if (grid[j,i] == null)
                            Console.Write('#');
                        else if (grid[j,i].x == 0 && grid[j,i].y == 0)
                            Console.Write('X');
                        else
                            Console.Write('.');
                    }
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

        private static void ComputeDistances(Room root, HashSet<Room> allNodes)
        {
            // Init the distances
            foreach (var node in allNodes)
            {
                node.distance = float.PositiveInfinity;
            }

            // Iterate through the graph
            var queue = new Queue<Tuple<Room, float>>();
            queue.Enqueue(new Tuple<Room, float>(root, 0));
            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                var current = tuple.Item1;
                var currentDistance = tuple.Item2;

                if (currentDistance < current.distance)
                {
                    current.distance = currentDistance;
                    foreach (var neighbour in current.GetConnectedRooms())
                    {
                        queue.Enqueue(new Tuple<Room, float>(neighbour, currentDistance + 1));
                    }
                }
            }
        }

        private class Room
        {
            public int x;
            public int y;

            public int debug;
            public float distance;

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