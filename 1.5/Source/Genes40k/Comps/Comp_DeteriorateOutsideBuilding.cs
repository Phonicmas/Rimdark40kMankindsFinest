using RimWorld;
using UnityEngine;
using UnityEngine.Diagnostics;
using Verse;

namespace Genes40k
{
    //Thanks to VE for letting me repurpose code from VE ancients
    public class Comp_DeteriorateOutsideBuilding : ThingComp
    {
        public CompProperties_DeteriorateOutsideBuilding Props => props as CompProperties_DeteriorateOutsideBuilding;

        public bool ShouldDeteriorate
        {
            get
            {
                var thing = parent.StoringThing();
                if (thing == null || !Props.antiDeteriorateContainers.Contains(thing.def)) return true;
                
                var comp = thing.TryGetComp<CompPowerTrader>();
                if (comp != null)
                {
                    return !comp.PowerOn;
                }
                return false;
            }
        }
    }
}