using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentBackpack : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentBackpack(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
    {
    }
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var backpackPath = Props.texPath;
            
        var apparelColourTwo = (BodyDecorativeApparelColourTwo)apparel;
            
        if (ModsConfig.RoyaltyActive && pawn.apparel.WornApparel.Any(wornApparel => wornApparel.def == Genes40kDefOf.Apparel_PackJump) && apparelColourTwo.def.HasModExtension<DefModExtension_HideJumpPack>() && apparelColourTwo.def.GetModExtension<DefModExtension_HideJumpPack>().changeBackpackVisual)
        {
            backpackPath += "_Jump";
        }
            
        return GraphicDatabase.Get<Graphic_Multi>(backpackPath, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}