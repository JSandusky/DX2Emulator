using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DX2Data
{
    public class ResistanceData
    {
        public Resistance Phys { get; set; }
        public Resistance Fire { get; set; }
        public Resistance Ice { get; set; }
        public Resistance Elec { get; set; }
        public Resistance Force { get; set; }
        public Resistance Light { get; set; }
        public Resistance Dark { get; set; }

        public ResistanceData Clone()
        {
            ResistanceData r = new ResistanceData();
            r.Phys = Phys;
            r.Fire = Fire;
            r.Ice = Ice;
            r.Elec = Elec;
            r.Force = Force;
            r.Light = Light;
            r.Dark = Dark;
            return r;
        }

        public Resistance Get(int index)
        {
            switch (index)
            {
            case 0:
                return Phys;
            case 1:
                return Fire;
            case 2:
                return Ice;
            case 3:
                return Elec;
            case 4:
                return Force;
            case 5:
                return Light;
            case 6:
                return Dark;
            }
            return Resistance.None;
        }

        public void Set(int index, Resistance val)
        {
            switch (index)
            {
            case 0:
                Phys = val;
                break;
            case 1:
                Fire = val;
                break;
            case 2:
                Ice = val;
                break;
            case 3:
                Elec = val;
                break;
            case 4:
                Force = val;
                break;
            case 5:
                Light = val;
                break;
            case 6:
                Dark = val;
                break;
            }
        }
    }

    public class DemonRecord
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Image { get; set; }
        public Archetype Archetype { get; set; }

        public int Strength { get; set; }
        public int Magic { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Luck { get; set; }

        public Skill Skill1 { get; set; }
        public Skill Skill2 { get; set; }
        public Skill Skill3 { get; set; }
        public Skill AwakenSkill { get; set; }
        public Skill GachaSkill { get; set; }

        public ResistanceData Resistances { get; set; } = new ResistanceData();

        internal DemonRecord Clone()
        {
            DemonRecord r = new DemonRecord();
            r.Name = Name;
            r.Race = Race;
            r.Image = Image;
            r.Strength = Strength;
            r.Vitality = Vitality;
            r.Agility = Agility;
            r.Luck = Luck;
            r.Magic = Magic;
            r.Skill1 = Skill1;
            r.Skill2 = Skill2;
            r.Skill3 = Skill3;

            r.Resistances = Resistances.Clone();
            return r;
        }
    }

    public class Demon
    {
        // Circular #1
        public Team Team { get; set; }
        public DemonRecord Kind { get; set; }
        public int Strength { get; set; }
        public int Magic { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Luck { get; set; }

        public int HP { get; set; }
        public int MP { get; set; } = 2;
        public int PhysicalAttack { get; set; }
        public int MagicAttack { get; set; }
        public int PhysicalDefense { get; set; }
        public int MagicDefense { get; set; }

        public Skill[] Skills { get; private set; } = new Skill[6];
        public List<Status> Status { get; private set; } = new List<Status>();

        public ResistanceData Resistances { get; set; } = new ResistanceData();

        public bool DivineBrands { get; set; }
        public bool LeadBrands { get; set; }

        public bool HasSkill(string name)
        {
            for (int i = 0; i < 6; ++i)
            {
                if (Skills[i] != null && String.Compare(Skills[i].Name, name, true) == 0)
                    return true;
            }
            return false;
        }

        public void RecalcStats(bool combatStats = true)
        {
            Strength = Kind.Strength;
            Vitality = Kind.Vitality;
            Agility = Kind.Agility;
            Magic = Kind.Magic;
            Luck = Kind.Luck;

            Resistances = Kind.Resistances.Clone();

            foreach (var skill in Skills)
            {
                if (skill != null)
                {
                    for (int i = 0; i < 7; ++i)
                        if (skill.GrantResist[i])
                            Resistances.Set(i, Resistance.Resist);
                    for (int i = 0; i < 7; ++i)
                        if (skill.GrantDrain[i])
                            Resistances.Set(i, Resistance.Drain);
                    for (int i = 0; i < 7; ++i)
                        if (skill.GrantNull[i])
                            Resistances.Set(i, Resistance.Null);
                    for (int i = 0; i < 7; ++i)
                        if (skill.GrantRepel[i])
                            Resistances.Set(i, Resistance.Repel);
                }
            }

            if (HasSkill("Strength Amp I"))
                Strength += 5;
            if (HasSkill("Strength Amp II"))
                Strength += 10;
            if (HasSkill("Strength Amp III"))
                Strength += 15;

            if (HasSkill("Magic Amp I"))
                Magic += 5;
            if (HasSkill("Magic Amp II"))
                Magic += 10;
            if (HasSkill("Magic Amp III"))
                Magic += 15;

            if (HasSkill("Agility Amp I"))
                Agility += 5;
            if (HasSkill("Agility Amp II"))
                Agility += 10;
            if (HasSkill("Agility Amp III"))
                Agility += 15;

            if (HasSkill("Vitality Amp I"))
                Vitality += 5;
            if (HasSkill("Vitality Amp II"))
                Vitality += 10;
            if (HasSkill("Vitality Amp III"))
                Vitality += 15;

            if (HasSkill("Luck Amp I"))
                Luck += 5;
            if (HasSkill("Luck Amp II"))
                Luck += 10;
            if (HasSkill("Luck Amp III"))
                Luck += 15;

            if (combatStats)
            {
                PhysicalAttack = (int)(Strength * 2.1 + 50 * 5.6 + 50);
                MagicAttack = (int)(Magic * 2.1 + 50 * 5.6 + 50);
                PhysicalDefense = (int)(Vitality * 1.1 + Strength * 0.5 + 50 * 5.6 + 50);
                MagicDefense = (int)(Vitality * 1.1 + Magic * 0.5 + 50 * 5.6 + 50);
            }
        }

        public void SetKind(DemonRecord rec, bool overwriteStats, bool overwriteGacha)
        {
            Kind = rec;

            Skills[0] = Kind.Skill1;
            Skills[1] = Kind.Skill2;
            Skills[2] = Kind.Skill3;
            Skills[3] = Kind.AwakenSkill;
            if (overwriteGacha)
                Skills[4] = Kind.GachaSkill;

            if (overwriteStats)
                RecalcStats();
        }

        public Demon Clone()
        {
            Demon r = new Demon();
            r.Kind = Kind;
            r.Resistances = Resistances.Clone();
            r.RecalcStats(false);
            r.MagicAttack = MagicAttack;
            r.PhysicalAttack = PhysicalAttack;
            r.MagicDefense = MagicDefense;
            r.PhysicalDefense = PhysicalDefense;

            r.DivineBrands = DivineBrands;
            r.LeadBrands = LeadBrands;

            r.Skills[0] = Skills[0];
            r.Skills[1] = Skills[1];
            r.Skills[2] = Skills[2];
            r.Skills[3] = Skills[3];
            r.Skills[4] = Skills[4];
            r.Skills[5] = Skills[5];

            return r;
        }
    }

    public class Team
    {
        public String Name { get; set; }
        public List<Demon> Demons { get; private set; } = new List<Demon>();
        public int AttackMod { get; set; }
        public int DefenseMod { get; set; }
        public int EvadeAccMod { get; set; }
        public float PressTurns { get; set; } = 4.0f;

        public float GetAttackModifier()
        {
            if (AttackMod > 0)
                return 1.2f;
            else if (AttackMod < 0)
                return 0.8f;
            return 1.0f;
        }

        public float GetDefenseModifier()
        {
            if (DefenseMod > 0)
                return 0.83f;
            else if (DefenseMod < 0)
                return 1.25f;
            return 1.0f;
        }

        public float GetEvAccModifier()
        {
            if (EvadeAccMod > 0)
                return 1.2f; //?? based on text
            else if (EvadeAccMod < 0)
                return 0.8f; //?? based on text
            return 1.0f;
        }

        public void Save(XmlElement into)
        {
            var team = into.OwnerDocument.CreateElement("team");
            team.SetAttribute("name", Name);
            into.AppendChild(team);
            foreach (var demon in Demons)
            {
                if (demon != null)
                {
                    var dem = into.OwnerDocument.CreateElement("demon");
                    dem.SetAttribute("name", demon.Kind.Name);
                    dem.SetAttribute("archetype", demon.Kind.Archetype.ToString());
                    dem.SetAttribute("patk", demon.PhysicalAttack.ToString());
                    dem.SetAttribute("matk", demon.MagicAttack.ToString());
                    dem.SetAttribute("pdef", demon.PhysicalDefense.ToString());
                    dem.SetAttribute("mdef", demon.MagicDefense.ToString());
                    if (demon.Skills[4] != null)
                        dem.SetAttribute("transfer1", demon.Skills[4].Name);
                    if (demon.Skills[5] != null)
                        dem.SetAttribute("transfer2", demon.Skills[5].Name);
                    dem.SetAttribute("divine", demon.DivineBrands.ToString());
                    dem.SetAttribute("lead", demon.LeadBrands.ToString());

                    team.AppendChild(dem);
                }
            }
        }

        public bool Load(XmlElement from, DemonDB database, List<Skill> skills)
        {
            Name = from.GetAttribute("name");
            
            foreach (var child in from.ChildNodes)
            {
                XmlElement dem = child as XmlElement;
                if (dem == null)
                    continue;

                Demon demon = new Demon();
                string name = dem.GetAttribute("name");
                Archetype arch = (Archetype)Enum.Parse(typeof(Archetype), dem.GetAttribute("archetype"));
                demon.Kind = database.Demons.FirstOrDefault(d => d.Name == name && d.Archetype == arch);
                string s1 = dem.GetAttribute("transfer1");
                string s2 = dem.GetAttribute("transfer2");

                demon.Skills[0] = demon.Kind.Skill1;
                demon.Skills[1] = demon.Kind.Skill2;
                demon.Skills[2] = demon.Kind.Skill3;
                demon.Skills[3] = demon.Kind.AwakenSkill;
                
                demon.Skills[4] = skills.FirstOrDefault(s => string.Compare(s.Name, s1, true) == 0);
                Debug.Assert(string.IsNullOrEmpty(s1) || demon.Skills[4] != null);
                demon.Skills[5] = skills.FirstOrDefault(s => string.Compare(s.Name, s2, true) == 0);
                Debug.Assert(string.IsNullOrEmpty(s2) || demon.Skills[5] != null);

                demon.MagicAttack = int.Parse(dem.GetAttribute("matk"));
                demon.MagicDefense = int.Parse(dem.GetAttribute("mdef"));
                demon.PhysicalAttack = int.Parse(dem.GetAttribute("patk"));
                demon.PhysicalDefense = int.Parse(dem.GetAttribute("pdef"));
                demon.DivineBrands = bool.Parse(dem.GetAttribute("divine"));
                demon.LeadBrands = bool.Parse(dem.GetAttribute("lead"));

                demon.RecalcStats(false);
                Demons.Add(demon);
            }

            while (Demons.Count < 4)
                Demons.Add(null);

            return true;
        }

        #region GUI Utility

        public string P1 { get { return Demons[0].Kind.Image; } }
        public string P2 { get { return Demons[1].Kind.Image; } }
        public string P3 { get { return Demons[2].Kind.Image; } }
        public string P4 { get { return Demons[3].Kind.Image; } }

        #endregion
    }

    public class DemonDB
    {
        public SortedSet<string> DemonNames = new SortedSet<string>();
        public List<DemonRecord> Demons = new List<DemonRecord>();

        Resistance ParseResist(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return Resistance.None;
            else if (txt == "rs")
                return Resistance.Resist;
            else if (txt == "nu")
                return Resistance.Null;
            else if (txt == "dr")
                return Resistance.Drain;
            else if (txt == "rp")
                return Resistance.Repel;
            else if (txt == "wk")
                return Resistance.Weak;
            return Resistance.None;
        }

        #region Save / Load

        public void Load(string filePath, List<Skill> skillsList)
        {
            using (System.IO.TextReader rdr = new System.IO.StringReader(System.IO.File.ReadAllText(filePath)))
            {
                using (Newtonsoft.Json.JsonTextReader jsonRdr = new Newtonsoft.Json.JsonTextReader(rdr))
                {
                    JArray root = (JArray)JToken.ReadFrom(jsonRdr);
                    for (int i = 0; i < root.Count; ++i)
                    {
                        JObject demonData = (JObject)root[i];

                        DemonRecord clear = new DemonRecord();
                        clear.Archetype = Archetype.Clear;
                        clear.Name = demonData["Name"].Value<string>();
                        clear.Image = "Demons/" + clear.Name + ".png";
                        clear.Race = demonData["Race"].Value<string>();
                        clear.Strength = demonData["6★ Strength"].Value<int>();
                        clear.Magic = demonData["6★ Magic"].Value<int>();
                        clear.Agility = demonData["6★ Agility"].Value<int>();
                        clear.Vitality = demonData["6★ Vitality"].Value<int>();
                        clear.Luck = demonData["6★ Luck"].Value<int>();

                        clear.Resistances.Phys = ParseResist(demonData["Phys"].Value<string>());
                        clear.Resistances.Fire = ParseResist(demonData["Fire"].Value<string>());
                        clear.Resistances.Ice = ParseResist(demonData["Ice"].Value<string>());
                        clear.Resistances.Elec = ParseResist(demonData["Elec"].Value<string>());
                        clear.Resistances.Force = ParseResist(demonData["Force"].Value<string>());
                        clear.Resistances.Light = ParseResist(demonData["Light"].Value<string>());
                        clear.Resistances.Dark = ParseResist(demonData["Dark"].Value<string>());

                        DemonNames.Add(clear.Name);

                        clear.Skill1 = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Skill 1"].Value<string>()) == 0);
                        clear.Skill2 = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Skill 2"].Value<string>()) == 0);
                        clear.Skill3 = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Skill 3"].Value<string>()) == 0);
                        clear.AwakenSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Clear Awaken"].Value<string>()) == 0);
                        Demons.Add(clear);

                        DemonRecord red = clear.Clone();
                        red.Archetype = Archetype.Red;
                        red.AwakenSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Red Awaken"].Value<string>()) == 0);
                        red.GachaSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Red Gacha"].Value<string>()) == 0);
                        Demons.Add(red);

                        DemonRecord yellow = clear.Clone();
                        yellow.Archetype = Archetype.Yellow;
                        yellow.AwakenSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Yellow Awaken"].Value<string>()) == 0);
                        yellow.GachaSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Yellow Gacha"].Value<string>()) == 0);
                        Demons.Add(yellow);

                        DemonRecord purple = clear.Clone();
                        purple.Archetype = Archetype.Purple;
                        purple.AwakenSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Purple Awaken"].Value<string>()) == 0);
                        purple.GachaSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Purple Gacha"].Value<string>()) == 0);
                        Demons.Add(purple);

                        DemonRecord teal = clear.Clone();
                        teal.Archetype = Archetype.Teal;
                        teal.AwakenSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Teal Awaken"].Value<string>()) == 0);
                        teal.GachaSkill = skillsList.FirstOrDefault(s => string.Compare(s.Name, demonData["Teal Gacha"].Value<string>()) == 0);
                        Demons.Add(teal);
                    }
                }
            }
        }

#endregion
    }
}
