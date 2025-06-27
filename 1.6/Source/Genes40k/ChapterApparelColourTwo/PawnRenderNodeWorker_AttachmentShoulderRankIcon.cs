using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class PawnRenderNodeWorker_AttachmentShoulderRankIcon : PawnRenderNodeWorker
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var pawn = parms.pawn;
        
        var apparelColourTwo = (ChapterBodyDecorativeApparelColourTwo)node.apparel;

        if (apparelColourTwo.RightShoulderIcon == Genes40kDefOf.BEWH_ShoulderNone)
        {
            return false;
        }
        
        if (parms.facing == Rot4.East)
        {
            return !apparelColourTwo.FlipShoulderIcons;
        }
            
        if (parms.facing == Rot4.West)
        {
            return apparelColourTwo.FlipShoulderIcons;
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
}