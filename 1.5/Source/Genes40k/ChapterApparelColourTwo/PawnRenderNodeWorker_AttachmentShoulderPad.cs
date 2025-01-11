using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentShoulderPad : PawnRenderNodeWorker
    {
        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            var vector = base.ScaleFor(node, parms);
            var bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
            return vector * ((bodyGraphicScale.x + bodyGraphicScale.y) / 2f);
        }
        
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            var pawn = parms.pawn;

            if (parms.Portrait)
            {
                if (parms.facing == Rot4.South || parms.facing == Rot4.North)
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

        /*protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            var apparelColourTwo = (ChapterApparelColourTwo)node.apparel;

            return GraphicDatabase.Get<Graphic_Multi>(node.Props.texPath, node.ShaderFor(parms.pawn), node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
        }*/
    }
}