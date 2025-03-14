using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentExtraDecoration : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            var pawn = parms.pawn;
            
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)pawn.apparel.WornApparel.FirstOrDefault(wornApparel => wornApparel is ExtraIconsChapterApparelColourTwo);

            var decoration = apparelColourTwo.ExtraDecorationDefs.Keys.FirstOrFallback(def => def.drawnTextureIconPath == node.Props.texPath);
            
            var showWhenFacing = new List<Rot4>();
            if (node.Props.flipGraphic)
            {
                showWhenFacing.AddRange(decoration.defaultShowRotation.Select(rotation => rotation.Opposite));
            } 
            else
            {
                showWhenFacing = decoration.defaultShowRotation;
            }
            if (parms.Portrait)
            {
                if (!showWhenFacing.Contains(parms.facing))
                {
                    return false;
                }
            }
            else
            {
                if (parms.posture == PawnPosture.LayingOnGroundNormal || parms.posture == PawnPosture.LayingOnGroundFaceUp)
                {
                    return true;
                }
                
                if (!showWhenFacing.Contains(parms.facing))
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
            
            return base.CanDrawNow(node, parms);
        }
    }
}