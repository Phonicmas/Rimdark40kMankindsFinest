using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PawnRenderNodeWorker_AttachmentShoulderPad : PawnRenderNodeWorker_AttachmentBody
    {
        private Graphic cachedShoulderGraphic = null;

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            if (parms.pawn.Rotation == Rot4.North || parms.pawn.Rotation == Rot4.South)
            {
                return Vector3.zero;
            }
            return base.ScaleFor(node, parms);
        }

        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            var def = node.apparel.def;
            var apparelColourTwo = (ApparelColourTwo)node.apparel;
            
            const string shoulderPath = "Things/Armor/Imperium/PowerArmor/Shoulder/BEWH_ImperiumPowerArmor_Shoulder";
            var shader = ShaderDatabase.CutoutComplex;
                    
            if (def.graphicData.shaderType != null)
            {
                shader = def.graphicData.shaderType.Shader;
            }
            cachedShoulderGraphic = GraphicDatabase.Get<Graphic_Multi>(shoulderPath, shader, def.graphicData.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo, def.graphicData);
            
            return cachedShoulderGraphic;
        }
    }
}