using RimWorld;
using Verse;

namespace Genes40k
{
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
            
            if (parms.Portrait)
            {
                if (parms.facing == Rot4.West)
                {
                    return false;
                }
            }
            else
            {
                if (parms.posture == PawnPosture.LayingOnGroundFaceUp)
                {
                    return true;
                }

                if (parms.posture == PawnPosture.LayingOnGroundNormal)
                {
                    return false;
                }
                
                if (pawn.Rotation == Rot4.West)
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
}