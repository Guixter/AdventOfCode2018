using System;
using System.IO;

namespace AdventOfCode
{
    public static class Utils
    {
        public static string[] GetLines(string pathFromSolutionRoot)
        {
            return File.ReadAllLines(GetPath(pathFromSolutionRoot));
        }

        public static string GetPath(string pathFromSolutionRoot)
        {
            return "..\\..\\" + pathFromSolutionRoot;
        }

        public static void SetOutputFile(string pathFromSolutionRoot)
        {
            FileStream filestream = new FileStream(GetPath(pathFromSolutionRoot), FileMode.Create);
            var streamwriter = new StreamWriter(filestream)
            {
                AutoFlush = true
            };
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
        }

        public static T[] Flatten<T>(T[,] grid)
        {
            var xSize = grid.GetLength(0);
            var ySize = grid.GetLength(1);
            var flat = new T[xSize * ySize];

            for (var i = 0; i < xSize; i++)
            {
                for (var j = 0; j < ySize; j++)
                {
                    flat[i * ySize + j] = grid[i, j];
                }
            }
            return flat;
        }

        public static void PrintGrid<T>(T[,] grid)
        {

        }
    }
}
