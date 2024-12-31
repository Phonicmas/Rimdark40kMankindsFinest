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
        private Graphic cachedShoulderGraphic = null;

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
            //Change standard to use neophyte icon instead when made.
            const string standardPath = "Things/Armor/Imperium/PowerArmor/RankIcons/BEWH_Veteran";
            var pawn = parms.pawn;

            var rankIconPath = standardPath;
            
            var def = node.apparel.def;
            var apparelColourTwo = (ChapterApparelColourTwo)node.apparel;

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
                        if (highestRank != null)
                        {
                            rankIconPath = highestRank.unlocksRankIconPath;
                        }
                    }
                }
            }
            
            var shader = ShaderDatabase.CutoutComplex;
                    
            if (def.graphicData.shaderType != null)
            {
                shader = def.graphicData.shaderType.Shader;
            }
            
            //If not colorable, set color.white instead of drawcolors of apparel
            return GraphicDatabase.Get<Graphic_Multi>(rankIconPath, shader, node.Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, def.graphicData);
        }
    }
}