using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCodeTools;

namespace AdventOfCode
{
    class Day8
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var line = IO.GetStringLines(@"Day8\Input.txt")[0];
            var numbers = line
                .Split(' ')
                .Select(x => int.Parse(x))
                .ToArray();

            var root = Parse(numbers);

            return root.Iterate<int>((curr, children) => children.Sum() + curr.metadata.Sum());
        }

        public static int Part2()
        {
            var line = IO.GetStringLines(@"Day8\Input.txt")[0];
            var numbers = line
                .Split(' ')
                .Select(x => int.Parse(x))
                .ToArray();

            var root = Parse(numbers);

            return root.Iterate<int>((curr, children) => curr.ComputeValue(children));
        }

        private static Node Parse(int[] numbers)
        {
            var stack = new Stack<Node>();
            var cursor = 0;
            while (cursor < numbers.Length)
            {
                // Handle the last elements in stack
                while (stack.Count > 0)
                {
                    var last = stack.Peek();
                    if (last.children.Count < last.nbChildren)
                        break;

                    for (var i = 0; i < last.nbMetadata; i++)
                    {
                        last.metadata.Add(numbers[cursor++]);
                    }
                    stack.Pop();

                    if (stack.Count > 0)
                    {
                        var parent = stack.Peek();
                        parent.children.Add(last);
                    }
                    else
                    {
                        return last;
                    }
                }

                // Create the next element
                var nbChildren = numbers[cursor++];
                var nbMetadata = numbers[cursor++];
                var currentNode = new Node() {
                    nbChildren = nbChildren,
                    nbMetadata = nbMetadata,
                    children = new List<Node>(nbChildren),
                    metadata = new List<int>(nbMetadata),
                };
                stack.Push(currentNode);
            }

            return null;
        }

        private class Node
        {
            public int nbChildren;
            public int nbMetadata;
            public List<Node> children;
            public List<int> metadata;

            public T Iterate<T>(Func<Node, T[], T> combine)
            {
                var childrenResult = new T[nbChildren];
                for (var i = 0; i < nbChildren; i++)
                {
                    childrenResult[i] = children[i].Iterate(combine);
                }

                return combine(this, childrenResult);
            }

            public int ComputeValue(int[] childrenValues)
            {
                if (nbChildren == 0)
                    return metadata.Sum();

                var result = 0;
                foreach (var meta in metadata)
                {
                    if (meta > 0 && meta <= nbChildren)
                        result += childrenValues[meta - 1];
                }
                return result;
            }
        }
    }
}
