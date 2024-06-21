using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class DefModExtension_PotentialHarvest : DefModExtension
    {
        public List<GeneDef> mustHaveOne;
        public List<int> amountFromTier;

        public ThingDef harvestedItem;

        public int chanceToKill;

        public HediffDef backlashHediff;
    }

}