using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentBackpack : PawnRenderNode
{
    public PawnRenderNode_AttachmentBackpack(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var backpackPath = Props.texPath;
            
        var apparelColourTwo = (ChapterBodyDecorativeApparelColourTwo)apparel;
            
        if (pawn.apparel.WornApparel.Any(wornApparel => wornApparel.def == Genes40kDefOf.Apparel_PackJump))
        {
            backpackPath += "_Jump";
        }
            
        return GraphicDatabase.Get<Graphic_Multi>(backpackPath, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
    }
}