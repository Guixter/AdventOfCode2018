using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day23
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day23\Input.txt");

            var bots = Bot.Parse(lines);

            var bestBot = bots
                .OrderByDescending(x => x.radius)
                .First();

            return bots
                .Where(x => x.DistanceTo(bestBot) <= bestBot.radius)
                .Count();
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day23\Input.txt");

            var bots = Bot.Parse(lines);
            var box = ComputeBestBox(bots);

            return box.distanceToOrigin;
        }

        private static Box ComputeBestBox(Bot[] bots)
        {
            // Compute the initial box
            var xMin = bots.Select(x => x.x - x.radius).Min();
            var yMin = bots.Select(x => x.y - x.radius).Min();
            var zMin = bots.Select(x => x.z - x.radius).Min();
            var xMax = bots.Select(x => x.x + x.radius).Max();
            var yMax = bots.Select(x => x.y + x.radius).Max();
            var zMax = bots.Select(x => x.z + x.radius).Max();
            var maxSize = Math.Max(xMax - xMin, Math.Max(yMax - yMin, zMax - zMin));
            var upperTwoPower = (int) Math.Pow(2, Math.Ceiling(Math.Log(maxSize, 2)));
            var box = new Box(bots, xMin, yMin, zMin, upperTwoPower);

            var queue = new SortedSet<Box>() { box };
            while (queue.Count() != 0)
            {
                var currentBox = queue.First();
                queue.Remove(currentBox);

                if (currentBox.size == 1)
                {
                    box = currentBox;
                    break;
                }

                // Split into 8 sub-boxes
                var subBoxes = new Box[8];
                var size = currentBox.size / 2;
                var x = currentBox.x;
                var y = currentBox.y;
                var z = currentBox.z;
                subBoxes[0] = new Box(bots, x, y, z, size);
                subBoxes[1] = new Box(bots, x + size, y, z, size);
                subBoxes[2] = new Box(bots, x, y + size, z, size);
                subBoxes[3] = new Box(bots, x, y, z + size, size);
                subBoxes[4] = new Box(bots, x + size, y + size, z, size);
                subBoxes[5] = new Box(bots, x + size, y, z + size, size);
                subBoxes[6] = new Box(bots, x, y + size, z + size, size);
                subBoxes[7] = new Box(bots, x + size, y + size, z + size, size);

                // Push the subboxes into queue
                for (var i = 0; i < 8; i++)
                {
                    queue.Add(subBoxes[i]);
                }
            }

            return box;
        }

        private class Bot
        {
            public int x;
            public int y;
            public int z;
            public int radius;

            private static readonly Regex regex = new Regex(@"pos=<(.*),(.*),(.*)>, r=(.*)");

            public static Bot[] Parse(string[] lines)
            {
                var result = new Bot[lines.Length];

                for (var i = 0; i < lines.Length; i++)
                {
                    result[i] = Parse(lines[i]);
                }

                return result;
            }

            public static Bot Parse(string line)
            {
                var firstMatch = regex.Match(line);
                if (firstMatch.Success)
                {
                    return new Bot() {
                        x = int.Parse(firstMatch.Groups[1].Value),
                        y = int.Parse(firstMatch.Groups[2].Value),
                        z = int.Parse(firstMatch.Groups[3].Value),
                        radius = int.Parse(firstMatch.Groups[4].Value),
                    };
                }
                else
                {
                    return new Bot();
                }
            }

            public int DistanceTo(Bot other)
            {
                return DistanceTo(other.x, other.y, other.z);
            }

            public int DistanceTo(int x, int y, int z)
            {
                return Math.Abs(x - this.x) + Math.Abs(y - this.y) + Math.Abs(z - this.z);
            }
        }

        private class Box : IComparable<Box>
        {
            public int x;
            public int y;
            public int z;
            public int size;
            public int distanceToOrigin;
            public int nbIntersectingBots;

            public Box(Bot[] bots, int x, int y, int z, int size)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.size = size;

                distanceToOrigin = Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
                nbIntersectingBots = ComputeNbIntersectingBots(bots);
            }

            private int ComputeNbIntersectingBots(Bot[] bots)
            {
                var cpt = 0;
                foreach (var bot in bots)
                {
                    if (DistanceTo(bot) <= bot.radius)
                        cpt++;
                }
                return cpt;
            }

            private int DistanceTo(Bot bot)
            {
                return DimensionalDistance(x, bot.x) + DimensionalDistance(y, bot.y) + DimensionalDistance(z, bot.z);
            }

            // Distance to the box for a specific dimension
            private int DimensionalDistance(int boxVal, int val)
            {
                var lowBorder = boxVal;
                var highBorder = boxVal + size - 1;

                if (val < lowBorder)
                    return lowBorder - val;
                if (val > highBorder)
                    return val - highBorder;
                return 0;
            }

            public int CompareTo(Box other)
            {
                // Check the number of intersecting bots
                var bots = - nbIntersectingBots.CompareTo(other.nbIntersectingBots);
                if (bots != 0)
                    return bots;

                // If there is a tie, check the distance to origin
                var distance = distanceToOrigin.CompareTo(other.distanceToOrigin);
                if (distance != 0)
                    return distance;

                // If there is a tie, check the size of the box
                return size.CompareTo(other.size);
            }
        }
    }
}