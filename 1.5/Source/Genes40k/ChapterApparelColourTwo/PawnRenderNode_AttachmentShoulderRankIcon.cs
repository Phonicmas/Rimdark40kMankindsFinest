using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode
    {
        public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            var rightShoulderPath = Props.texPath;
            
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)apparel;
            //var drawColour = apparelColourTwo.RightShoulderIconColour;
            var drawColour = apparelColourTwo.DrawColor;
            
            if (apparelColourTwo.RightShoulderIcon != null)
            {
                rightShoulderPath = apparelColourTwo.RightShoulderIcon.drawnTextureIconPath;
            }
            else if (pawn.HasComp<CompRankInfo>())
            {
                var comp = pawn.GetComp<CompRankInfo>();
                if (comp.HasRankOfCategory(Genes40kDefOf.BEWH_AstartesRankCategory))
                {
                    var highestRank = (ChapterRankDef)comp.HighestRankDef(true) ?? (ChapterRankDef)comp.HighestRankDef(false);
                    if (highestRank?.unlocksRankIcon != null)
                    {
                        rightShoulderPath = highestRank.unlocksRankIcon.drawnTextureIconPath;
                    }
                }
            }
            
            return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour);
        }
    }
}