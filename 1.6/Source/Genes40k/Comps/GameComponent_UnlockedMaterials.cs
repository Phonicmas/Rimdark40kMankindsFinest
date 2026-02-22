using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_UnlockedMaterials : GameComponent
{
    private Genes40kModSettings modSettings = null;
    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    private List<ThingDef> allMaterials = [];
    public List<ThingDef> UnlockedChapterMaterial => allMaterials.Where(def => def.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
    public List<ThingDef> UnlockedPrimarchMaterial => allMaterials.Where(def => def.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();
    
    
    private SortedList<int, (ThingDef chapter, ThingDef primarch)> allMaterialsPaired = [];
    public SortedList<int, (ThingDef chapter, ThingDef primarch)> AllMaterialsPaired => allMaterialsPaired;

    public GameComponent_UnlockedMaterials(Game game)
    {
        SetupMaterialList();
    }

    public void UnlockMaterial(ThingDef material)
    {
        allMaterials.Add(material);
    }

    public bool HasMaterial(ThingDef material)
    {
        return allMaterials.Contains(material);
    }
    
    private void SetupMaterialList()
    {
        var chapterMaterial = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
        var primarchMaterial = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();

        var res = chapterMaterial.Count > primarchMaterial.Count 
            ? ZipMaterials(chapterMaterial, primarchMaterial) 
            : ZipMaterials(primarchMaterial, chapterMaterial);

        allMaterials ??= [];
        allMaterialsPaired = res;
    }
    private SortedList<int, (ThingDef chapter, ThingDef primarch)> ZipMaterials(List<ThingDef> longList, List<ThingDef> shortList)
    {
        var pairedList = new SortedList<int, (ThingDef, ThingDef)>();
        
        foreach (var thingDef in longList)
        {
            var orderInt = thingDef.GetModExtension<DefModExtension_BaseMaterial>().orderInt;
            if (pairedList.ContainsKey(orderInt))
            {
                continue;
            }
            var shortListDef = shortList.Where(sDef => sDef.GetModExtension<DefModExtension_BaseMaterial>().orderInt == orderInt).FirstOrFallback();
            pairedList.Add(orderInt, (thingDef, shortListDef));
        }

        return pairedList;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref allMaterials, "allMaterials", LookMode.Def);
        
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            SetupMaterialList();
        }
    }
}