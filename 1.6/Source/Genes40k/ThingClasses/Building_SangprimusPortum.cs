using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_SangprimusPortum : Building, IThingHolder
{
    private ThingOwner innerContainer;
        
    public List<Thing> SearchableContentsChapter => innerContainer.Where(thing => thing.def.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
        
    public List<Thing> SearchableContentsPrimarch => innerContainer.Where(thing => thing.def.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();

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
        innerContainer = new ThingOwner<Thing>(this);
        UpdateMaterialList();
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

    public bool CanAcceptMaterial(Thing thing)
    {
        return innerContainer.All(x => x.def != thing.def);
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings() => innerContainer;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
    }
}