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
        var gameComp = Current.Game.GetComponent<GameComponent_UnlockedMaterials>();

        for (var i = 0; i < numThingDefsToUse; i++)
        {
            if (!DefDatabase<ThingDef>.AllDefs.Where(d => HandlesThingDef(d) && d.tradeability.TraderCanSell() && d.PlayerAcquirable && !gameComp.HasMaterial(d) && (excludedThingDefs == null || !excludedThingDefs.Contains(d)) && !generatedDefs.Contains(d)).TryRandomElementByWeight(SelectionWeight, out var chosenThingDef))
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