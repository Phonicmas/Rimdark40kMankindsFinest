using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_GeneticStorage : Building_Storage
    {
        [Unsaved]
        private DefModExtension_GeneStorageGraphicProgression cachedDefMod;

        private DefModExtension_GeneStorageGraphicProgression DefMod => cachedDefMod ?? (cachedDefMod = def.GetModExtension<DefModExtension_GeneStorageGraphicProgression>());
        
        [Unsaved]
        private Graphic cachedHalfFullGraphic;

        private Graphic HalfFullGraphic =>
            cachedHalfFullGraphic ?? (cachedHalfFullGraphic =
                GraphicDatabase.Get<Graphic_Multi>(DefMod.halfFullGraphic, ShaderDatabase.DefaultShader,
                    def.graphicData.drawSize, Color.white, Color.white, DefaultGraphic.data));

        [Unsaved]
        private Graphic cachedFullGraphic;

        private Graphic FullGraphic =>
            cachedFullGraphic ?? (cachedFullGraphic = GraphicDatabase.Get<Graphic_Multi>(DefMod.fullGraphic,
                ShaderDatabase.DefaultShader, def.graphicData.drawSize, Color.white, Color.white,
                DefaultGraphic.data));
        
        public override Graphic Graphic
        {
            get
            {
                var storedAmount = slotGroup.HeldThings.Count();
                var maximumItems = def.building.maxItemsInCell * AllSlotCells().Count();
                if (DefMod.halfFullGraphic.NullOrEmpty())
                {
                    return storedAmount == maximumItems ? FullGraphic : DefaultGraphic;
                }
                
                var filledPercent = (float)storedAmount / maximumItems;
                if (filledPercent < 0.5f)
                {
                    return DefaultGraphic;
                }
                return filledPercent < 1 ? HalfFullGraphic : FullGraphic;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach (var item in slotGroup.HeldThings)
            {
                UnhideItem(item);
            }
            base.Destroy(mode);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach (var item in slotGroup.HeldThings)
            {
                UnhideItem(item);
            }
            base.DeSpawn(mode);
        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            foreach (var item in slotGroup.HeldThings)
            {
                UnhideItem(item);
            }
            base.Kill(dinfo, exactCulprit);
        }

        public override void Notify_ReceivedThing(Thing newItem)
        {
            switch (newItem)
            {
                case GeneseedVial geneseedVial:
                    geneseedVial.ChangeVisibility(true);
                    break;
                case GeneticMatrix geneticMatrix:
                    geneticMatrix.ChangeVisibility(true);
                    break;
                case PrimarchEmbryo primarchEmbryo:
                    primarchEmbryo.ChangeVisibility(true);
                    break;
            }
            base.Notify_ReceivedThing(newItem);
        }

        public override void Notify_LostThing(Thing newItem)
        {
            UnhideItem(newItem);
            base.Notify_ReceivedThing(newItem);
        }

        private void UnhideItem(Thing item)
        {
            switch (item)
            {
                case GeneseedVial geneseedVial:
                    geneseedVial.ChangeVisibility(false);
                    break;
                case GeneticMatrix geneticMatrix:
                    geneticMatrix.ChangeVisibility(false);
                    break;
                case PrimarchEmbryo primarchEmbryo:
                    primarchEmbryo.ChangeVisibility(false);
                    break;
            }
        }
        
        public override void DrawGUIOverlay()
        {
            if (def.HasModExtension<DefModExtension_SangprimusPortum>())
            {
                return;
            }
            
            var storedAmount = slotGroup.HeldThings.Count();
            var maximumItems = def.building.maxItemsInCell * AllSlotCells().Count();
                
            GenMapUI.DrawThingLabel(this, storedAmount + "/" + maximumItems);
        }
    }
}