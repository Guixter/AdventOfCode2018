using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day24
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static int Part1()
        {
            var lines = Utils.GetLines(".\\Day24\\Input.txt");
            Army.Parse(lines, out var immune, out var infection);

            var winner = ComputeAllFights(immune, infection);

            return winner.groups
                .Select(x => x.nbUnits)
                .Sum();
        }

        public static int Part2()
        {
            var lines = Utils.GetLines(".\\Day24\\Input.txt");
            Army.Parse(lines, out var immune, out var infection);

            var boost = 0;
            Army winner;
            while ((winner = ComputeAllFights(immune, infection, false, boost)) != immune)
            {
                boost++;
            }

            return winner.groups
                .Select(x => x.nbUnits)
                .Sum();
        }

        private static Army ComputeAllFights(Army immune, Army infection, bool debug = false, int immuneBoost = 0)
        {
            immune.boost = immuneBoost;
            immune.Reset();
            infection.Reset();
            

            while (immune.IsAlive() && infection.IsAlive())
            {
                var groups = immune.groups
                    .Where(x => x.IsAlive())
                    .Concat(infection.groups
                        .Where(x => x.IsAlive())
                    )
                    .ToList();
                if (Fight(groups, immune, infection, debug) == 0)
                    return null;
            }

            return immune.IsAlive() ? immune : infection;
        }

        private static int Fight(List<Group> groups, Army immune, Army infection, bool debug = false)
        {
            if (debug)
            {
                Console.WriteLine("-----------------");
                immune.Print();
                infection.Print();
                Console.WriteLine();
            }


            // Reset the targets to null
            foreach (var group in groups)
            {
                group.target = null;
                group.attacker = null;
            }

            // Perform the target selection
            groups.Sort(Group.CompareToDefensive);
            foreach (var group in groups)
            {
                group.TargetSelection(debug);
            }
            if (debug)
                Console.WriteLine();

            // Perform the attacks
            var nbKilledUnits = 0;
            groups.Sort(Group.CompareToOffensive);
            foreach (var group in groups)
            {
                nbKilledUnits += group.Attack(debug);
            }
            if (debug)
                Console.WriteLine();

            return nbKilledUnits;
        }

        private class Army
        {
            public List<Group> groups;
            public string name;
            public Army enemy;
            public int boost;

            public bool IsAlive()
            {
                return groups
                    .Where(x => x.IsAlive())
                    .Count() > 0;
            }

            public static void Parse(string[] lines, out Army immune, out Army infection)
            {
                immune = new Army() {
                    groups = new List<Group>(),
                    name = "Immune System",
                };
                infection = new Army() {
                    groups = new List<Group>(),
                    name = "Infection",
                };
                immune.enemy = infection;
                infection.enemy = immune;

                var i = 1;
                Group group;
                while ((group = Group.Parse(lines[i], immune)) != null)
                {
                    immune.groups.Add(group);
                    i++;
                }

                i+= 2;
                while (i < lines.Length && (group = Group.Parse(lines[i], infection)) != null)
                {
                    infection.groups.Add(group);
                    i++;
                }
            }

            public void Print()
            {
                Console.WriteLine(name);
                for (var i = 0; i < groups.Count; i++)
                {
                    groups[i].Print();
                }
            }

            public void Reset()
            {
                foreach (var group in groups)
                {
                    group.nbUnits = group.initNbUnits;
                }
            }
        }

        private class Group
        {
            public int initNbUnits;
            public int nbUnits;
            public Army army;
            public int num;

            public int hitPoints;
            public int attackDamage;
            public int initiative;
            public string attackType;
            public string[] weaknesses;
            public string[] immunities;

            public Group target;
            public Group attacker;
            public int flag;

            private static readonly Regex wholeRegex = new Regex(@"(.*) units each with (.*) hit points (.*) ?with an attack that does (.*) (.*) damage at initiative (.*)");
            private static readonly Regex weaknessesRegex = new Regex(@"weak to (.*?)[;\)]");
            private static readonly Regex immunitiesRegex = new Regex(@"immune to (.*?)[;\)]");

            public int EffectivePower
            {
                get
                {
                    return nbUnits * (attackDamage + army.boost);
                }
            }

            public void TargetSelection(bool debug)
            {
                var candidates = army.enemy.groups.Where(x => x.IsAlive() && x.attacker == null);

                if (candidates.Count() > 0)
                {
                    foreach (var group in candidates)
                    {
                        group.flag = group.ComputeDamage(this);
                        if (debug)
                        {
                            Console.WriteLine(army.name + " " + num + " would deal defending group " + group.num + " " + group.flag + " damage");
                        }
                    }

                    var bestScore = candidates
                        .Select(x => x.flag)
                        .Max();

                    if (bestScore == 0)
                        return;

                    var bestCandidates = candidates
                        .Where(x => x.flag == bestScore)
                        .ToList();

                    bestCandidates.Sort(CompareToDefensive);
                    ChooseTarget(bestCandidates.First(), debug);
                }
            }

            public int Attack(bool debug)
            {
                if (IsAlive() && target != null)
                    return target.UndergoAttack(this, debug);
                else
                    return 0;
            }

            public int ComputeDamage(Group attacker)
            {
                var damage = attacker.EffectivePower;

                if (immunities.Contains(attacker.attackType))
                    damage = 0;
                if (weaknesses.Contains(attacker.attackType))
                    damage *= 2;

                return damage;
            }

            public bool IsAlive()
            {
                return nbUnits > 0;
            }

            public void ChooseTarget(Group target, bool debug)
            {
                target.attacker = this;
                this.target = target;

                if (debug)
                    Console.WriteLine(army.name + " " + num + " chose " + target.army.name + " " + target.num);
            }

            public int UndergoAttack(Group attacker, bool debug)
            {
                var killedUnits = ComputeDamage(attacker) / hitPoints;
                killedUnits = Math.Min(killedUnits, nbUnits);
                nbUnits -= killedUnits;

                if (debug)
                    Console.WriteLine(attacker.army.name + " group " + attacker.num + " attacks defending group " + num + ", killing " + killedUnits + " units");

                return killedUnits;
            }

            public static int CompareToDefensive(Group x, Group y)
            {
                var effectivePower = y.EffectivePower.CompareTo(x.EffectivePower);
                if (effectivePower != 0)
                    return effectivePower;

                var initiative = y.initiative.CompareTo(x.initiative);
                if (initiative != 0)
                    return initiative;

                return 1;
            }

            public static int CompareToOffensive(Group x, Group y)
            {
                var initiative = y.initiative.CompareTo(x.initiative);
                if (initiative != 0)
                    return initiative;

                return 1;
            }

            public static Group Parse(string line, Army army)
            {
                var match = wholeRegex.Match(line);
                if (match.Success)
                {
                    ParseStats(match.Groups[3].Value, out var weaknesses, out var immunities);
                    return new Group() {
                        army = army,
                        nbUnits = int.Parse(match.Groups[1].Value),
                        initNbUnits = int.Parse(match.Groups[1].Value),
                        hitPoints = int.Parse(match.Groups[2].Value),
                        attackDamage = int.Parse(match.Groups[4].Value),
                        initiative = int.Parse(match.Groups[6].Value),
                        attackType = match.Groups[5].Value,
                        weaknesses = weaknesses,
                        immunities = immunities,
                        num = army.groups.Count() + 1,
                    };
                }
                else
                {
                    return null;
                }
            }

            public static void ParseStats(string line, out string[] weaknesses, out string[] immunities)
            {
                line = line.Replace(", ", ",");

                var match = weaknessesRegex.Match(line);
                if (match.Success)
                    weaknesses = match.Groups[1].Value.Split(',');
                else
                    weaknesses = new string[0];

                match = immunitiesRegex.Match(line);
                if (match.Success)
                    immunities = match.Groups[1].Value.Split(',');
                else
                    immunities = new string[0];
            }

            public void Print()
            {
                if (nbUnits > 0)
                    Console.WriteLine("Group " + num + " contains " + nbUnits + " units (ep: " + EffectivePower + ", in: " + initiative + ")");
            }
        }
    }
}