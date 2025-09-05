using System.Collections.Generic;
using System.Linq;
using Core40k;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Building_OutfitStand), "RecacheGraphics")]
public static class OutfitStandBackpack
{
    public static void Postfix(Building_OutfitStand __instance, ThingOwner<Thing> ___innerContainer, List<CachedGraphicRenderInfo> ___cachedApparelGraphicsNonHeadgear)
    {
        var list = ___innerContainer.InnerListForReading.OfType<Apparel>().ToList();
        foreach (var item in list)
        {
            if (!item.HasComp<CompChapterColorWithShoulderDecoration>())
            {
                continue;
            }
            
            var chapterColorComp = item.GetComp<CompChapterColorWithShoulderDecoration>();
            
            foreach (var pawnRenderNode in item.def.apparel.RenderNodeProperties)
            {
                if (pawnRenderNode.texPath.Contains("_Backpack"))
                {
                    var maskPath = chapterColorComp.MaskDef?.maskPath;
                    if (maskPath != null && chapterColorComp.MaskDef.maskExtraFlags.Contains("HasBackpack"))
                    {
                        maskPath += "_Backpack";
                    }
                    else
                    {
                        maskPath = null;
                    }
                    var shader = Core40kDefOf.BEWH_CutoutThreeColor.Shader;
                    var graphic = MultiColorUtils.GetGraphic<Graphic_Multi>(pawnRenderNode.texPath, shader, pawnRenderNode.drawSize, chapterColorComp.DrawColor, chapterColorComp.DrawColorTwo, chapterColorComp.DrawColorThree, item.def.graphicData, maskPath);
                    var layer = (int)pawnRenderNode.drawData.LayerForRot(__instance.Rotation, (int)pawnRenderNode.baseLayer);
                    var vector = Vector3.zero;
                    var vectorOffset = BodyTypeDefOf.Hulk.headOffset.y;
                    if (__instance.Rotation == Rot4.North)
                    {
                        vector.y = vectorOffset;
                    }
                    if (__instance.Rotation == Rot4.West || __instance.Rotation == Rot4.East)
                    {
                        vector.y = vectorOffset;
                    }
                    var cachedGraphicRenderInfo = new CachedGraphicRenderInfo(graphic, layer, Vector3.one, vector);
                    ___cachedApparelGraphicsNonHeadgear.Add(cachedGraphicRenderInfo);
                }
            }
            
        }
    }
}

