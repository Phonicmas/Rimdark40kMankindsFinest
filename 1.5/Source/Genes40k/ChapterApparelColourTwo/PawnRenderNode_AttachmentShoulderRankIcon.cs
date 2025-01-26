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
            var drawColour = apparelColourTwo.RightShoulderIconColour;
            
            if (apparelColourTwo.RightShoulderIcon != null)
            {
                rightShoulderPath = apparelColourTwo.RightShoulderIcon.drawnTextureIconPath;
            }
            
            return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour);
        }
    }
}