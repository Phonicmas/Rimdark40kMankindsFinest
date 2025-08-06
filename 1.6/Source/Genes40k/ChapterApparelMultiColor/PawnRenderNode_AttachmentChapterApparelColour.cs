using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentChapterApparelColour : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentChapterApparelColour(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }

    public override Graphic GraphicFor(Pawn pawn)
    {
        var apparelMultiColor = (BodyDecorativeApparelMultiColor)apparel;

        string maskPath = null; 
        if (apparelMultiColor.MaskDef != null && apparelMultiColor.MaskDef.maskExtraFlags.Contains("HasShoulder"))
        {
            maskPath = apparelMultiColor.MaskDef?.maskPath;

            if (maskPath != null)
            {
                maskPath += "_Shoulder";
            }
        }
        
        var shader = Core40kDefOf.BEWH_CutoutThreeColor.Shader;
        
        return MultiColorUtils.GetGraphic<Graphic_Multi>(Props.texPath, shader, Props.drawSize, apparelMultiColor.DrawColor, apparelMultiColor.DrawColorTwo, apparelMultiColor.DrawColorThree, apparelMultiColor.Graphic.data, maskPath);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}