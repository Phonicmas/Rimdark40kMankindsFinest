using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode
{
    
    public bool Flipped = false;
    //protected override bool FlipGraphic => Flipped;
    
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
    
    public override bool FlipGraphic(PawnDrawParms parms)
    {
        return Flipped;
    }
        
    public override Graphic GraphicFor(Pawn pawn)
    {
        var rightShoulderPath = Props.texPath;
            
        var apparelColourTwo = (ChapterBodyDecorativeApparelColourTwo)apparel;
        var drawColour = apparelColourTwo.RightShoulderIconColour;
            
        if (apparelColourTwo.RightShoulderIcon != null)
        {
            rightShoulderPath = apparelColourTwo.RightShoulderIcon.drawnTextureIconPath;
        }
        
        if (apparelColourTwo.FlipShoulderIcons)
        {
            Flipped = !Flipped;
        }

        return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour, drawColour);
    }
}