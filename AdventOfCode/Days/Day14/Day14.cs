using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day14
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static string Part1()
        {
            var supposedSkillImprovementRecipe = 880751;
            var nbRelevantRecipes = 10;

            var recipeScores = new List<int>() { 3, 7 };
            var firstElfRecipe = 0;
            var secondElfRecipe = 1;

            while (recipeScores.Count < supposedSkillImprovementRecipe + nbRelevantRecipes)
            {
                PerformStep(recipeScores, ref firstElfRecipe, ref secondElfRecipe);
            }

            var builder = new StringBuilder();
            for (var i = 0; i < nbRelevantRecipes; i++)
            {
                builder.Append(recipeScores[supposedSkillImprovementRecipe + i]);
            }

            return builder.ToString();
        }

        public static int Part2()
        {
            var wishedScore = "880751";
            var wishedScoreDigits = wishedScore
                .ToCharArray()
                .Select(x => (int) char.GetNumericValue(x))
                .ToArray();
            var scoreSize = wishedScoreDigits.Length;

            var recipeScores = new List<int>() { 3, 7 };
            var firstElfRecipe = 0;
            var secondElfRecipe = 1;

            int wishedScorePosition = -1;
            while (true)
            {
                var nbNewRecipes = PerformStep(recipeScores, ref firstElfRecipe, ref secondElfRecipe);

                for (var i = 0; i < nbNewRecipes; i++)
                {
                    if (recipeScores.Count - i >= scoreSize) {
                        var wishedScoreFound = true;
                        for (var j = 0; j < scoreSize; j++)
                        {
                            if (recipeScores[recipeScores.Count - scoreSize - i + j] != wishedScoreDigits[j])
                            {
                                wishedScoreFound = false;
                                break;
                            }
                        }

                        if (wishedScoreFound)
                            wishedScorePosition = recipeScores.Count - scoreSize - i;
                    }
                }

                if (wishedScorePosition != -1)
                    break;
            }

            return wishedScorePosition;
        }

        private static int PerformStep(List<int> recipeScores, ref int firstElfRecipe, ref int secondElfRecipe)
        {
            // Add the new recipes
            var sum = recipeScores[firstElfRecipe] + recipeScores[secondElfRecipe];
            var digits = GetDigits(sum);
            recipeScores.AddRange(digits);

            // Change the elves current recipes
            firstElfRecipe = (firstElfRecipe + 1 + recipeScores[firstElfRecipe]) % recipeScores.Count;
            secondElfRecipe = (secondElfRecipe + 1 + recipeScores[secondElfRecipe]) % recipeScores.Count;

            return digits.Length;
        }

        private static int[] GetDigits(int value)
        {
            if (value == 0)
                return new int[] { 0 };

            var digits = new int[(int)Math.Floor(Math.Log10(value)) + 1];
            for (var i = 0; i < digits.Length; i++)
            {
                var pow10 = (int) Math.Pow(10, digits.Length - i - 1);
                digits[i] = value / pow10;
                value -= digits[i] * pow10;
            }
            return digits;
        }

        private static void Print(List<int> scores, int parenthesisIndex, int bracketIndex)
        {
            for (var i = 0; i < scores.Count; i++)
            {
                if (i == parenthesisIndex)
                {
                    Console.Write("(" + scores[i] + ")");
                }
                else if (i == bracketIndex)
                {
                    Console.Write("[" + scores[i] + "]");
                }
                else
                {
                    Console.Write(" " + scores[i] + " ");
                }
            }
            Console.WriteLine();
        }
    }
}