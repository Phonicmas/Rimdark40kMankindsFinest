using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

/// <summary>
/// Shared draw rules for the two shoulder-pad icon nodes. The only thing the two sides disagree on
/// is which pad they are, which side they hide on while facing east or west, and which offset table
/// they read.
/// </summary>
public abstract class PawnRenderNodeWorker_AttachmentShoulderIcon : PawnRenderNodeWorker
{
    protected abstract bool IsLeftShoulder { get; }

    private ShoulderIconDef IconFrom(CompChapterColorWithShoulderDecoration chapterDecoComp)
    {
        return IsLeftShoulder ? chapterDecoComp.LeftShoulderIcon : chapterDecoComp.RightShoulderIcon;
    }

    private Dictionary<Rot4, Vector3> OffsetsFrom(DefModExtension_ShoulderFlippedData flippedData)
    {
        return IsLeftShoulder ? flippedData.offsetForChapterWhenFacing : flippedData.offsetForRankWhenFacing;
    }

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var pawn = parms.pawn;

        var chapterDecoComp = node.apparel?.GetComp<CompChapterColorWithShoulderDecoration>();

        if (chapterDecoComp == null)
        {
            return false;
        }

        var icon = IconFrom(chapterDecoComp);

        if (icon == Genes40kDefOf.BEWH_ShoulderNone || icon == null)
        {
            return false;
        }

        //Each pad hides on the side of the body it has rotated behind, and the two sides swap while
        //the icons are flipped.
        if (parms.facing == Rot4.East)
        {
            return IsLeftShoulder ? chapterDecoComp.FlipShoulderIcons : !chapterDecoComp.FlipShoulderIcons;
        }

        if (parms.facing == Rot4.West)
        {
            return IsLeftShoulder ? !chapterDecoComp.FlipShoulderIcons : chapterDecoComp.FlipShoulderIcons;
        }

        if (parms.Portrait)
        {
            if ((parms.flags & PawnRenderFlags.Clothes) != PawnRenderFlags.Clothes)
            {
                return false;
            }
        }
        else
        {
            switch (parms.posture)
            {
                case PawnPosture.LayingOnGroundNormal:
                case PawnPosture.LayingOnGroundFaceUp:
                case PawnPosture.Standing:
                    return true;
            }

            var mindState = pawn.mindState;
            if (mindState != null && mindState.duty?.def?.drawBodyOverride.HasValue == true)
            {
                return pawn.mindState.duty.def.drawBodyOverride.Value;
            }
            if (parms.bed != null && parms.pawn.RaceProps.Humanlike)
            {
                return parms.bed.def.building.bed_showSleeperBody;
            }
        }

        return true;
    }

    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        var res = base.OffsetFor(node, parms, out pivot);

        var chapterDecoComp = node.apparel?.GetComp<CompChapterColorWithShoulderDecoration>();

        if (chapterDecoComp?.FlipShoulderIcons != true)
        {
            return res;
        }

        var flippedData = node.apparel.def.GetModExtension<DefModExtension_ShoulderFlippedData>();

        if (flippedData != null && OffsetsFrom(flippedData).TryGetValue(parms.facing, out var offset))
        {
            return offset;
        }

        return res;
    }
}