using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCodeTools
{
    // TODO :
    // - interface for each day
    // - utils methods (rectangle, matrix, ...)

    public class IO
    {

        public static string[] GetStringLines(string pathFromSolution)
        {
            return File.ReadAllLines($@"..\..\{pathFromSolution}");
        }

        public static int[] GetIntLines(string pathFromSolution)
        {
            return GetStringLines(pathFromSolution)
                .Select(x => int.Parse(x))
                .ToArray();
        }

        public static void SetOutputFile(string pathFromSolutionRoot)
        {
            FileStream filestream = new FileStream(pathFromSolutionRoot, FileMode.Create);
            var streamwriter = new StreamWriter(filestream)
            {
                AutoFlush = true
            };
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
        }

        public static string[] Match(Regex regex, string input)
        {
            var match = regex.Match(input);
            if (match.Success)
            {
                var nbGroups = match.Groups.Count;
                var result = new string[nbGroups - 1];

                for (var i = 1 ; i < nbGroups; i++)
                {
                    result[i - 1] = match.Groups[i].Value;
                }

                return result;
            }
            else
            {
                throw new Exception($"Failed to match the input : {input}\n with the regex : {regex.ToString()}");
            }
        }

        public static string[] Match(string regex, string input)
        {
            return Match(new Regex(regex), input);
        }
    }
}
