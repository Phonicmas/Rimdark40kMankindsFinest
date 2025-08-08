using System.Collections.Generic;
using System.Linq;
using Core40k;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Building_OutfitStand), "RecacheGraphics")]
public static class OutfitStandShoulder
{
    public static void Postfix(Building_OutfitStand __instance, ThingOwner<Thing> ___innerContainer, List<CachedGraphicRenderInfo> ___cachedApparelGraphicsNonHeadgear)
    {
        var list = ___innerContainer.InnerListForReading.OfType<Apparel>().ToList();
        foreach (var item in list)
        {
            if (item is not ChapterBodyDecorativeApparelMultiColor chapterApparel)
            {
                continue;
            }

            foreach (var pawnRenderNode in chapterApparel.def.apparel.RenderNodeProperties)
            {
                if (pawnRenderNode.texPath.Contains("Shoulder"))
                {
                    var maskPath = chapterApparel.MaskDef?.maskPath;
                    if (maskPath != null && chapterApparel.MaskDef.maskExtraFlags.Contains("HasShoulder"))
                    {
                        maskPath += "Shoulder";
                    }
                    else
                    {
                        maskPath = null;
                    }
                    var shader = Core40kDefOf.BEWH_CutoutThreeColor.Shader;
                    var graphic = MultiColorUtils.GetGraphic<Graphic_Multi>(pawnRenderNode.texPath, shader, pawnRenderNode.drawSize, chapterApparel.DrawColor, chapterApparel.DrawColorTwo, chapterApparel.DrawColorThree, chapterApparel.def.graphicData, maskPath);
                    var layer = (int)pawnRenderNode.drawData.LayerForRot(__instance.Rotation, (int)pawnRenderNode.baseLayer);
                    var vector = Vector3.zero;
                    if (__instance.Rotation == Rot4.West || __instance.Rotation == Rot4.East)
                    {
                        vector.y += 2;
                    }
                    var cachedGraphicRenderInfo = new CachedGraphicRenderInfo(graphic, layer, Vector3.one, vector);
                    ___cachedApparelGraphicsNonHeadgear.Add(cachedGraphicRenderInfo);
                }
            }
            
        }
    }
    
    private static Vector3 HeadOffsetAt(Rot4 rotation, BodyTypeDef BodyTypeDefForRendering)
    {
        var headOffset = BodyTypeDefForRendering.headOffset;
        return rotation.AsInt switch
        {
            0 => new Vector3(0f, 0f, headOffset.y), 
            1 => new Vector3(headOffset.x, 0f, headOffset.y), 
            2 => new Vector3(0f, 0f, headOffset.y), 
            3 => new Vector3(0f - headOffset.x, 0f, headOffset.y), 
            _ => Vector3.zero, 
        };
    }
}

