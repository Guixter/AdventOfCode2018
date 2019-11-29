using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCodeTools
{
    public class IO
    {
        public static void Print(string text, ConsoleColor color = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = background;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Print(Printer printer)
        {
            Print(printer.text, printer.color, printer.background);
        }

        public static void PrintMultiple(string msg, int nb, ConsoleColor color = ConsoleColor.White)
        {
            for (int i = 0; i < nb; i++)
            {
                Print(msg, color);
            }
        }

        public static string[] GetStringLines(string pathFromDaysFolder)
        {
            return File.ReadAllLines($@"..\..\Days\{pathFromDaysFolder}");
        }

        public static int[] GetIntLines(string pathFromDaysFolder)
        {
            return GetStringLines(pathFromDaysFolder)
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

    public class Printer
    {
        public string text = "";
        public ConsoleColor color = ConsoleColor.White;
        public ConsoleColor background = ConsoleColor.Black;

        public Printer(string text = "", ConsoleColor color = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            this.text = text;
            this.color = color;
            this.background = background;
        }

        public static implicit operator Printer(string value)
        {
            return new Printer()
            {
                text = value
            };
        }

        public static implicit operator Printer(char value)
        {
            return new Printer()
            {
                text = value.ToString()
            };
        }
    }
}
