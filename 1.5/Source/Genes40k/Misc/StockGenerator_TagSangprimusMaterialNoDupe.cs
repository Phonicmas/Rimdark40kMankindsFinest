using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class StockGenerator_TagSangprimusMaterialNoDupe : StockGenerator
    {
        [NoTranslate]
        public string tradeTag;

        private IntRange thingDefCountRange = IntRange.one;

        private List<ThingDef> excludedThingDefs;

        public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
        {
            List<ThingDef> generatedDefs = new List<ThingDef>();
            int numThingDefsToUse = thingDefCountRange.RandomInRange;
            for (int i = 0; i < numThingDefsToUse; i++)
            {
                if (!DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => HandlesThingDef(d) && d.tradeability.TraderCanSell() && d.PlayerAcquirable && (excludedThingDefs == null || !excludedThingDefs.Contains(d)) && !generatedDefs.Contains(d)).TryRandomElementByWeight(SelectionWeight, out var chosenThingDef))
                {
                    break;
                }
                foreach (Thing item in StockGeneratorUtility.TryMakeForStock(chosenThingDef, RandomCountOf(chosenThingDef), faction))
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
}