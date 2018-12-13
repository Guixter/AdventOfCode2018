using System;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Day12.Run();
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
    }
}
