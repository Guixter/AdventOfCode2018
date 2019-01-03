using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day9
    {
        private static readonly Regex regex = new Regex(@"(.*) players; last marble is worth (.*) points");
        private static readonly int tourLength = 23;


        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var line = Utils.GetLines(".\\Day9\\Input.txt")[0];

            Parse(line, out var nbPlayers, out var lastMarble);

            var current = new Marble(0);
            current.clockwise = current;
            current.counterClockwise = current;

            var playerScores = new int[nbPlayers];
            var currentPlayer = 0;
            for (var i = 1; i <= lastMarble; i++)
            {
                if (i % tourLength == 0)
                {
                    var marble = current.RemoveAt(-7);
                    playerScores[currentPlayer] += i + marble.value;
                    current = marble.clockwise;
                }
                else
                {
                    var marble = current.AddAt(new Marble(i), 1);
                    current = marble;
                }
                currentPlayer = (currentPlayer + 1) % nbPlayers;
            }

            return playerScores.Max();
        }

        public static long Part2()
        {
            var line = Utils.GetLines(".\\Day9\\Input.txt")[0];

            Parse(line, out var nbPlayers, out var lastMarble);

            lastMarble *= 100;

            var current = new Marble(0);
            current.clockwise = current;
            current.counterClockwise = current;

            var playerScores = new long[nbPlayers];
            var currentPlayer = 0;
            for (var i = 1; i <= lastMarble; i++)
            {
                if (i % tourLength == 0)
                {
                    var marble = current.RemoveAt(-7);
                    playerScores[currentPlayer] += i + marble.value;
                    current = marble.clockwise;
                }
                else
                {
                    var marble = current.AddAt(new Marble(i), 1);
                    current = marble;
                }
                currentPlayer = (currentPlayer + 1) % nbPlayers;
            }

            return playerScores.OrderByDescending(x => x).First();
        }

        private static void Parse(string line, out int nbPlayers, out long lastMarble)
        {
            var match = regex.Match(line);
            if (match.Success)
            {
                nbPlayers = int.Parse(match.Groups[1].Value);
                lastMarble = int.Parse(match.Groups[2].Value);
            }
            else
            {
                nbPlayers = 0;
                lastMarble = 0;
            }
        }

        private class Marble
        {
            public Marble clockwise;
            public Marble counterClockwise;
            public int value;

            public Marble(int value)
            {
                this.value = value;
            }

            public Marble GetClockwise(int at)
            {
                return at == 0 ? this : clockwise.GetClockwise(at - 1);
            }

            public Marble GetCounterClockwise(int at)
            {
                return at == 0 ? this : counterClockwise.GetCounterClockwise(at - 1);
            }

            public Marble AddAt(Marble marble, int clockwise)
            {
                var left = clockwise > 0 ? GetClockwise(clockwise) : GetCounterClockwise(-clockwise - 1);
                var right = left.clockwise;

                marble.clockwise = right;
                marble.counterClockwise = left;
                left.clockwise = marble;
                right.counterClockwise = marble;

                return marble;
            }

            public Marble RemoveAt(int clockwise)
            {
                var marble = clockwise > 0 ? GetClockwise(clockwise) : GetCounterClockwise(-clockwise);
                var left = marble.counterClockwise;
                var right = marble.clockwise;

                left.clockwise = right;
                right.counterClockwise = left;

                return marble;
            }
        }
    }
}
