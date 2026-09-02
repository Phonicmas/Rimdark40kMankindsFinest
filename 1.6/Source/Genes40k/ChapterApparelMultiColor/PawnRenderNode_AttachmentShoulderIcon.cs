using System.Collections.Generic;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

/// <summary>
/// Shared graphic resolution for the two shoulder-pad icon nodes.
/// </summary>
public abstract class PawnRenderNode_AttachmentShoulderIcon : PawnRenderNode_Apparel
{
    protected PawnRenderNode_AttachmentShoulderIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
    {
    }

    private CompChapterColorWithShoulderDecoration cachedChapterDecoComp;

    protected CompChapterColorWithShoulderDecoration ChapterDecoComp => cachedChapterDecoComp ??= apparel?.GetComp<CompChapterColorWithShoulderDecoration>();

    /// <summary>
    /// Whether this pad is drawn flipped while facing north or south. The two pads disagree on the
    /// polarity AND on what to do when the comp is missing, so this cannot be a shared expression.
    /// </summary>
    protected abstract bool Flipped { get; }

    protected abstract void ResolveIcon(Pawn pawn, out string texPath, out Color drawColour);

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
        ResolveIcon(pawn, out var texPath, out var drawColour);

        //Built through MultiColorUtils so the framework recognises it as one of its own and leaves the
        //icon colour alone instead of repainting it in the armour colours.
        return MultiColorUtils.GetGraphic<Graphic_Multi>(texPath, ShaderFor(pawn), Props.drawSize, drawColour, drawColour, drawColour, null);
    }

    protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
    {
        yield return GraphicFor(pawn);
    }
}