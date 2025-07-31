using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel) : base(pawn, props, tree, apparel)
    {
    }
    
    public bool Flipped = false;
    
    public override bool FlipGraphic(PawnDrawParms parms)
    {
        if (parms.facing == Rot4.West || parms.facing == Rot4.East)
        {
            return base.FlipGraphic(parms);
        }
        return Flipped;
    }
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var rightShoulderPath = Props.texPath;
            
        var apparelMultiColor = (ChapterBodyDecorativeApparelMultiColor)apparel;
        var drawColour = apparelMultiColor.RightShoulderIconColour;
            
        if (apparelMultiColor.RightShoulderIcon != null)
        {
            rightShoulderPath = apparelMultiColor.RightShoulderIcon.drawnTextureIconPath;
        }
        
        if (apparelMultiColor.FlipShoulderIcons)
        {
            Flipped = !Flipped;
        }

        return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour, drawColour);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}