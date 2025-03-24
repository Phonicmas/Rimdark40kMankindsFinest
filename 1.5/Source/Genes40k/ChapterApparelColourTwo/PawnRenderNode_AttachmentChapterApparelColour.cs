using Verse;

namespace Genes40k
{
    public class PawnRenderNode_AttachmentChapterApparelColour : PawnRenderNode
    {
        public PawnRenderNode_AttachmentChapterApparelColour(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            var apparelColourTwo = (ChapterBodyDecorativeApparelColourTwo)apparel;

            return GraphicDatabase.Get<Graphic_Multi>(Props.texPath, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
        }
    }
}