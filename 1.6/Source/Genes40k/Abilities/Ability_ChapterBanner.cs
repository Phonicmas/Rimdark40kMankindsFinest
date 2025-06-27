using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class Ability_ChapterBanner : VEF.Abilities.Ability
{
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        var intVec = targets[0].Cell;
            
        var flag = GenSpawn.Spawn(Genes40kDefOf.BEWH_AncientChapterBanner, intVec, pawn.Map);
        flag.SetFaction(Faction.OfPlayer);
    }
    
    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = false)
    {
        if (target.Cell.GetFirstBuilding(pawn.Map) != null)
        {
            Messages.Message("AbilityOccupiedCells".Translate(def.LabelCap), target.ToTargetInfo(pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
            
        return base.ValidateTarget(target, showMessages);
    }
}