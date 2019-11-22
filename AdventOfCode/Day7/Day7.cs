using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCodeTools;

namespace AdventOfCode
{
    class Day7
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static string Part1()
        {
            var lines = IO.GetStringLines(@"Day7\Input.txt");

            var nodes = Parse(lines);
            var freeNodes = GetRootNodes(nodes);

            // Iterate though the graph
            var cursor = 0;
            var result = new char[nodes.Count];
            var doneNodes = new HashSet<Node>();
            while(freeNodes.Count > 0)
            {
                var current = freeNodes.First();
                freeNodes.Remove(current);

                doneNodes.Add(current);
                result[cursor] = current.name;

                UnlockDependencies(current, doneNodes, freeNodes);
                cursor++;
            }

            return new string(result);
        }

        public static int Part2()
        {
            var lines = IO.GetStringLines(@"Day7\Input.txt");
            var nbWorkers = 5;

            var nodes = Parse(lines);
            var freeNodes = GetRootNodes(nodes);

            // Iterate though the graph
            var currentSecond = 0;
            var doneNodes = new HashSet<Node>();
            var currentTasks = new Node[nbWorkers];
            while (doneNodes.Count < nodes.Count)
            {
                // Update the current tasks
                for (int workerId = 0; workerId < nbWorkers; workerId++)
                {
                    if (currentTasks[workerId] == null && freeNodes.Count > 0)
                    {
                        currentTasks[workerId] = freeNodes.First();
                        freeNodes.Remove(currentTasks[workerId]);
                    }
                }

                // Apply time changes
                for (int workerId = 0; workerId < nbWorkers; workerId++)
                {
                    var currentTask = currentTasks[workerId];
                    if (currentTask != null)
                    {
                        currentTask.workTime++;

                        if (currentTask.workTime == currentTask.TotalTime)
                        {
                            currentTasks[workerId] = null;
                            doneNodes.Add(currentTask);

                            UnlockDependencies(currentTask, doneNodes, freeNodes);
                        }
                    }
                }
                currentSecond++;
            }

            return currentSecond;
        }

        private static HashSet<Node> Parse(string[] lines)
        {
            HashSet<Node> nodes = new HashSet<Node>();
            foreach (var line in lines)
            {
                var parsedNodes = Node.Parse(line);
                var firstNode = nodes
                    .Where(x => x.name.Equals(parsedNodes[0]))
                    .FirstOrDefault();
                if (firstNode == null)
                {
                    firstNode = new Node() {
                        name = parsedNodes[0]
                    };
                    nodes.Add(firstNode);
                }
                var secondNode = nodes
                    .Where(x => x.name.Equals(parsedNodes[1]))
                    .FirstOrDefault();
                if (secondNode == null)
                {
                    secondNode = new Node() {
                        name = parsedNodes[1]
                    };
                    nodes.Add(secondNode);
                }

                firstNode.next.Add(secondNode);
                secondNode.dependencies.Add(firstNode);
            }
            return nodes;
        }

        private static SortedSet<Node> GetRootNodes(HashSet<Node> nodes)
        {
            var freeNodes = new SortedSet<Node>();
            var rootNodes = nodes
                .Where(x => x.dependencies.Count == 0);

            foreach (var node in rootNodes) {
                freeNodes.Add(node);
            }
            return freeNodes;
        }

        private static void UnlockDependencies(Node current, HashSet<Node> doneNodes, SortedSet<Node> freeNodes)
        {
            foreach (var next in current.next)
            {
                var lockedDependencies = next.dependencies
                    .Where(x => !doneNodes.Contains(x));
                if (lockedDependencies.Count() == 0)
                    freeNodes.Add(next);
            }
        }

        private class Node : IComparable<Node>
        {
            public List<Node> next = new List<Node>();
            public List<Node> dependencies = new List<Node>();
            public char name;
            public int workTime = 0;

            public int TotalTime { get {
                    return defaultTime + 1 + (name - 'A');
            }}

            private static readonly Regex regex = new Regex(@"Step (.*) must be finished before step (.*) can begin.");
            private static readonly int defaultTime = 60;

            public static char[] Parse(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    return new char[] {
                        match.Groups[1].Value[0],
                        match.Groups[2].Value[0],
                    };
                }
                else
                {
                    return new char[0];
                }
            }

            public int CompareTo(Node obj)
            {
                return name.CompareTo(obj.name);
            }
        }
    }
}
