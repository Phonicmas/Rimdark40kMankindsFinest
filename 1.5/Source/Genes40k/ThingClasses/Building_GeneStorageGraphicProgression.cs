using RimWorld;
using System.Linq;
using System.Runtime;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Building_GeneStorageGraphicProgression : Building_GeneStorage
    {
        [Unsaved(false)]
        private DefModExtension_GeneStorageGraphicProgression cachedDefMod;

        private DefModExtension_GeneStorageGraphicProgression DefMod => cachedDefMod ?? (cachedDefMod = def.GetModExtension<DefModExtension_GeneStorageGraphicProgression>());

        [Unsaved(false)]
        private Graphic cachedHalfFullGraphic;

        private Graphic HalfFullGraphic =>
            cachedHalfFullGraphic ?? (cachedHalfFullGraphic =
                GraphicDatabase.Get<Graphic_Multi>(DefMod.halfFullGraphic, ShaderDatabase.DefaultShader,
                    def.graphicData.drawSize, Color.white, Color.white, DefaultGraphic.data));

        [Unsaved(false)]
        private Graphic cachedFullGraphic;

        private Graphic FullGraphic =>
            cachedFullGraphic ?? (cachedFullGraphic = GraphicDatabase.Get<Graphic_Multi>(DefMod.fullGraphic,
                ShaderDatabase.DefaultShader, def.graphicData.drawSize, Color.white, Color.white,
                DefaultGraphic.data));

        public override Graphic Graphic
        {
            get
            {
                if (DefMod.halfFullGraphic.NullOrEmpty())
                {
                    return GeneAmount.Count() == MaximumItems ? FullGraphic : DefaultGraphic;
                }
                
                var filledPercent = (float)GeneAmount.Count() / MaximumItems;
                if (filledPercent < 0.5f)
                {
                    return DefaultGraphic;
                }
                return filledPercent < 1 ? HalfFullGraphic : FullGraphic;
            }
        }
    }
}