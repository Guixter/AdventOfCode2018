using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace AdventOfCode
{
    class Day6
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day6\Input.txt");

            // Parse the points
            var nbPoints = lines.Length;
            var points = new Point[nbPoints];
            for (var i = 0; i < nbPoints; i++)
            {
                points[i].Parse(lines[i], i);
            }

            GetBoxSize(points, out var width, out var height, out var left, out var top);

            // We use a grid of size (width+2,height+2) to include the first external border in the computations
            var grid = new Grid<Cell>(width + 2, height + 2);
            var scores = new int[points.Count()];
            for (var i = 0; i < grid.xLength; i++)
            {
                for (var j = 0; j < grid.yLength; j++)
                {
                    var currentMinDistance = float.PositiveInfinity;
                    var currentClosestPoint = -1;
                    foreach (var point in points)
                    {
                        var distance = MathUtils.ManhattanDistance(point.pos, new float2(i + left - 1, j + top - 1));
                        if (distance < currentMinDistance)
                        {
                            if (currentClosestPoint != -1)
                                scores[currentClosestPoint]--;
                            currentMinDistance = distance;
                            currentClosestPoint = point.id;
                            scores[currentClosestPoint]++;
                        }
                        else if (distance == currentMinDistance)
                        {
                            if (currentClosestPoint != -1)
                                scores[currentClosestPoint]--;
                            currentClosestPoint = -1;
                        }
                    }
                    grid[i, j] = new Cell
                    {
                        distanceToClosestPoint = (int)currentMinDistance,
                        closestPointId = currentClosestPoint
                    };
                }
            }

            // Determine the finite points (those who don't appear in the external border)
            var lastIndexX = grid.xLength - 1;
            var lastIndexY = grid.yLength - 1;
            for (var i = 0; i <= lastIndexX; i++)
            {
                if (grid[i, 0].closestPointId != -1)
                    points[grid[i, 0].closestPointId].infiniteArea = true;

                if (grid[i, lastIndexY].closestPointId != -1)
                    points[grid[i, lastIndexY].closestPointId].infiniteArea = true;
            }
            for (var j = 0; j <= lastIndexY; j++)
            {
                if (grid[0, j].closestPointId != -1)
                    points[grid[0, j].closestPointId].infiniteArea = true;

                if (grid[lastIndexX, j].closestPointId != -1)
                    points[grid[lastIndexX, j].closestPointId].infiniteArea = true;
            }
            var finitePoints = points
                .Select((x, index) => new Tuple<int, Point>(index, x))
                .Zip(scores, (x, y) => new Tuple<int, Point, int>(x.Item1, x.Item2, y))
                .Where(x => !x.Item2.infiniteArea)
                .ToArray();

            return finitePoints.OrderByDescending(x => x.Item3).First().Item3;
        }

        private static void Print(Point[] points, Grid<Cell> grid)
        {
            grid.Print((Cell cell, int x, int y) =>
            {
                if (cell.closestPointId != -1 && MathUtils.ManhattanDistance(points[cell.closestPointId].pos, new float2(x, y)) == 0)
                    return (char)(cell.closestPointId + 'A');
                else
                    return (char)(cell.closestPointId + 'a');
            });

            grid.Print((Cell cell, int x, int y) =>
            {
                return cell.distanceToClosestPoint + "-";
            });
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day6\Input.txt");
            var maximumDistanceSum = 10000;

            var nbPoints = lines.Length;
            var points = new Point[nbPoints];
            for (var i = 0; i < nbPoints; i++)
            {
                points[i].Parse(lines[i], i);
            }

            GetBoxSize(points, out var width, out var height, out var left, out var top);

            var nbValidCells = 0;
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var sum = points.Sum(x => MathUtils.ManhattanDistance(x.pos, new float2(i + left, j + top)));
                    if (sum < maximumDistanceSum)
                        nbValidCells++;
                }
            }

            return nbValidCells;
        }

        private static void GetBoxSize(Point[] points, out int width, out int height, out int left, out int top)
        {
            left = (int) points.Select(x => x.pos.x).Min();
            var right = (int) points.Select(x => x.pos.x).Max();
            top = (int) points.Select(x => x.pos.y).Min();
            var bottom = (int) points.Select(x => x.pos.y).Max();
            width = right - left + 1;
            height = bottom - top + 1;
        }

        private struct Point {
            public float2 pos;
            public int id;
            public bool infiniteArea;

            public void Parse(string line, int id)
            {
                var coordinates = line.Split(',');
                pos = new float2(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
                this.id = id;
                infiniteArea = false;
            }
        }

        private struct Cell
        {
            // -1 if not computed yet
            public int distanceToClosestPoint;

            // -1 if equally far from multiple points
            public int closestPointId;
        }
    }
}
