using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day19
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static long Part1()
        {
            var lines = Utils.GetLines(".\\Day19\\Input.txt");

            Command.Parse(lines, out var binding, out var commands);
            var registers = new long[6];
            Compute(binding, commands, registers, false);

            return registers[0];
        }

        public static long Part2()
        {
            var lines = Utils.GetLines(".\\Day19\\Input.txt");

            Command.Parse(lines, out var binding, out var commands);
            var registers = new long[6] { 1, 0, 0, 0, 0, 0 };

            Compute(binding, commands, registers, false, (ip, reg) => {
                // Few "hacks" to speed the process up
                // (I found them by retro-engineering the code, thanks to the debugger)
                if (ip == 3 && reg[2] == 1)
                {
                    reg[2] = reg[3] / reg[5];
                }

                if (ip == 9)
                {
                    reg[2] = reg[3] + 1;
                }

                return true;
            });

            return registers[0];
        }

        public static void Compute(int binding, Command[] commands, long[] registers, bool debug = false, Func<long, long[], bool> additionalStep = null)
        {
            var ip = (long) 0;

            while (ip < commands.Length)
            {
                var current = commands[ip];

                // Perform an additional step if necessary
                if (additionalStep != null && !additionalStep.Invoke(ip, registers))
                    break;

                if (debug)
                    Console.Write("ip=" + ip + " ");

                // Save the ip in the register
                registers[binding] = ip;

                if (debug) {
                    PrintRegisters(registers);
                    Console.Write(" " + current.instruction.ToString(current.command) + " ");
                }

                // Compute the instruction
                current.instruction.Process(current.command, registers);
                if (debug) {
                    PrintRegisters(registers);
                    Console.WriteLine();
                }

                // Get the register back to the ip
                ip = registers[binding];

                // Increment the ip
                ip++;

                if (debug)
                    Debugger(registers);
            }
        }

        private static void Debugger(long[] registers)
        {
            var regex = new Regex(@"(.*) = (.*)");

            var input = Console.ReadLine();
            while (!string.IsNullOrEmpty(input))
            {
                var match = regex.Match(input);
                if (match.Success)
                {
                    var register =  match.Groups[1].Value[0] - 'A';
                    var value = int.Parse(match.Groups[2].Value);

                    registers[register] = value;
                }

                PrintRegisters(registers);
                input = Console.ReadLine();
            }
        }

        private static void PrintRegisters(long[] registers)
        {
            Console.Write("[" + registers[0]);
            for (var i = 1; i < registers.Length; i++)
            {
                Console.Write(", " + registers[i]);
            }
            Console.Write("]");
        }

        public struct Command
        {
            public Day16.Command command;
            public Day16.Instruction instruction;
            public string name;

            private static readonly Regex commandRegex = new Regex(@"(.*) (.*) (.*) (.*)");
            private static readonly Regex bindRegex = new Regex(@"#ip (.*)");

            public static Day16.Instruction ParseInstruction(string name)
            {
                switch (name)
                {
                    case "addr":
                        return new Day16.Instruction.Addr();
                    case "addi":
                        return new Day16.Instruction.Addi();
                    case "mulr":
                        return new Day16.Instruction.Mulr();
                    case "muli":
                        return new Day16.Instruction.Muli();
                    case "banr":
                        return new Day16.Instruction.Banr();
                    case "bani":
                        return new Day16.Instruction.Bani();
                    case "borr":
                        return new Day16.Instruction.Borr();
                    case "bori":
                        return new Day16.Instruction.Bori();
                    case "setr":
                        return new Day16.Instruction.Setr();
                    case "seti":
                        return new Day16.Instruction.Seti();
                    case "gtir":
                        return new Day16.Instruction.Gtir();
                    case "gtri":
                        return new Day16.Instruction.Gtri();
                    case "gtrr":
                        return new Day16.Instruction.Gtrr();
                    case "eqir":
                        return new Day16.Instruction.Eqir();
                    case "eqri":
                        return new Day16.Instruction.Eqri();
                    case "eqrr":
                        return new Day16.Instruction.Eqrr();
                }
                return null;
            }

            public static Command ParseCommand(string line)
            {
                var match = commandRegex.Match(line);
                return new Command() {
                    instruction = ParseInstruction(match.Groups[1].Value),
                    command = new Day16.Command()
                    {
                        a = int.Parse(match.Groups[2].Value),
                        b = int.Parse(match.Groups[3].Value),
                        c = int.Parse(match.Groups[4].Value),
                    },
                    name = match.Groups[1].Value,
                };
            }

            public static int ParseBinding(string line)
            {
                var match = bindRegex.Match(line);
                return int.Parse(match.Groups[1].Value);
            }

            public static void Parse(string[] lines, out int binding, out Command[] commands)
            {
                binding = ParseBinding(lines[0]);
                commands = new Command[lines.Length - 1];
                for (var i = 1; i < lines.Length; i++)
                {
                    commands[i - 1] = ParseCommand(lines[i]);
                }
            }

            public override string ToString()
            {
                var builder = new StringBuilder();

                builder.Append(name);
                builder.Append(" ");
                builder.Append(command.a);
                builder.Append(" ");
                builder.Append(command.b);
                builder.Append(" ");
                builder.Append(command.c);

                return builder.ToString();
            }
        }
    }
}