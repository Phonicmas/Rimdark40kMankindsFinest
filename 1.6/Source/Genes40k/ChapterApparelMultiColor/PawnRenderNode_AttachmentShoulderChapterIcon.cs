using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class PawnRenderNode_AttachmentShoulderChapterIcon : PawnRenderNode_Apparel
{
    public PawnRenderNode_AttachmentShoulderChapterIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }
    
    private Genes40kModSettings modSettings = null;

    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
    
    public bool Flipped = true;
    

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
        var leftShoulderIcon = Props.texPath;
            
        var chapterDecoComp = apparel.GetComp<CompChapterColorWithShoulderDecoration>();
        
        var drawColour = chapterDecoComp.LeftShoulderIconColour;
            
        if (chapterDecoComp.LeftShoulderIcon != null)
        {
            leftShoulderIcon = chapterDecoComp.LeftShoulderIcon.drawnTextureIconPath;
        }
        else if (pawn.Faction != null && pawn.Faction.IsPlayer && ModSettings.CurrentlySelectedPreset.relatedChapterIcon != null)
        {
            leftShoulderIcon = ModSettings.CurrentlySelectedPreset.relatedChapterIcon.drawnTextureIconPath;
            if (ModSettings.chapterShoulderIconColor != null)
            {
                drawColour = ModSettings.chapterShoulderIconColor.Value;
            }
        }

        if (chapterDecoComp.FlipShoulderIcons)
        {
            Flipped = !Flipped;
        }
        
        return GraphicDatabase.Get<Graphic_Multi>(leftShoulderIcon, ShaderFor(pawn), Props.drawSize, drawColour, drawColour);
    }
    
    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}