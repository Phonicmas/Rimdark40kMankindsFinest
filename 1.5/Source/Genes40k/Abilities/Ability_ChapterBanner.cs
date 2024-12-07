using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Ability_ChapterBanner : VFECore.Abilities.Ability
    {
        internal IEnumerable<IntVec3> TotalAffectedCells(LocalTargetInfo target, Map map, IEnumerable<IntVec3> affectedCells)
        {
            return from intVec in affectedCells
                select target.Cell + new IntVec3(intVec.x, 0, intVec.z) into intVec2
                where intVec2.InBounds(map)
                select intVec2;
        }
        
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var intVec = targets[0].Cell;
            
            var flag = GenSpawn.Spawn(Genes40kDefOf.BEWH_AncientChapterBanner, intVec, pawn.Map);
            flag.SetFaction(Faction.OfPlayer);
        }
        

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = false)
        {
            if (target.Cell.GetFirstBuilding(pawn.Map) == null)
            {
                return true;
            }
            /*if (!target.Cell.Filled(pawn.Map))
            {
                return true;
            }*/
            
            Messages.Message("AbilityOccupiedCells".Translate(def.LabelCap), target.ToTargetInfo(pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
            return false;

        }
    }
}