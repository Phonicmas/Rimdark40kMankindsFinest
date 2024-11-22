using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_SangprimusPortum : Building_Storage
    {
        private List<Thing> innerContainerChapter = new List<Thing>();
        private List<Thing> innerContainerPrimarch = new List<Thing>();
        public List<Thing> SearchableContentsChapter => innerContainerChapter;
        public List<Thing> SearchableContentsPrimarch => innerContainerPrimarch;

        public List<Thing> SearchableContents
        {
            get
            {
                var result = new List<Thing>();
                result.AddRange(innerContainerPrimarch);
                result.AddRange(innerContainerChapter);
                return result;
            }
        }

        private List<ThingDef> allChapterMaterials;
        private List<ThingDef> allPrimarchMaterials;

        private SortedList<int, (ThingDef chapter, ThingDef primarch)> allMaterialsPaired;
        public SortedList<int, (ThingDef chapter, ThingDef primarch)> AllMaterialsPaired
        {
            get
            {
                if (allChapterMaterials == null)
                {
                    UpdateMaterialList();
                }

                return allMaterialsPaired;
            }
        }
        
        public Building_SangprimusPortum()
        {
            UpdateMaterialList();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            DropAllItem();
            base.DeSpawn(mode);
        }

        private void DropAllItem()
        {
            for (var i = innerContainerChapter.Count - 1; i >= 0; i--)
            {
                var t = ThingMaker.MakeThing(innerContainerChapter[i].def);
                if (GenPlace.TryPlaceThing(t, Position, Map, ThingPlaceMode.Near))
                {
                    continue;
                }
                Log.Error("Could not drop item near " + Position);
            }
            
            for (var i = innerContainerPrimarch.Count - 1; i >= 0; i--)
            {
                var t = ThingMaker.MakeThing(innerContainerPrimarch[i].def);
                if (GenPlace.TryPlaceThing(t, Position, Map, ThingPlaceMode.Near))
                {
                    Log.Message(t + " - " + t.Position + " - " + t.Spawned);
                    continue;
                }
                Log.Error("Could not drop item near " + Position);
            }
        }

        private void UpdateMaterialList()
        {
            var tempList = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.HasModExtension<DefModExtension_PrimarchMaterial>() || thingDef.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
            
            allPrimarchMaterials = tempList.Where(thingDef => thingDef.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();
            allChapterMaterials = tempList.Where(thingDef => thingDef.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
            
            allMaterialsPaired = new SortedList<int, (ThingDef, ThingDef)>();
            
            foreach (var chapterMaterial in allChapterMaterials)
            {
                var orderInt = chapterMaterial.GetModExtension<DefModExtension_ChapterMaterial>().orderInt;
                if (AllMaterialsPaired.ContainsKey(orderInt))
                {
                    continue;
                }
                var primarchMaterial = allPrimarchMaterials.Find(g => g.GetModExtension<DefModExtension_PrimarchMaterial>().orderInt == orderInt);
                AllMaterialsPaired.Add(orderInt, (chapterMaterial, primarchMaterial));
            }
            
            foreach (var primarchMaterial in allPrimarchMaterials)
            {
                var orderInt = primarchMaterial.GetModExtension<DefModExtension_PrimarchMaterial>().orderInt;
                if (AllMaterialsPaired.ContainsKey(orderInt))
                {
                    continue;
                }
                var chapterMaterial = allChapterMaterials.Find(g => g.GetModExtension<DefModExtension_ChapterMaterial>().orderInt == orderInt);
                AllMaterialsPaired.Add(orderInt, (chapterMaterial, primarchMaterial));
            }
        }
        
        public override void Notify_ReceivedThing(Thing newItem)
        {
            if (newItem.def.HasModExtension<DefModExtension_PrimarchMaterial>())
            {
                innerContainerPrimarch.Add(newItem);
            }
            else
            {
                innerContainerChapter.Add(newItem);
            }
            
            base.Notify_ReceivedThing(newItem);
            newItem.DeSpawn();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref innerContainerChapter, "innerContainerChapter", LookMode.Deep);
            Scribe_Collections.Look(ref innerContainerPrimarch, "innerContainerPrimarch", LookMode.Deep);
        }
    }
}