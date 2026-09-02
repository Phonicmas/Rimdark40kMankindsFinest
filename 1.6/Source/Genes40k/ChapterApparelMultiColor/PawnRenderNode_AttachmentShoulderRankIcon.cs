using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
    
    private CompChapterColorWithShoulderDecoration cachedChapterDecoComp;
    private CompChapterColorWithShoulderDecoration ChapterDecoComp => cachedChapterDecoComp ??= apparel?.GetComp<CompChapterColorWithShoulderDecoration>();

    private bool Flipped => ChapterDecoComp?.FlipShoulderIcons == true;
    
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
            
        var chapterDecoComp = ChapterDecoComp;
        
        var drawColour = chapterDecoComp.RightShoulderIconColour;
            
        if (chapterDecoComp.RightShoulderIcon != null)
        {
            rightShoulderPath = chapterDecoComp.RightShoulderIcon.drawnTextureIconPath;
        }
        
        //Built through MultiColorUtils so the framework recognises it as one of its own and leaves the
        //icon colour alone instead of repainting it in the armour colours.
        return MultiColorUtils.GetGraphic<Graphic_Multi>(rightShoulderPath, ShaderFor(pawn), Props.drawSize, drawColour, drawColour, drawColour, null);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}