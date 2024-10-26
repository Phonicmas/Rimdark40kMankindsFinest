using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Building_PrimarchEmbryoStorage : Building_GeneStorage
    {
        [Unsaved(false)]
        private Graphic cachedTopGraphic;

        private Graphic TopGraphic =>
            cachedTopGraphic ?? (cachedTopGraphic = GraphicDatabase.Get<Graphic_Multi>(
                "Things/Building/PrimarchEmbryoContainer/BEWH_PrimarchEmbryoContainerTop",
                ShaderDatabase.Transparent, def.graphicData.drawSize, Color.white));

        private Graphic cachedFullTopGraphic;

        private Graphic FullTopGraphic =>
            cachedFullTopGraphic ?? (cachedFullTopGraphic =
                GraphicDatabase.Get<Graphic_Multi>(
                    "Things/Building/PrimarchEmbryoContainer/BEWH_PrimarchEmbryoContainerTopFull",
                    ShaderDatabase.Transparent, def.graphicData.drawSize, Color.white));

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (GeneAmount.Any())
            {
                FullTopGraphic.Draw(DrawPos + new Vector3(0, 2, 0), base.Rotation, this);
            }
            TopGraphic.Draw(DrawPos + new Vector3(0, 2, 0), base.Rotation, this);
        }
    }
}