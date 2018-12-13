using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day12
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static long Part1()
        {
            var lines = Program.GetLines(".\\Day12\\Input.txt");
            var nbGenerations = 20;

            return ComputePlantSum(lines, nbGenerations);
        }

        public static long Part2()
        {
            var lines = Program.GetLines(".\\Day12\\Input.txt");
            var nbGenerations = 50000000000;

            return ComputePlantSum(lines, nbGenerations);
        }

        private static long ComputePlantSum(string[] lines, long nbGenerations)
        {
            var currentGeneration = new Generation(lines[0]);
            var rules = RuleNode.Parse(lines.Skip(2));

            for (long i = 0; i < nbGenerations; i++)
            {
                currentGeneration = currentGeneration.ComputeNext(rules);
            }

            return currentGeneration.ComputePlantSum();
        }

        private struct Generation
        {
            public bool[] plants;
            public int firstPlantIndex;

            private static readonly Regex regex = new Regex(@"initial state: (.*)");
            private static readonly int ruleLength = 5;
            private static readonly int ruleRadius = ruleLength / 2;

            public Generation(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var chars = match.Groups[1].Value.ToCharArray();
                    plants = chars.Select(x => x == '#').ToArray();
                    firstPlantIndex = 0;
                }
                else
                {
                    plants = new bool[0];
                    firstPlantIndex = 0;
                }
            }

            public void Print()
            {
                for (var i = firstPlantIndex; i < 0; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("0");


                for (var i = 0; i < firstPlantIndex; i++)
                {
                    Console.Write(".");
                }
                foreach (var index in plants)
                {
                    Console.Write((index ? '#' : '.'));
                }
                Console.WriteLine();
            }

            public Generation ComputeNext(RuleNode rules)
            {
                // We need to extend to handle the extremities
                var extendedNewPlants = new bool[plants.Length + 2 * ruleRadius];
                for (var i = -ruleRadius; i < plants.Length + ruleRadius; i++)
                {
                    var pattern = new bool[ruleLength];
                    for (var j = -ruleRadius; j <= ruleRadius; j++)
                    {
                        if (i + j >= 0 && i + j < plants.Length)
                            pattern[j + ruleRadius] = plants[i + j];
                    }

                    extendedNewPlants[i + ruleRadius] = RuleNode.RuleResult(rules, pattern);
                }

                // Cut the empty extremities
                var leftOverage = 0;
                var rightOverage = 0;
                while (leftOverage < extendedNewPlants.Length && !extendedNewPlants[leftOverage])
                    leftOverage++;
                while (rightOverage < extendedNewPlants.Length && !extendedNewPlants[extendedNewPlants.Length - rightOverage - 1])
                    rightOverage++;
                var cleanNewPlants = new bool[extendedNewPlants.Length - leftOverage - rightOverage];
                for (var i = 0; i < cleanNewPlants.Length; i++)
                {
                    cleanNewPlants[i] = extendedNewPlants[i + leftOverage];
                }

                return new Generation() {
                    plants = cleanNewPlants,
                    firstPlantIndex = firstPlantIndex - ruleRadius + leftOverage,
                };
            }

            public long ComputePlantSum()
            {
                var result = (long) 0;
                for (var i = 0; i < plants.Length; i++)
                {
                    if (plants[i])
                        result += i + firstPlantIndex;
                }

                return result;
            }
        }

        private class RuleNode
        {
            public RuleNode yes = null;
            public RuleNode no = null;

            private static readonly Regex regex = new Regex(@"(.*) => (.*)");

            public static RuleNode Parse(IEnumerable<string> rules)
            {
                var root = new RuleNode();

                foreach (var rule in rules)
                {
                    var match = regex.Match(rule);
                    if (match.Success && match.Groups[2].Value.Equals("#"))
                        root.AddStep(match.Groups[1].Value.ToArray(), 0);
                }

                return root;
            }

            public void AddStep(char[] rule, int cursor)
            {
                if (cursor == rule.Length)
                    return;
                var character = rule[cursor];
                if (character == '#')
                {
                    yes = yes == null ? new RuleNode() : yes;
                    yes.AddStep(rule, cursor + 1);
                }
                else
                {
                    no = no == null ? new RuleNode() : no;
                    no.AddStep(rule, cursor + 1);
                }
            }

            public static bool RuleResult(RuleNode root, bool[] pattern) {
                var currentNode = root;
                for (var i = 0; i < pattern.Length; i++)
                {
                    if (currentNode == null)
                        return false;

                    currentNode = pattern[i] ? currentNode.yes : currentNode.no;
                }

                return !(currentNode == null);
            }
        }
    }
}