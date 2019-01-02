using System;
using System.IO;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Day18.Run();
            Console.ReadLine();
        }

        public static string[] GetLines(string pathFromSolutionRoot)
        {
            return System.IO.File.ReadAllLines(GetPath(pathFromSolutionRoot));
        }

        public static string GetPath(string pathFromSolutionRoot)
        {
            return "..\\..\\" + pathFromSolutionRoot;
        }

        public static void SetOutputFile(string pathFromSolutionRoot)
        {
            FileStream filestream = new FileStream(GetPath(pathFromSolutionRoot), FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
        }

        public static T[] Flatten<T>(T[,] multiple)
        {
            var xSize = multiple.GetLength(0);
            var ySize = multiple.GetLength(1);
            var flat = new T[xSize * ySize];

            for (var i = 0; i < xSize; i++)
            {
                for (var j = 0; j < ySize; j++)
                {
                    flat[i * ySize + j] = multiple[i, j];
                }
            }
            return flat;
        }
    }
}
