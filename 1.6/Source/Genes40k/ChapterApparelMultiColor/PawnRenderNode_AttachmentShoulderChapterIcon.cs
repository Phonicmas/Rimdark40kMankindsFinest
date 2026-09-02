using UnityEngine;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderChapterIcon : PawnRenderNode_AttachmentShoulderIcon
{
    public PawnRenderNode_AttachmentShoulderChapterIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }

    private Genes40kModSettings modSettings = null;

    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    protected override bool Flipped => ChapterDecoComp?.FlipShoulderIcons != true;

    protected override void ResolveIcon(Pawn pawn, out string texPath, out Color drawColour)
    {
        texPath = Props.texPath;
        drawColour = Color.white;

        var chapterDecoComp = ChapterDecoComp;

        if (chapterDecoComp == null)
        {
            return;
        }

        drawColour = chapterDecoComp.LeftShoulderIconColour;

        if (chapterDecoComp.LeftShoulderIcon != null)
        {
            texPath = chapterDecoComp.LeftShoulderIcon.drawnTextureIconPath;
            return;
        }

        if (pawn.Faction == null || !pawn.Faction.IsPlayer || ModSettings?.CurrentlySelectedPreset?.relatedChapterIcon == null)
        {
            return;
        }

        texPath = ModSettings.CurrentlySelectedPreset.relatedChapterIcon.drawnTextureIconPath;

        if (ModSettings.chapterShoulderIconColor != null)
        {
            drawColour = ModSettings.chapterShoulderIconColor.Value;
        }
    }
}