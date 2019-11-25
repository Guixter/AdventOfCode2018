using AdventOfCodeTools;
using System;

namespace AdventOfCode
{
    class Day11
    {
        private static readonly int gridSize = 300;

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static string Part1()
        {
            var gridSerialNumber = 9306;
            var squareSize = 3;
            var powerGrid = BuildPowerGrid(gridSerialNumber);
            var maxPoint = ComputeMaxPowerSquare(squareSize, gridSerialNumber, powerGrid);
            return maxPoint.Item1 + "," + maxPoint.Item2 + "  -> " + maxPoint.Item3;
        }

        public static string Part2()
        {
            var gridSerialNumber = 9306;
            var powerGrid = BuildPowerGrid(gridSerialNumber);

            var totalGrid = new Grid<int>(gridSize, gridSize);
            var maxSquareSize = (0, 0, 0, float.NegativeInfinity);
            for (var squareSize = 1; squareSize < gridSize; squareSize++)
            {
                var maxPoint = ComputeMaxPowerSquare(squareSize, gridSerialNumber, powerGrid, totalGrid);

                if (maxPoint.Item3 > maxSquareSize.Item4)
                    maxSquareSize = (maxPoint.Item1, maxPoint.Item2, squareSize, maxPoint.Item3);
            }

            return maxSquareSize.Item1 + "," + maxSquareSize.Item2 + "," + maxSquareSize.Item3 + "  -> " + maxSquareSize.Item4;
        }

        // Each cell of the total grid contains the total power of the square whose top-left cell is this one
        private static (int, int, float) ComputeMaxPowerSquare(int squareSize, int gridSerialNumber, Grid<int> powerGrid, Grid<int> totalGrid = null)
        {
            var fromScratch = totalGrid == null;
            if (fromScratch)
                totalGrid = new Grid<int>(gridSize - squareSize + 1, gridSize - squareSize + 1);

            var maxPoint = (0, 0, float.NegativeInfinity);
            var borderX = totalGrid.xLength - squareSize + 1;
            var borderY = totalGrid.yLength - squareSize + 1;
            for (var i = 0; i < borderX; i++)
            {
                for (var j = 0; j < borderY; j++)
                {
                    var power = ComputeTotalPower(i, j, squareSize, powerGrid, totalGrid, fromScratch);
                    totalGrid[i, j] = power;

                    if (power > maxPoint.Item3)
                        maxPoint = (i + 1, j + 1, totalGrid[i, j]);
                }
            }

            return maxPoint;
        }

        private static int ComputeTotalPower(int x, int y, int squareSize, Grid<int> powerGrid, Grid<int> lastTotalGrid, bool fromScratch)
        {
            var power = fromScratch ? 0 : lastTotalGrid[x, y];

            if (fromScratch)
            {
                for (var dx = 0; dx < squareSize; dx++)
                {
                    for (var dy = 0; dy < squareSize; dy++)
                    {
                        power += powerGrid[x + dx, y + dy];
                    }
                }
            }
            else
            {
                for (var d = 0; d < squareSize - 1; d++)
                {
                    power += powerGrid[x + d, y + squareSize - 1];
                    power += powerGrid[x + squareSize - 1, y + d];
                }
                power += powerGrid[x + squareSize - 1, y + squareSize - 1];
            }
            return power;
        }

        private static Grid<int> BuildPowerGrid(int gridSerialNumber)
        {
            var powerGrid = new Grid<int>(gridSize, gridSize);
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    powerGrid[i, j] = ComputePower(i + 1, j + 1, gridSerialNumber);
                }
            }
            return powerGrid;
        }

        private static int ComputePower(int x, int y, int gridSerialNumber)
        {
            var rackId = x + 10;
            var power = y * rackId;
            power += gridSerialNumber;
            power *= rackId;
            power = (power / 100) % 10;
            return power - 5;
        }

        private static void AddPower(int x, int y, int power, int squareSize, Grid<int> grid)
        {
            for (var i = 0; i < squareSize; i++)
            {
                for (var j = 0; j < squareSize; j++)
                {
                    grid[x + i, y + j] += power;
                }
            }
        }

        private static void PrintGrid(int value, int x, int y)
        {
            var cell = value.ToString();
            IO.Print(cell);
            for (var k = cell.Length; k < 4; k++)
            {
                IO.Print(" ");
            }
        }
    }
}