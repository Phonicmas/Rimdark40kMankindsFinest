using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ExtraDecorationComp : ThingComp
{
    public override List<PawnRenderNode> CompRenderNodes()
    {
        if (parent is not Pawn pawn)
        {
            return null;
        }

        var apparelTwo = (ExtraIconsChapterApparelColourTwo)pawn.apparel.WornApparel.FirstOrDefault(wornApparel => wornApparel is ExtraIconsChapterApparelColourTwo);
        
        if (apparelTwo == null)
        {
            return null;
        }
        
        var list = new List<PawnRenderNode>();
        
        foreach (var decoration in apparelTwo.ExtraDecorationDefs)
        {
            var defaultLayer = new DrawData.RotationalData
            {
                layer = 76 + decoration.Key.LayerOrder,
            };
            
            var eastLayer = new DrawData.RotationalData
            {
                layer = 76 + decoration.Key.LayerOrder,
                rotation = Rot4.East,
                flip = decoration.Value,
            };
            
            var westLayer = new DrawData.RotationalData
            {
                layer = 76 + decoration.Key.LayerOrder,
                rotation = Rot4.West,
                flip = decoration.Value,
            };
            
            var rotationalData = new DrawData.RotationalData[3];
            rotationalData[0] = defaultLayer;
            rotationalData[1] = eastLayer;
            rotationalData[2] = westLayer;
            
            var pawnRenderNodeProperty = new PawnRenderNodeProperties
            {
                nodeClass = typeof(PawnRenderNode_AttachmentExtraDecoration),
                workerClass = typeof(PawnRenderNodeWorker_AttachmentExtraDecoration),
                texPath = decoration.Key.drawnTextureIconPath,
                shaderTypeDef = decoration.Key.shaderType,                                                                      
                parentTagDef = PawnRenderNodeTagDefOf.Body,
                drawData = DrawData.NewWithData(rotationalData),
                flipGraphic = decoration.Value,
                color = decoration.Key.defaultColor,
            };
            
            var pawnRenderNode = (PawnRenderNode_AttachmentExtraDecoration)Activator.CreateInstance(typeof(PawnRenderNode_AttachmentExtraDecoration), pawn, pawnRenderNodeProperty, pawn.Drawer.renderer.renderTree);
            
            list.Add(pawnRenderNode);
        }
        
        return list;
    }
}