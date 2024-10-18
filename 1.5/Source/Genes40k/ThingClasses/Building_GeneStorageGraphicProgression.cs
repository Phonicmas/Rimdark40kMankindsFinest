using RimWorld;
using System.Linq;
using System.Runtime;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Building_GeneStorageGraphicProgression : Building_GeneStorage
    {
        public Building_GeneStorageGraphicProgression()
        {
        }

        [Unsaved(false)]
        private DefModExtension_GeneStorageGraphicProgression cachedDefMod;

        private DefModExtension_GeneStorageGraphicProgression DefMod
        {
            get
            {
                if (cachedDefMod == null)
                {
                    cachedDefMod = def.GetModExtension<DefModExtension_GeneStorageGraphicProgression>();
                }
                return cachedDefMod;
            }
        }

        [Unsaved(false)]
        private Graphic cachedHalfFullGraphic;

        private Graphic HalfFullGraphic
        {
            get
            {
                if (cachedHalfFullGraphic == null)
                {
                    cachedHalfFullGraphic = GraphicDatabase.Get<Graphic_Multi>(DefMod.halfFullGraphic, ShaderDatabase.DefaultShader, def.graphicData.drawSize, Color.white, Color.white, DefaultGraphic.data);
                }
                return cachedHalfFullGraphic;
            }
        }

        [Unsaved(false)]
        private Graphic cachedFullGraphic;

        private Graphic FullGraphic
        {
            get
            {
                if (cachedFullGraphic == null)
                {
                    cachedFullGraphic = GraphicDatabase.Get<Graphic_Multi>(DefMod.fullGraphic, ShaderDatabase.DefaultShader, def.graphicData.drawSize, Color.white, Color.white, DefaultGraphic.data);
                    cachedFullGraphic.data = DefaultGraphic.data;
                }
                return cachedFullGraphic;
            }
        }

        public override Graphic Graphic
        {
            get
            {
                if (DefMod.halfFullGraphic.NullOrEmpty())
                {
                    if (GeneAmount.Count() == MaximumItems)
                    {
                        return FullGraphic;
                    }
                    else
                    {
                        return DefaultGraphic;
                    }
                }
                else
                {
                    float filledPercent = (float)GeneAmount.Count() / MaximumItems;
                    if (filledPercent < 0.5f)
                    {
                        return DefaultGraphic;
                    }
                    else if (filledPercent < 1)
                    {
                        return HalfFullGraphic;
                    }
                    else
                    {
                        return FullGraphic;
                    }
                }
            }
        }
    }
}