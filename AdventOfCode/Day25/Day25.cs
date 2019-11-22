using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day25
    {
        public static readonly int maximumConstellationDistance = 3;

        public static void Run()
        {
            Console.WriteLine(Part1());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day25\Input.txt");
            var points = Point.Parse(lines);

            var constellations = new List<Constellation>();
            foreach (var point in points)
            {
                var nearConstellations = new List<Constellation>();
                foreach (var constellation in constellations)
                {
                    if (constellation.TryAddPoint(point))
                        nearConstellations.Add(constellation);
                }

                if (nearConstellations.Count == 0)
                    constellations.Add(new Constellation(point));
                else if (nearConstellations.Count > 1)
                    Constellation.Merge(constellations, nearConstellations);
            }

            return constellations.Count;
        }

        private class Point
        {
            public int x;
            public int y;
            public int z;
            public int t;

            private static readonly Regex regex = new Regex(@"(.*),(.*),(.*),(.*)");

            public static Point Parse(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    return new Point() {
                        x = int.Parse(match.Groups[1].Value),
                        y = int.Parse(match.Groups[2].Value),
                        z = int.Parse(match.Groups[3].Value),
                        t = int.Parse(match.Groups[4].Value),
                    };
                }
                else
                {
                    return new Point();
                }
            }

            public static Point[] Parse(string[] lines)
            {
                var result = new Point[lines.Length];

                for (var i = 0; i < lines.Length; i++)
                {
                    result[i] = Parse(lines[i]);
                }

                return result;
            }

            public int ManhattanDistance(Point other)
            {
                return Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z) + Math.Abs(t - other.t);
            }
        }

        private class Constellation
        {
            public List<Point> points;

            public Constellation(Point point)
            {
                points = new List<Point>() { point };
            }

            public Constellation()
            {
                points = new List<Point>();
            }

            public bool TryAddPoint(Point point)
            {
                foreach (var p in points)
                {
                    if (p.ManhattanDistance(point) <= maximumConstellationDistance)
                    {
                        points.Add(point);
                        return true;
                    }
                }

                return false;
            }

            public static void Merge(List<Constellation> all, List<Constellation> toMerge)
            {
                var newConstellation = new Constellation();
                foreach (var c in toMerge)
                {
                    newConstellation.points.AddRange(c.points);
                    all.Remove(c);
                }
                newConstellation.points = newConstellation.points
                    .Distinct()
                    .ToList();
                all.Add(newConstellation);
            }
        }
    }
}