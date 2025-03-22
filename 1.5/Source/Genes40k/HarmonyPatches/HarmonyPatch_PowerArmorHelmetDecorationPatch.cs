using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(PawnRenderTree), "ProcessApparel")]
    public class PowerArmorHelmetDecorationPatch
    {
        public static void Postfix(Apparel ap, Dictionary<PawnRenderNodeTagDef, PawnRenderNode> ___nodesByTag, Dictionary<PawnRenderNodeTagDef, List<PawnRenderNode>> ___tmpChildTagNodes, PawnRenderTree __instance, Pawn ___pawn)
        {
            if (ap is not HelmetChapterApparelColourTwo chapterApparel)
            {
                return;
            }

            var defaultLayerValue = 100f;
            
            foreach (var decoration in chapterApparel.ExtraDecorationDefs)
            {
                var defaultLayer = new DrawData.RotationalData
                {
                    layer = defaultLayerValue + decoration.Key.LayerOrder,
                };
                
                var eastLayer = new DrawData.RotationalData
                {
                    layer = defaultLayerValue + decoration.Key.LayerOrder,
                    rotation = Rot4.East,
                    flip = decoration.Value,
                };
                
                var westLayer = new DrawData.RotationalData
                {
                    layer = defaultLayerValue + decoration.Key.LayerOrder,
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
                    workerClass = typeof(PawnRenderNodeWorker_AttachmentExtraDecorationHelmet),
                    texPath = decoration.Key.drawnTextureIconPath,
                    shaderTypeDef = decoration.Key.shaderType,                                                                      
                    parentTagDef = PawnRenderNodeTagDefOf.Head,
                    drawData = DrawData.NewWithData(rotationalData),
                    flipGraphic = decoration.Value,
                    color = decoration.Key.defaultColour,
                };
                
                var pawnRenderNode = (PawnRenderNode_AttachmentExtraDecoration)Activator.CreateInstance(typeof(PawnRenderNode_AttachmentExtraDecoration), ___pawn, pawnRenderNodeProperty, __instance);
                pawnRenderNode.Props.parentTagDef = PawnRenderNodeTagDefOf.Head;
                
                AddChild(pawnRenderNode, null, ___nodesByTag, ___tmpChildTagNodes, __instance.rootNode);
            }
        }
        
        private static void AddChild(PawnRenderNode child, PawnRenderNode parent, Dictionary<PawnRenderNodeTagDef, PawnRenderNode> nodesByTag, Dictionary<PawnRenderNodeTagDef, List<PawnRenderNode>> tmpChildTagNodes, PawnRenderNode rootNode)
        {
            if (parent == null)
            {
                parent = child.Props.parentTagDef == null || !nodesByTag.TryGetValue(child.Props.parentTagDef, out var value) ? rootNode : value;
            }
            if (parent.Props.tagDef != null)
            {
                if (tmpChildTagNodes.TryGetValue(parent.Props.tagDef, out var value2))
                {
                    value2.Add(child);
                }
                else
                {
                    tmpChildTagNodes.Add(parent.Props.tagDef, new List<PawnRenderNode> { child });
                }
            }
            child.parent = parent;
        }
    }
}