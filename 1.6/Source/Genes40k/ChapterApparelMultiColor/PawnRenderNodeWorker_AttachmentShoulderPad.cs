using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNodeWorker_AttachmentShoulderPad : PawnRenderNodeWorker
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var pawn = parms.pawn;

        if (parms.Portrait)
        {
            if (parms.facing == Rot4.South || parms.facing == Rot4.North)
            {
                return false;
            }
            
            if ((parms.flags & PawnRenderFlags.Clothes) != PawnRenderFlags.Clothes)
            {
                return false;
            }
        }
        else
        {
            if (parms.posture is PawnPosture.LayingOnGroundNormal or PawnPosture.LayingOnGroundFaceUp)
            {
                return true;
            }
                
            if (pawn.Rotation == Rot4.North || pawn.Rotation == Rot4.South)
            {
                return false;
            }
                
            if (parms.posture == PawnPosture.Standing)
            {
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