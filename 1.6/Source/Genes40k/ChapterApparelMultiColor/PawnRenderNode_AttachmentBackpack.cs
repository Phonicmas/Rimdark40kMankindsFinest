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
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var apparelMultiColor = (BodyDecorativeApparelMultiColor)apparel;

        var texPath = Props.texPath;
        string maskPath = null;

        var jumpPackVisual = ModsConfig.RoyaltyActive 
                             && pawn.apparel.WornApparel.Any(wornApparel => wornApparel.def == Genes40kDefOf.Apparel_PackJump) 
                             && apparelMultiColor.def.HasModExtension<DefModExtension_HideJumpPack>() 
                             && apparelMultiColor.def.GetModExtension<DefModExtension_HideJumpPack>().changeBackpackVisual;

        if (jumpPackVisual)
        {
            texPath += "_Jump";
        }

        if (apparelMultiColor.MaskDef != null)
        {
            maskPath = apparelMultiColor.MaskDef.maskPath;
            
            var backpackMask = apparelMultiColor.MaskDef.maskExtraFlags.Contains("HasBackpack");
            var jumppackMask = apparelMultiColor.MaskDef.maskExtraFlags.Contains("HasJumppack");
            
            if (jumpPackVisual)
            {
                if (maskPath != null && jumppackMask)
                {
                    maskPath += "_Backpack_Jump";
                }
                else
                {
                    maskPath = null;
                }
            }
            else
            {
                if (maskPath != null && backpackMask)
                {
                    maskPath += "_Backpack";
                }
                else
                {
                    maskPath = null;
                }
            }
        }
        
        var shader = Core40kDefOf.BEWH_CutoutThreeColor.Shader;
        
        return MultiColorUtils.GetGraphic<Graphic_Multi>(texPath, shader, Props.drawSize, apparelMultiColor.DrawColor, apparelMultiColor.DrawColorTwo, apparelMultiColor.DrawColorThree, apparelMultiColor.def.graphicData, maskPath);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}