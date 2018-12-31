using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX2Data
{

    /// <summary>
    /// For mock-battles, table and handling of op-codes for general and specific skill handling.
    /// </summary>
    public static class DX2Opcodes
    {
        public delegate void SkillOpCode(Demon caster, Demon activeTarget);
        static Dictionary<string, SkillOpCode> opCodes_;

        static DX2Opcodes()
        {

        }

        public static bool Execute(Skill skill, Demon caster, Demon activeTarget)
        {
            if (skill.Element == Element.Recovery && caster.Team != activeTarget.Team)
            {
                return false;
            }
            return false;
        }
    }
}
