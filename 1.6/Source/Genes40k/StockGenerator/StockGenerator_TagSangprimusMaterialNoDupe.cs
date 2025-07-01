using RimWorld;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class StockGenerator_TagSangprimusMaterialNoDupe : StockGenerator
{
    [NoTranslate]
    public string tradeTag;

    private IntRange thingDefCountRange = IntRange.One;

    private List<ThingDef> excludedThingDefs = new List<ThingDef>();

    public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
    {
        var generatedDefs = new List<ThingDef>();
        var numThingDefsToUse = thingDefCountRange.RandomInRange;
        var maps = Find.Maps.Where(m => m.listerBuildings.ColonistsHaveBuilding(Genes40kDefOf.BEWH_SangprimusPortum));
        var excludedMaterials = new List<ThingDef>();
        //In case it is ever a problem; count amount of maps with a sangprimus then make that the limit of the amount of materials
        foreach (var map in maps)
        {
            var building = (Building_SangprimusPortum)map.listerBuildings.AllBuildingsColonistOfDef(Genes40kDefOf.BEWH_SangprimusPortum).First();
            excludedMaterials.AddRange(building.GetDirectlyHeldThings().Select(material => material.def));
        }
        for (var i = 0; i < numThingDefsToUse; i++)
        {
            if (!DefDatabase<ThingDef>.AllDefs.Where(d => HandlesThingDef(d) && d.tradeability.TraderCanSell() && d.PlayerAcquirable && !excludedMaterials.Contains(d) && (excludedThingDefs == null || !excludedThingDefs.Contains(d)) && !generatedDefs.Contains(d)).TryRandomElementByWeight(SelectionWeight, out var chosenThingDef))
            {
                break;
            }
            foreach (var item in StockGeneratorUtility.TryMakeForStock(chosenThingDef, RandomCountOf(chosenThingDef), faction))
            {
                yield return item;
            }
            generatedDefs.Add(chosenThingDef);
            chosenThingDef = null;
        }
    }

    public override bool HandlesThingDef(ThingDef thingDef)
    {
        if (thingDef.tradeTags != null && thingDef.tradeability != 0 && (int)thingDef.techLevel <= (int)maxTechLevelBuy)
        {
            return thingDef.tradeTags.Contains(tradeTag);
        }
        return false;
    }

    protected virtual float SelectionWeight(ThingDef thingDef)
    {
        return 1f;
    }
}