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
            if (!item.HasComp<CompChapterColorWithShoulderDecoration>())
            {
                continue;
            }
            
            var chapterColorComp = item.GetComp<CompChapterColorWithShoulderDecoration>();

            foreach (var pawnRenderNode in item.def.apparel.RenderNodeProperties)
            {
                if (pawnRenderNode.texPath.Contains("Shoulder"))
                {
                    var maskPath = chapterColorComp.MaskDef?.maskPath;
                    if (maskPath != null && chapterColorComp.MaskDef.maskExtraFlags.Contains("HasShoulder"))
                    {
                        maskPath += "_Shoulder";
                    }
                    else
                    {
                        maskPath = null;
                    }
                    var shader = Core40kDefOf.BEWH_CutoutThreeColor.Shader;
                    var graphic = MultiColorUtils.GetGraphic<Graphic_Multi>(pawnRenderNode.texPath, shader, pawnRenderNode.drawSize, chapterColorComp.DrawColor, chapterColorComp.DrawColorTwo, chapterColorComp.DrawColorThree, item.def.graphicData, maskPath);
                    var layer = (int)pawnRenderNode.drawData.LayerForRot(__instance.Rotation, (int)pawnRenderNode.baseLayer);
                    var vector = Vector3.zero;
                    if (__instance.Rotation == Rot4.West || __instance.Rotation == Rot4.East)
                    {
                        var vectorOffset = BodyTypeDefOf.Hulk.headOffset.y;
                        vector.y = vectorOffset;
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

