﻿using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day16
    {
        private static readonly Instruction[] instructions = new Instruction[]
        {
            new Instruction.Addr(),
            new Instruction.Addi(),
            new Instruction.Mulr(),
            new Instruction.Muli(),
            new Instruction.Banr(),
            new Instruction.Bani(),
            new Instruction.Borr(),
            new Instruction.Bori(),
            new Instruction.Setr(),
            new Instruction.Seti(),
            new Instruction.Gtir(),
            new Instruction.Gtri(),
            new Instruction.Gtrr(),
            new Instruction.Eqir(),
            new Instruction.Eqri(),
            new Instruction.Eqrr(),
        };

        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = IO.GetStringLines(@"Day16\Input.txt");

            var snapshots = ParseCpuSnapshots(lines, out var lastLine);

            return snapshots
                .Where(x => x.PossibleInstructions().Count() >= 3)
                .Count();
        }

        public static long Part2()
        {
            var lines = IO.GetStringLines(@"Day16\Input.txt");

            var snapshots = ParseCpuSnapshots(lines, out var lastLine);
            var opcodeMapping = ComputeOpcodeMapping(snapshots);

            // Execute the instructions
            var program = ParseProgram(lines, lastLine + 4);
            var registers = new long[4];
            foreach (var command in program)
            {
                opcodeMapping[command.opcode].Process(command, registers);
            }

            return registers[0];
        }

        private static Instruction[] ComputeOpcodeMapping(List<CpuSnapshot> snapshots)
        {
            var possibleMappings = new Dictionary<int, List<int>>();
            foreach (var snap in snapshots)
            {
                var possibleInstructions = snap
                    .PossibleInstructions()
                    .Distinct();

                foreach (var i in possibleInstructions)
                {
                    if (!possibleMappings.ContainsKey(i))
                        possibleMappings[i] = new List<int>();

                    if (!possibleMappings[i].Contains(snap.command.opcode))
                        possibleMappings[i].Add(snap.command.opcode);
                }
            }

            var mapping = Enumerable
                .Repeat(-1, instructions.Length)
                .ToArray();

            while (mapping.Contains(-1))
            {
                // Find instructions that have only one possible opcode
                foreach (var couple in possibleMappings)
                {
                    if (couple.Value.Count == 1)
                    {
                        var newInstruction = couple.Value[0];
                        mapping[newInstruction] = couple.Key;
                    }
                }

                // Remove from possible mappings the opcodes that have already been assignated
                foreach (var couple in possibleMappings)
                {
                    couple.Value.RemoveAll(x => mapping[x] != -1);
                }
            }

            return mapping
                .Select(x => instructions[x])
                .ToArray();
        }

        private static List<CpuSnapshot> ParseCpuSnapshots(string[] lines, out int lastLine)
        {
            var list = new List<CpuSnapshot>();

            var i = 0;
            while (i * 4 < lines.Length && !string.IsNullOrEmpty(lines[i * 4]))
            {
                list.Add(CpuSnapshot.Parse(lines[i * 4], lines[i * 4 + 1], lines[i * 4 + 2]));
                i++;
            }

            lastLine = i * 4 - 2;
            return list;
        }

        private static List<Command> ParseProgram(string[] lines, int start)
        {
            var list = new List<Command>();

            while (start < lines.Length)
            {
                list.Add(Command.Parse(lines[start++]));
            }

            return list;
        }

        public struct Command
        {
            public int opcode;
            public int a;
            public int b;
            public int c;

            private static readonly Regex regex = new Regex(@"(.*) (.*) (.*) (.*)");

            public static Command Parse(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    return new Command() {
                        opcode = int.Parse(match.Groups[1].Value),
                        a = int.Parse(match.Groups[2].Value),
                        b = int.Parse(match.Groups[3].Value),
                        c = int.Parse(match.Groups[4].Value),
                    };
                }
                else
                {
                    return new Command();
                }
            }
        }

        private class CpuSnapshot
        {
            public Command command;
            public long[] before;
            public long[] after;

            private static readonly Regex regex = new Regex(@".*\[(.*), (.*), (.*), (.*)\]");

            public IEnumerable<int> PossibleInstructions()
            {
                return instructions
                    .Select((x, i) => new Tuple<Instruction, int>(x, i))
                    .Where(i =>
                        i.Item1.ProcessClone(command, before)
                            .Zip(after, (x, y) => x == y)
                            .Aggregate(true, (curr, val) => val && curr)
                    )
                    .Select(x => x.Item2);
            }

            public static CpuSnapshot Parse(string before, string command, string after)
            {
                return new CpuSnapshot() {
                    command = Command.Parse(command),
                    before = ParseRegisters(before),
                    after = ParseRegisters(after),
                };
            }

            private static long[] ParseRegisters(string line)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    return new long[] {
                        long.Parse(match.Groups[1].Value),
                        long.Parse(match.Groups[2].Value),
                        long.Parse(match.Groups[3].Value),
                        long.Parse(match.Groups[4].Value),
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public abstract class Instruction
        {
            public long[] ProcessClone(Command command, long[] registers)
            {
                var result = (long[]) registers.Clone();
                Process(command, result);
                return result;
            }

            public abstract void Process(Command command, long[] registers);
            public abstract string ToString(Command command);

            public class Addr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] + registers[command.b];
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " + " + (char) ('A' + command.b);
                }
            }

            public class Addi : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] + command.b;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " + " + command.b;
                }
            }

            public class Mulr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] * registers[command.b];
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " * " + (char) ('A' + command.b);
                }
            }

            public class Muli : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] * command.b;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " * " + command.b;
                }
            }

            public class Banr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] & registers[command.b];
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " & " + (char) ('A' + command.b);
                }
            }

            public class Bani : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] & command.b;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " & " + command.b;
                }
            }

            public class Borr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] | registers[command.b];
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " | " + (char) ('A' + command.b);
                }
            }

            public class Bori : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = (int) registers[command.a] | command.b;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " | " + command.b;
                }
            }

            public class Setr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a];
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a);
                }
            }

            public class Seti : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = command.a;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + command.a;
                }
            }

            public class Gtir : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = command.a > registers[command.b] ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + command.a + " > " + (char) ('A' + command.b);
                }
            }

            public class Gtri : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] > command.b ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " > " + command.b;
                }
            }

            public class Gtrr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] > registers[command.b] ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " > " + (char) ('A' + command.b);
                }
            }

            public class Eqir : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = command.a == registers[command.b] ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + command.a + " == " + (char) ('A' + command.b);
                }
            }

            public class Eqri : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] == command.b ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " == " + command.b;
                }
            }

            public class Eqrr : Instruction
            {
                public override void Process(Command command, long[] registers)
                {
                    registers[command.c] = registers[command.a] == registers[command.b] ? 1 : 0;
                }

                public override string ToString(Command command)
                {
                    return (char) ('A' + command.c) + " = " + (char) ('A' + command.a) + " == " + (char) ('A' + command.b);
                }
            }
        }
    }
}