using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class CompProperties_DeteriorateOutsideBuilding : CompProperties
    {
        public List<ThingDef> antiDeteriorateContainers;

        public CompProperties_DeteriorateOutsideBuilding()
        {
            compClass = typeof(Comp_DeteriorateOutsideBuilding);
        }

    }
}