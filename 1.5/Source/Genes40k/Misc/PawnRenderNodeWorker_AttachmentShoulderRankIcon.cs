using System.Linq;
using Core40k;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentShoulderRankIcon : PawnRenderNodeWorker_AttachmentBody
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            var pawn = parms.pawn;

            if (!pawn.HasComp<CompRankInfo>() || !pawn.GetComp<CompRankInfo>().HasRankOfCategory(Genes40kDefOf.BEWH_AstartesRankCategory))
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
            var pawn = parms.pawn;

            var rankIconPath = node.Props.texPath;
            
            var def = node.apparel.def;
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)node.apparel;

            if (apparelColourTwo.OverrideRankIcon != null)
            {
                rankIconPath = apparelColourTwo.OverrideRankIcon;
            }
            else
            {
                if (pawn.HasComp<CompRankInfo>())
                {
                    var comp = pawn.GetComp<CompRankInfo>();
                    if (comp.HasRankOfCategory(Genes40kDefOf.BEWH_AstartesRankCategory))
                    {
                        var highestRank = (ChapterRankDef)comp.HighestRankDef(true) ?? (ChapterRankDef)comp.HighestRankDef(false);
                        if (highestRank != null && highestRank.unlocksRankIconPath != string.Empty)
                        {
                            rankIconPath = highestRank.unlocksRankIconPath;
                        }
                    }
                }
            }
            
            //If not colorable, set color.white instead of drawcolors of apparel
            return GraphicDatabase.Get<Graphic_Multi>(rankIconPath, node.Props.shaderTypeDef.Shader, node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, def.graphicData);
        }
    }
}