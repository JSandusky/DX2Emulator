using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX2Data
{

    public enum Archetype
    {
        Clear,
        Red,
        Yellow,
        Purple,
        Teal
    }

    public enum Resistance
    {
        None,
        Weak,
        Resist,
        Null,
        Drain,
        Repel
    }

    public enum Status
    {
        Normal,
        Weak,
        Curse,
        Poison,
        Charm,
        RepelPhys,
        RepelMagic,
        RepelAlmighty,
        Lydia,
        Barrier,
        Mute,
        Bind,
        Charge,
        Might,
        Concentrate,
        Mortal
    }

    public enum Buff
    {
        Attack,
        Defense,
        EvadeAccuracy
    }

    public enum Element
    {
        Physical,
        Fire,
        Ice,
        Elec,
        Force,
        Light,
        Dark,
        Almighty,
        Ailment,
        Recovery,
        Passive,
        Support
    }

    public enum TargetKind
    {
        Self,
        SingleAlly,
        AllAllies,
        SingleEnemy,
        AllEnemies,
        RandomEnemies,
        Universal
    }

    public enum StatTarget
    {
        Str,
        Vit,
        Agi,
        Mag,
        Luck,
        Evade,
        HP,
        Crit,
        AllDamage,
        SingleTargetDamage,
        MultiTargetDamage
    }

    public static class DX2EnumExtensions
    {
        // meh, there's some ways of dealing with this generically, all kind of nasty and a bit too global (accident risk)
        public static string PrettyString(this Status[] status)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < status.Length; ++i)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(status[i].ToString());
            }
            return sb.ToString();
        }
    }
}
