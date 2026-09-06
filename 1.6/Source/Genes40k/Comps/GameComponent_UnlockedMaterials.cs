using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_UnlockedMaterials : GameComponent
{
    private static GameComponent_UnlockedMaterials instance;
    private static Game instanceGame;

    /// <summary>
    /// The current game's component, looked up once per game instead of scanning Game.components per call.
    /// </summary>
    public static GameComponent_UnlockedMaterials Instance
    {
        get
        {
            var game = Current.Game;

            if (game == null)
            {
                return null;
            }

            if (instance == null || instanceGame != game)
            {
                instance = game.GetComponent<GameComponent_UnlockedMaterials>();
                instanceGame = game;
            }

            return instance;
        }
    }

    private Genes40kModSettings modSettings = null;
    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    private List<ThingDef> unlockedMaterials = [];
    public List<ThingDef> UnlockedChapterMaterial => unlockedMaterials.Where(def => def.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
    public List<ThingDef> UnlockedPrimarchMaterial => unlockedMaterials.Where(def => def.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();
    
    private SortedList<int, (ThingDef chapter, ThingDef primarch)> allMaterialsPaired = [];
    public SortedList<int, (ThingDef chapter, ThingDef primarch)> AllMaterialsPaired => allMaterialsPaired;

    public GameComponent_UnlockedMaterials(Game game)
    {
        SetupMaterialList();
    }

    public void UnlockMaterial(ThingDef material)
    {
        unlockedMaterials.Add(material);
    }

    public bool HasMaterial(ThingDef material)
    {
        return unlockedMaterials.Contains(material);
    }

    private List<ThingDef> GetLockedLegionMaterials()
    {
        return AllMaterialsPaired.Values.Select(thing => thing.chapter).Where(def => !unlockedMaterials.Contains(def) && def.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
    }

    public ThingDef GetRandomLockedLegionMaterial()
    {
        return GetLockedLegionMaterials().RandomElement();
    }

    public bool AnyLockedLegionMaterialRemaining()
    {
        return GetLockedLegionMaterials().Count > 0;
    }
    
    private void SetupMaterialList()
    {
        var chapterMaterial = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.HasModExtension<DefModExtension_ChapterMaterial>()).ToList();
        var primarchMaterial = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();
        
        var res = chapterMaterial.Count >= primarchMaterial.Count 
            ? ZipMaterials(chapterMaterial, primarchMaterial) 
            : ZipMaterials(primarchMaterial, chapterMaterial);
        
        var temp = new SortedList<int, (ThingDef chapter, ThingDef primarch)>();
        if (chapterMaterial.Count < primarchMaterial.Count )
        {
            foreach (var pair in res)
            {
                temp.Add(pair.Key, (pair.Value.primarch, pair.Value.chapter));
            }
            
            res = temp;
        }
        unlockedMaterials ??= [];
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
        Scribe_Collections.Look(ref unlockedMaterials, "unlockedMaterials", LookMode.Def);
        
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            SetupMaterialList();
        }
    }
}