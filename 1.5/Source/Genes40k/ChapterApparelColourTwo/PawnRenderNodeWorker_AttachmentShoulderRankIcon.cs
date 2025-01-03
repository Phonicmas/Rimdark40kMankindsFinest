﻿using Core40k;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentShoulderRankIcon : PawnRenderNodeWorker_AttachmentBody
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            var pawn = parms.pawn;
            
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)node.apparel;

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

        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            var rightShoulderPath = node.Props.texPath;

            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)node.apparel;

            if (apparelColourTwo.RightShoulderIcon != null)
            {
                rightShoulderPath = apparelColourTwo.RightShoulderIcon.drawnTextureIconPath;
            }
            else if (parms.pawn.HasComp<CompRankInfo>())
            {
                var comp = parms.pawn.GetComp<CompRankInfo>();
                if (comp.HasRankOfCategory(Genes40kDefOf.BEWH_AstartesRankCategory))
                {
                    var highestRank = (ChapterRankDef)comp.HighestRankDef(true) ?? (ChapterRankDef)comp.HighestRankDef(false);
                    if (highestRank?.unlocksRankIcon != null)
                    {
                        rightShoulderPath = highestRank.unlocksRankIcon.drawnTextureIconPath;
                    }
                }
            }
            else
            {
                apparelColourTwo.RightShoulderIcon = Genes40kDefOf.BEWH_ShoulderNone;
                rightShoulderPath = apparelColourTwo.RightShoulderIcon.drawnTextureIconPath;
            }
            
            return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, node.Props.shaderTypeDef.Shader, node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, node.apparel.def.graphicData);
        }
    }
}