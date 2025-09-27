using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class PawnRenderNodeWorker_AttachmentShoulderRankIcon : PawnRenderNodeWorker
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var pawn = parms.pawn;
        
        if (!node.apparel.HasComp<CompChapterColorWithShoulderDecoration>())
        {
            return false;
        }

        var chapterDecoComp = node.apparel.GetComp<CompChapterColorWithShoulderDecoration>();

        if (chapterDecoComp.RightShoulderIcon == Genes40kDefOf.BEWH_ShoulderNone)
        {
            return false;
        }
        
        if (parms.facing == Rot4.East)
        {
            return !chapterDecoComp.FlipShoulderIcons;
        }
            
        if (parms.facing == Rot4.West)
        {
            return chapterDecoComp.FlipShoulderIcons;
        }
            
        if (parms.Portrait)
        {
            if ((parms.flags & PawnRenderFlags.Clothes) != PawnRenderFlags.Clothes)
            {
                return false;
            }
        }
        else
        {
            switch (parms.posture)
            {
                case PawnPosture.LayingOnGroundFaceUp:
                case PawnPosture.LayingOnGroundNormal:
                case PawnPosture.Standing:
                    return true;
            }

            var mindState = pawn.mindState;
            if (mindState != null && mindState.duty?.def?.drawBodyOverride.HasValue == true)
            {
                return pawn.mindState.duty.def.drawBodyOverride.Value;
            }
            if (parms.bed != null && parms.pawn.RaceProps.Humanlike)
            {
                return parms.bed.def.building.bed_showSleeperBody;
            }
        }

        return true;
    }
    
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        var res = base.OffsetFor(node, parms, out pivot);
        if (node.apparel.GetComp<CompChapterColorWithShoulderDecoration>().FlipShoulderIcons && node.apparel.def.HasModExtension<DefModExtension_ShoulderFlippedData>())
        {
            var flippedData = node.apparel.def.GetModExtension<DefModExtension_ShoulderFlippedData>();
            if (flippedData.offsetForRankWhenFacing.TryGetValue(parms.facing, out var offset))
            {
                return offset;
            }
        }
        return res; 
    }
}