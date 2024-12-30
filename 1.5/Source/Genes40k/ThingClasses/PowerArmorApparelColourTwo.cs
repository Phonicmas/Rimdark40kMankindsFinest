using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PowerArmorApparelColourTwo : ChapterApparelColourTwo
    {
        private Graphic cachedShoulderGraphic = null;

        public Graphic ShoulderGraphic
        {
            get
            {
                if (cachedShoulderGraphic == null)
                {
                    const string shoulderPath = "Things/Armor/Imperium/PowerArmor/Shoulder/BEWH_ImperiumPowerArmor_Shoulder";
                    var shader = ShaderDatabase.CutoutComplex;
                    if (def.graphicData.shaderType != null)
                    {
                        shader = def.graphicData.shaderType.Shader;
                    }
                    cachedShoulderGraphic = GraphicDatabase.Get<Graphic_Multi>(shoulderPath, shader, def.graphicData.drawSize, DrawColor, DrawColorTwo, def.graphicData);
                }

                return cachedShoulderGraphic;
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
        }
    }
}