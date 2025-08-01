using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentBackpack : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentBackpack(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
    public PawnRenderNode_AttachmentBackpack(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
    {
    }
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var apparelMultiColor = (BodyDecorativeApparelMultiColor)apparel;

        string maskPath = null;

        if (apparelMultiColor.MaskDef != null && apparelMultiColor.MaskDef.maskExtraFlags.Contains("HasBackpack"))
        {
            maskPath = apparelMultiColor.MaskDef?.maskPath;

            if (maskPath != null)
            {
                maskPath += "_Backpack";
            }
            if (ModsConfig.RoyaltyActive && pawn.apparel.WornApparel.Any(wornApparel => wornApparel.def == Genes40kDefOf.Apparel_PackJump) && apparelMultiColor.def.HasModExtension<DefModExtension_HideJumpPack>() && apparelMultiColor.def.GetModExtension<DefModExtension_HideJumpPack>().changeBackpackVisual)
            {
                maskPath += "_Jump";
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