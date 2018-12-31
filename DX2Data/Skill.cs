using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DX2Data
{

    public class BasicBuff
    {
        public StatTarget Target { get; set; }
        // ie. Deadly Charm
        public Status RequiredStatus { get; set; } = Status.Normal;
        public int Value { get; set; }
    }

    // This needs a ton of refactoring.
    public class Skill
    {
        public string Name { get; set; }
        public string[] Description { get; set; }
        public string TargetingText { get; set; }
        public int MPCost { get; set; }

        public TargetKind TargetKind { get; set; }
        public Element Element { get; set; }
        public int Power { get; set; } = 0;
        public int ExtraCrit { get; set; } = 0;
        public int[] ElementalBoost { get; } = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public bool[] GrantResist { get; } = new bool[] { false, false, false, false, false, false, false, false, false };
        public bool[] GrantNull { get; } = new bool[] { false, false, false, false, false, false, false, false, false };
        public bool[] GrantRepel { get; } = new bool[] { false, false, false, false, false, false, false, false, false };
        public bool[] GrantDrain { get; } = new bool[] { false, false, false, false, false, false, false, false, false };
        public bool[] GrantPierce { get; } = new bool[] { false, false, false, false, false, false, false, false, false };
        public Status[] InflictStatus { get; set; }
        public Status[] RemoveStatus { get; set; }
        public Buff[] CauseBuffs { get; set; }
        public Buff[] CaseDebuffs { get; set; }
        public int BuffTurns { get; set; } = 3; // default to Raku/Taru/Suka 3
        public List<BasicBuff> BasicBuffs { get; set; }
        public int InflictStatusChance { get; set; } = 0;
        public int MinRandom { get; set; } = 1;
        public int MaxRandom { get; set; } = 1;
        public string AutoCast { get; set; }
        public string ShiftCast { get; set; }
        public bool EraseBuffs { get; set; } = false;
        public bool EraseDebuffs { get; set; } = false;

        public static Element ParseElement(string elementName)
        {
            elementName = elementName.ToLowerInvariant();
            if (elementName == "phys")
                return Element.Physical;
            else if (elementName == "fire")
                return Element.Fire;
            else if (elementName == "ice")
                return Element.Ice;
            else if (elementName == "ice")
                return Element.Ice;
            else if (elementName == "elec")
                return Element.Elec;
            else if (elementName == "force")
                return Element.Force;
            else if (elementName == "light")
                return Element.Light;
            else if (elementName == "dark")
                return Element.Dark;
            else if (elementName == "almighty")
                return Element.Almighty;
            else if (elementName == "ailment")
                return Element.Ailment;
            else if (elementName == "support")
                return Element.Support;
            else if (elementName == "passive")
                return Element.Passive;
            else if (elementName == "recovery")
                return Element.Recovery;
            Debug.Assert(false, "Failed to find element");
            return Element.Ailment;
        }

        public static TargetKind ParseTarget(string targetKind)
        {
            targetKind = targetKind.ToLowerInvariant();
            if (targetKind == "self")
                return TargetKind.Self;
            else if (targetKind == "single party member")
                return TargetKind.SingleAlly;
            else if (targetKind == "all party members")
                return TargetKind.AllAllies;
            else if (targetKind == "single enemy")
                return TargetKind.SingleEnemy;
            else if (targetKind == "all enemies")
                return TargetKind.AllEnemies;
            else if (targetKind == "random enemy/ies")
                return TargetKind.RandomEnemies;
            else if (targetKind == "universal" || targetKind == "all enemies/party members")
                return TargetKind.Universal;

            // passives
            if (string.IsNullOrEmpty(targetKind))
                return TargetKind.Self;

            Debug.Assert(false, "Unhandled targeting kind");
            return TargetKind.Universal;
        }

        public static Status[] ParseStatuses(string description)
        {
            if (description.Contains("chance to inflict status ailments"))
                return null;
            if (description.Contains("inflict Poison, Bind, Mute and Charm"))
                return new Status[] { Status.Charm, Status.Poison, Status.Bind, Status.Mute };
            var matches = inflictRegex.Matches(description);
            if (matches.Count > 0)
            {
                List<Status> statuses = new List<Status>();
                for (int i = 0; i < matches.Count; ++i)
                    statuses.Add((Status)Enum.Parse(typeof(Status), matches[i].Groups[1].Value));
                return statuses.ToArray();
            }
            return null;
        }

        public static List<Skill> ReadSkills(string filePath)
        {
            List<Skill> ret = new List<Skill>();
            using (System.IO.TextReader rdr = new System.IO.StringReader(System.IO.File.ReadAllText(filePath)))
            {
                using (Newtonsoft.Json.JsonTextReader jsonRdr = new Newtonsoft.Json.JsonTextReader(rdr))
                {
                    JArray root = (JArray)JToken.ReadFrom(jsonRdr);
                    if (root.Type == JTokenType.Array)
                    {
                        for (int i = 0; i < root.Count; ++i)
                        {
                            JObject skillData = (JObject)root[i];
                            Skill skill = new Skill();
                            skill.Name = skillData["Name"].Value<string>();
                            if (string.IsNullOrEmpty(skill.Name))
                                continue;

                            skill.Description = skillData["Description"].Value<string>().Split('\n');
                            skill.TargetingText = skillData["Target"].Value<string>();

                            skill.TargetKind = ParseTarget(skill.TargetingText);
                            skill.Element = ParseElement(skillData["Element"].Value<string>());

                            string costText = skillData["Cost"].Value<string>();
                            
                            if (costText.ToLowerInvariant().Trim() == "passive")
                                skill.MPCost = 0;
                            else
                                skill.MPCost = int.Parse(costText.Replace(" MP", ""));

                            skill.ExtractDataFromDescription();

                            ret.Add(skill);
                        }
                    }
                }
            }
            return ret;
        }

        static Regex inflictRegex = new Regex("inflict ([a-zA-Z]+)");
        static Regex powerRegex = new Regex("([0-9]+) power");
        static Regex critRateRegex = new Regex("([0-9]+)% crit rate");
        static Regex statusRateRegex = new Regex("([0-9]+)% chance to inflict");
        static Regex randomRangeRegex = new Regex("([0-9]+) to ([0-9]+) times");
        static Regex lockedRangeRegex = new Regex("([0-9]+) times");
        static Regex elementBoostRegex = new Regex("([0-9]+)% to ([a-zA-Z]+) damage");
        static Regex addResistRegex = new Regex("Adds ([a-zA-Z]+) Resist");
        static Regex addNullRegex = new Regex("Adds ([a-zA-Z]+) Null");
        static Regex addDrainRegex = new Regex("Adds ([a-zA-Z]+) Drain");
        static Regex addRepelRegex = new Regex("Adds ([a-zA-Z]+) Repel");
        static Regex addPierceRegex = new Regex("Adds ([a-zA-Z]+) Pierce");
        static Regex autoCastRegex = new Regex("Casts ([a-zA-Z]+) at the beginning of the 1st turn");
        static Regex shiftCastRegex = new Regex("Casts ([a-zA-Z]+) at the beginning of a battle if the enemy attacks first");
        static Regex singleCureRegex = new Regex("Cures a Single Party Member of ([a-zA-Z]+)");
        static Regex allCureRegex = new Regex("Cures All Party Members of ([a-zA-Z]+)");

        void ExtractDataFromDescription()
        {
            // Attack/healing power
            Power = 0;
            MatchCollection powMatches = powerRegex.Matches(Description[0]);
            if (powMatches.Count > 0)
                Power = int.Parse(powMatches[0].Groups[1].Value.ToLowerInvariant());

            // Extra crit chance
            MatchCollection critMatches = critRateRegex.Matches(Description[0]);
            if (critMatches.Count > 0)
                ExtraCrit = int.Parse(critMatches[0].Groups[1].Value.ToLowerInvariant());

            // Status
            MatchCollection statusRateMatches = statusRateRegex.Matches(Description[0]);
            if (statusRateMatches.Count > 0)
                InflictStatusChance = int.Parse(statusRateMatches[0].Groups[1].Value.ToLowerInvariant());
            InflictStatus = ParseStatuses(Description[0]);

            if (Description[0].Contains("enters a state") || Description[0].Contains("enter a state"))
            {
                List<Status> stat = new List<Status>();
                if (Description[0].Contains("Charge"))
                    stat.Add(Status.Charge);
                if (Description[0].Contains("Might"))
                    stat.Add(Status.Might);
                if (Description[0].Contains("Concentrate"))
                    stat.Add(Status.Concentrate);
                if (stat.Count > 0)
                    InflictStatus = stat.ToArray();
            }

            if (Description[0].Contains("Remove buffs"))
                EraseBuffs = true;
            if (Description[0].Contains("Remove debuffs"))
                EraseDebuffs = true;

            // Random and multi-attack
            if (TargetKind == TargetKind.RandomEnemies)
            {
                MatchCollection randomRateMatches = randomRangeRegex.Matches(Description[0]);
                if (randomRateMatches.Count > 0)
                {
                    MinRandom = int.Parse(randomRateMatches[0].Groups[1].Value.ToLowerInvariant());
                    MaxRandom = int.Parse(randomRateMatches[0].Groups[2].Value.ToLowerInvariant());
                }
                else
                {
                    MatchCollection fixedRateMatches = lockedRangeRegex.Matches(Description[0]);
                    if (fixedRateMatches.Count > 0)
                        MinRandom = MaxRandom = int.Parse(fixedRateMatches[0].Groups[1].Value.ToLowerInvariant());
                }
            }

            // phys boost, phys amp, etc
            var elementBoostMatches = elementBoostRegex.Matches(Description[0]);
            if (elementBoostMatches.Count > 0)
            {
                int boostPercent = int.Parse(elementBoostMatches[0].Groups[1].Value);
                Element elem = ParseElement(elementBoostMatches[0].Groups[2].Value);
                ElementalBoost[(int)elem] = boostPercent;
            }

            // Resistances
            var addResistMatches = addResistRegex.Matches(Description[0]);
            if (addResistMatches.Count > 0)
            {
                Element elem = ParseElement(addResistMatches[0].Groups[1].Value);
                GrantResist[(int)elem] = true;
            }

            // Nulls
            var addNullMatches = addNullRegex.Matches(Description[0]);
            if (addNullMatches.Count > 0)
            {
                Element elem = ParseElement(addNullMatches[0].Groups[1].Value);
                GrantNull[(int)elem] = true;
            }

            // Repels
            var addRepelMatches = addRepelRegex.Matches(Description[0]);
            if (addRepelMatches.Count > 0)
            {
                Element elem = ParseElement(addRepelMatches[0].Groups[1].Value);
                GrantRepel[(int)elem] = true;
            }

            // Drains
            var addDrainMatches = addDrainRegex.Matches(Description[0]);
            if (addDrainMatches.Count > 0)
            {
                Element elem = ParseElement(addDrainMatches[0].Groups[1].Value);
                GrantDrain[(int)elem] = true;
            }

            // Pierce
            var addPierceMatches = addPierceRegex.Matches(Description[0]);
            if (addPierceMatches.Count > 0)
            {
                Element elem = ParseElement(addPierceMatches[0].Groups[1].Value);
                GrantPierce[(int)elem] = true;
            }

            // Auto-buffs
            var autoBuffMatches = autoCastRegex.Matches(Description[0]);
            if (autoBuffMatches.Count > 0)
                AutoCast = autoBuffMatches[0].Groups[1].Value;

            // Shift-casts
            var shiftCastMatches = shiftCastRegex.Matches(Description[0]);
            if (shiftCastMatches.Count > 0)
                ShiftCast = shiftCastMatches[0].Groups[1].Value;

            var cureSingleMatches = singleCureRegex.Matches(Description[0]);
            if (cureSingleMatches.Count > 0)
                RemoveStatus = new Status[] { (Status)Enum.Parse(typeof(Status), cureSingleMatches[0].Groups[1].Value) };

            var cureAllMatches = allCureRegex.Matches(Description[0]);
            if (cureAllMatches.Count > 0)
                RemoveStatus = new Status[] { (Status)Enum.Parse(typeof(Status), cureAllMatches[0].Groups[1].Value) };

            // Status ailment removal, targeting mode will cover Silent Prayer and single/all party cases
            if (Description[0].Contains("Cures all status ailments"))
                RemoveStatus = AllStatus;
        }

        static Status[] AllStatus = new Status[]
        {
            Status.Weak,
            Status.Curse,
            Status.Poison,
            Status.Charm,
            Status.RepelPhys,
            Status.RepelMagic,
            Status.RepelAlmighty,
            Status.Lydia,
            Status.Barrier,
            Status.Mute,
            Status.Bind,
            Status.Charge,
            Status.Might,
            Status.Concentrate
        };

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Name);
            sb.AppendFormat("    {0} {1}\r\n", Element.ToString(), TargetKind.ToString());
            if (MPCost != 0)
                sb.AppendFormat("    {0} MP\r\n", MPCost);
            if (Power > 0)
                sb.AppendFormat("    {0} Power\r\n", Power);
            if (ExtraCrit > 0)
                sb.AppendFormat("    +{0}% Crit Chance\r\n", ExtraCrit);
            if (InflictStatusChance != 0)
                sb.AppendFormat("    Status Chance {0}%\r\n", InflictStatusChance);
            if (InflictStatus != null)
                sb.AppendFormat("    Inflicts {0}\r\n", InflictStatus.PrettyString());
            if (RemoveStatus != null)
                sb.AppendFormat("    Removes {0}\r\n", RemoveStatus.PrettyString());
            for (int i = 0; i < ElementalBoost.Length; ++i)
                if (ElementalBoost[i] != 0)
                    sb.AppendFormat("    +{0}% to {1} damage\r\n", ElementalBoost[i], (Element)i);
            for (int i = 0; i < GrantResist.Length; ++i)
                if (GrantResist[i])
                    sb.AppendFormat("    Adds Resist {0}\r\n", (Element)i);
            for (int i = 0; i < GrantNull.Length; ++i)
                if (GrantNull[i])
                    sb.AppendFormat("    Adds Null {0}\r\n", (Element)i);
            for (int i = 0; i < GrantRepel.Length; ++i)
                if (GrantRepel[i])
                    sb.AppendFormat("    Adds Repel {0}\r\n", (Element)i);
            for (int i = 0; i < GrantDrain.Length; ++i)
                if (GrantDrain[i])
                    sb.AppendFormat("    Adds Drain {0}\r\n", (Element)i);

            return sb.ToString();
        }

        public string DisplayString {
            get {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Description.Length; ++i)
                {
                    if (i < Description.Length - 1)
                        sb.AppendLine(Description[i]);
                    else
                        sb.Append(Description[i]);
                }
                return sb.ToString();
            }
        }
    }
}
