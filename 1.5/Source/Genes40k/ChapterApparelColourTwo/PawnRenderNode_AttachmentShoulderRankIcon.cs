using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode
{
    
    private bool flipped = false;
    protected override bool FlipGraphic => flipped;
    
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
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
            flipped = true;
        }
            
        return GraphicDatabase.Get<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour, drawColour);
    }
}