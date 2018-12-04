using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Day2.Part1());
            Console.WriteLine(Day2.Part2());
            Console.ReadLine();
        }

        public static string GetPath(string pathFromSolutionRoot)
        {
            return "..\\..\\" + pathFromSolutionRoot;
        }
    }
}
