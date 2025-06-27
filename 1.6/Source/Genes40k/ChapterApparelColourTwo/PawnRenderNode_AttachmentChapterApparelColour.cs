using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentChapterApparelColour : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentChapterApparelColour(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
    {
    }

    public override Graphic GraphicFor(Pawn pawn)
    {
        var apparelColourTwo = (ChapterBodyDecorativeApparelColourTwo)apparel;

        return GraphicDatabase.Get<Graphic_Multi>(Props.texPath, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}