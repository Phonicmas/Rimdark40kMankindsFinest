using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Building_PrimarchEmbryoStorage : Building_GeneStorage
    {
        public Building_PrimarchEmbryoStorage()
        {
        }

        [Unsaved(false)]
        private Graphic cachedTopGraphic;

        private Graphic TopGraphic
        {
            get
            {
                if (cachedTopGraphic == null)
                {
                    cachedTopGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Building/PrimarchEmbryoContainer/BEWH_PrimarchEmbryoContainerTop", ShaderDatabase.Transparent, def.graphicData.drawSize, Color.white);
                }
                return cachedTopGraphic;
            }
        }

        private Graphic cachedFullTopGraphic;

        private Graphic FullTopGraphic
        {
            get
            {
                if (cachedFullTopGraphic == null)
                {
                    cachedFullTopGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Building/PrimarchEmbryoContainer/BEWH_PrimarchEmbryoContainerTopFull", ShaderDatabase.Transparent, def.graphicData.drawSize, Color.white);
                }
                return cachedFullTopGraphic;
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (GeneAmount.Count() > 0)
            {
                FullTopGraphic.Draw(DrawPos + new Vector3(0, 2, 0), base.Rotation, this);
            }
            TopGraphic.Draw(DrawPos + new Vector3(0, 2, 0), base.Rotation, this);
        }
    }
}