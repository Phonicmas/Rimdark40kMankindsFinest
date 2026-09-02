using UnityEngine;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderRankIcon : PawnRenderNode_AttachmentShoulderIcon
{
    public PawnRenderNode_AttachmentShoulderRankIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }

    protected override bool Flipped => ChapterDecoComp?.FlipShoulderIcons == true;

    protected override void ResolveIcon(Pawn pawn, out string texPath, out Color drawColour)
    {
        texPath = Props.texPath;
        drawColour = Color.white;

        var chapterDecoComp = ChapterDecoComp;

        if (chapterDecoComp == null)
        {
            return;
        }

        drawColour = chapterDecoComp.RightShoulderIconColour;

        if (chapterDecoComp.RightShoulderIcon != null)
        {
            texPath = chapterDecoComp.RightShoulderIcon.drawnTextureIconPath;
        }
    }
}