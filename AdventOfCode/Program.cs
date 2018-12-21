using System;
using System.IO;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Day17.Run();
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
    }
}
