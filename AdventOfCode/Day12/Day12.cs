using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day12
    {
        private static readonly int ruleLength = 5;
        private static readonly int ruleRadius = ruleLength / 2;

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static long Part1()
        {
            var lines = Utils.GetLines(".\\Day12\\Input.txt");
            var nbGenerations = 20;

            return ComputePlantSum(lines, nbGenerations);
        }

        public static long Part2()
        {
            var lines = Utils.GetLines(".\\Day12\\Input.txt");
            var nbGenerations = 50000000000;

            return ComputePlantSum(lines, nbGenerations);
        }

        private static long ComputePlantSum(string[] lines, long nbGenerations)
        {
            var currentGeneration = new Generation(lines[0]);
            var rules = RuleNode.Parse(lines.Skip(2));

            currentGeneration.ComputeGenerations(nbGenerations, rules);
            return currentGeneration.ComputePlantSum();
        }

        private struct Generation
        {
            private bool[] plants;
            private long indexFirstPlant;
            private long generationNumber;

            // Some additional information to avoid unnecessary computing
            private bool plantsArrayStable; // true when we reach a point where the array stays exactly the same in each generation
            private int indexFirstPlantDelta; // the first index delta between two generations

            private static readonly Regex regex = new Regex(@"initial state: (.*)");

            public Generation(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    plants = match
                        .Groups[1]
                        .Value
                        .Select(x => x == '#')
                        .ToArray();
                }
                else
                {
                    plants = new bool[0];
                }
                indexFirstPlant = 0;
                plantsArrayStable = false;
                indexFirstPlantDelta = 0;
                generationNumber = 1;
            }

            public void Print()
            {
                for (var i = indexFirstPlant; i < 0; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("0");

                for (var i = 0; i < indexFirstPlant; i++)
                {
                    Console.Write(".");
                }
                foreach (var index in plants)
                {
                    Console.Write((index ? '#' : '.'));
                }
                Console.WriteLine();
            }

            public void ComputeGenerations(long nbGenerations, RuleNode rules)
            {
                for (long i = 0; i < nbGenerations; i++)
                {
                    ComputeNextGeneration(rules);
                    if (plantsArrayStable)
                        break;
                }

                var delta = nbGenerations - generationNumber + 1;
                indexFirstPlant += delta * indexFirstPlantDelta;
            }

            public void ComputeNextGeneration(RuleNode rules)
            {
                // We need to extend to handle the extremities
                var extendedNewPlants = new bool[plants.Length + 2 * ruleRadius];
                var plantLength = plants.Length;
                var extendedNewPlantsLength = extendedNewPlants.Length;
                var extendedNewPlantsMax = plantLength + ruleRadius;
                for (var i = -ruleRadius; i < extendedNewPlantsMax; i++)
                {
                    var pattern = new bool[ruleLength];
                    for (var j = -ruleRadius; j <= ruleRadius; j++)
                    {
                        var sumIndexes = i + j;
                        if (sumIndexes >= 0 && sumIndexes < plantLength)
                            pattern[j + ruleRadius] = plants[sumIndexes];
                    }

                    extendedNewPlants[i + ruleRadius] = rules.ComputeResult(pattern);
                }

                // Cut the empty extremities
                var leftOverage = 0;
                var rightOverage = 0;
                while (leftOverage < extendedNewPlantsLength && !extendedNewPlants[leftOverage])
                    leftOverage++;
                while (rightOverage < extendedNewPlantsLength && !extendedNewPlants[extendedNewPlantsLength - rightOverage - 1])
                    rightOverage++;
                var cleanNewPlants = new bool[extendedNewPlantsLength - leftOverage - rightOverage];
                for (var i = 0; i < cleanNewPlants.Length; i++)
                {
                    cleanNewPlants[i] = extendedNewPlants[i + leftOverage];
                }
                indexFirstPlantDelta = leftOverage - ruleRadius;

                // Check if constant
                if (plants.Length == cleanNewPlants.Length)
                {
                    var checkConstant = true;
                    for (var i = 0; i < plants.Length; i++)
                    {
                        if (plants[i] != cleanNewPlants[i])
                        {
                            checkConstant = false;
                            break;
                        }
                    }

                    plantsArrayStable = checkConstant;
                }

                plants = cleanNewPlants;
                indexFirstPlant = indexFirstPlant + indexFirstPlantDelta;
                generationNumber++;
            }

            public long ComputePlantSum()
            {
                var result = (long) 0;
                for (var i = 0; i < plants.Length; i++)
                {
                    if (plants[i])
                        result += i + indexFirstPlant;
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
                        root.AddNodeRecursive(match.Groups[1].Value.ToArray(), 0);
                }

                return root;
            }

            public void AddNodeRecursive(char[] rule, int cursor)
            {
                if (cursor == rule.Length)
                    return;
                var character = rule[cursor];
                if (character == '#')
                {
                    yes = yes ?? new RuleNode();
                    yes.AddNodeRecursive(rule, cursor + 1);
                }
                else
                {
                    no = no ?? new RuleNode();
                    no.AddNodeRecursive(rule, cursor + 1);
                }
            }

            public bool ComputeResult(bool[] pattern) {
                var currentNode = this;
                var patternLength = pattern.Length;
                for (var i = 0; i < patternLength; i++)
                {
                    if (currentNode == null)
                        return false;

                    currentNode = pattern[i] ? currentNode.yes : currentNode.no;
                }

                return currentNode != null;
            }
        }
    }
}