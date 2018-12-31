using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX2Data
{
    public class DX2Game
    {

        public static void StartMatch(Team goingFirst, Team goingSecond)
        {
            // need to get a list of all demons and sort it so the sequence of auto-buffs is correct
            List<Demon> allDemons = new List<Demon>();
            allDemons.AddRange(goingFirst.Demons.Where(d => d != null).ToList());
            allDemons.AddRange(goingSecond.Demons.Where(d => d != null).ToList());
            allDemons.Sort((Demon l, Demon r) =>
            {
                if (l.Agility > r.Agility)
                    return -1;
                else if (l.Agility < r.Agility)
                    return 1;
                return 0;
            });

            foreach (var demon in allDemons)
            {
                var autoSkills = demon.Skills.Where((s) => !String.IsNullOrEmpty(s.AutoCast));
                // todo
            }

            int intimidateCt = 0;
            foreach (var demon in goingSecond.Demons)
            {
                // can only use intimidating stance on a max of 2 press turns
                if (intimidateCt < 2 && demon.HasSkill(DX2Strings.IntimidatingStance))
                {
                    goingFirst.PressTurns -= 1;
                    intimidateCt += 1;
                }

                Skill skill = demon.Skills.FirstOrDefault((s) => !String.IsNullOrEmpty(s.ShiftCast));
                // todo
            }

            foreach (var demon in goingFirst.Demons)
                demon.MP = 2;
            foreach (var demon in goingSecond.Demons)
                demon.MP = 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        /// <param name="turnCt">Total turns (including enemy turns)</param>
        public static void StartTurn(Team team, int turnCt)
        {
            // drift back towards 0 by 1 point each team turn
            team.AttackMod -= Math.Sign(team.AttackMod);
            team.DefenseMod -= Math.Sign(team.DefenseMod);
            team.EvadeAccMod -= Math.Sign(team.EvadeAccMod);
        }

        public static void StartTurn(Demon demon)
        {
            if (demon.HP >= 0)
                demon.MP += demon.HasSkill(DX2Strings.InfiniteChakra) ? 4 : 3;

            int maxMP = 10;
            if (demon.HasSkill(DX2Strings.ManaBonus))
                maxMP += 1;
            if (demon.HasSkill(DX2Strings.ManaGain))
                maxMP += 2;
            if (demon.MP > maxMP)
                demon.MP = maxMP;
        }

        public static void EndTurn(Demon demon)
        {
            if (demon.Status.Contains(Status.Poison))
            {

            }
        }
    }
}
