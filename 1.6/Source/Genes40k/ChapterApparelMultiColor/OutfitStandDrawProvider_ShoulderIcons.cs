using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

/// <summary>
/// Draws the left chapter icon and the right rank icon on an outfit stand, mirroring what
/// PawnRenderNode_AttachmentShoulderChapterIcon and PawnRenderNode_AttachmentShoulderRankIcon do on
/// a pawn.
/// </summary>
public class OutfitStandDrawProvider_ShoulderIcons : OutfitStandDrawProvider
{
    public override int Order => 20;

    private Genes40kModSettings modSettings = null;

    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    public override void CollectDraws(OutfitStandDrawContext context)
    {
        var apparel = context.Apparel;

        var chapterComp = apparel.GetComp<CompChapterColorWithShoulderDecoration>();
        if (chapterComp == null)
        {
            return;
        }

        var nodeProperties = apparel.def.apparel?.RenderNodeProperties;
        if (nodeProperties.NullOrEmpty())
        {
            return;
        }

        var flippedData = apparel.def.GetModExtension<DefModExtension_ShoulderFlippedData>();

        foreach (var props in nodeProperties)
        {
            if (typeof(PawnRenderNodeWorker_AttachmentShoulderChapterIcon).IsAssignableFrom(props.workerClass))
            {
                AddIcon(context, props, chapterComp, flippedData, true);
            }
            else if (typeof(PawnRenderNodeWorker_AttachmentShoulderRankIcon).IsAssignableFrom(props.workerClass))
            {
                AddIcon(context, props, chapterComp, flippedData, false);
            }
        }
    }

    private void AddIcon(OutfitStandDrawContext context, PawnRenderNodeProperties props, CompChapterColorWithShoulderDecoration chapterComp, DefModExtension_ShoulderFlippedData flippedData, bool leftShoulder)
    {
        var icon = leftShoulder ? chapterComp.LeftShoulderIcon : chapterComp.RightShoulderIcon;

        if (icon == Genes40kDefOf.BEWH_ShoulderNone)
        {
            return;
        }

        //The chapter icon worker also refuses to draw without a chapter preset; the stand has no
        //faction, so the item's own icon is the only source here.
        if (leftShoulder && (icon == null || ModSettings?.CurrentlySelectedPreset?.relatedChapterIcon == null))
        {
            return;
        }

        var texPath = icon?.drawnTextureIconPath ?? props.texPath;
        if (texPath.NullOrEmpty())
        {
            return;
        }

        var colour = leftShoulder ? chapterComp.LeftShoulderIconColour : chapterComp.RightShoulderIconColour;
        var shader = props.shaderTypeDef?.Shader ?? ShaderDatabase.Cutout;

        //Built through MultiColorUtils so the framework recognises it as one of its own and leaves
        //the icon colour alone instead of repainting it in the armour colours.
        var graphic = MultiColorUtils.GetGraphic<Graphic_Multi>(texPath, shader, props.drawSize, colour, colour, colour, null);
        if (graphic == null)
        {
            return;
        }

        var flipIcons = chapterComp.FlipShoulderIcons;

        for (var i = 0; i < 4; i++)
        {
            var rot = new Rot4(i);

            if (rot == Rot4.East && (leftShoulder ? !flipIcons : flipIcons))
            {
                continue;
            }
            if (rot == Rot4.West && (leftShoulder ? flipIcons : !flipIcons))
            {
                continue;
            }

            Vector3? offsetOverride = null;
            if (flipIcons && flippedData != null)
            {
                var offsets = leftShoulder ? flippedData.offsetForChapterWhenFacing : flippedData.offsetForRankWhenFacing;
                if (offsets != null && offsets.TryGetValue(rot, out var offset))
                {
                    offsetOverride = offset;
                }
            }

            //Facing north or south the node uses its own flip - true for the chapter icon, false for
            //the rank icon - inverted while the icons are swapped.
            bool? flipOverride = rot.IsHorizontal ? null : (bool?)(leftShoulder ? !flipIcons : flipIcons);

            AddNodeDraw(context, props, graphic, rot, offsetOverride, flipOverride);
        }
    }
}
