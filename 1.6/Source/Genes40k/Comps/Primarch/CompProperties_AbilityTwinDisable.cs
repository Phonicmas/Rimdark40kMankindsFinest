using RimWorld;

namespace Genes40k
{
    public class CompProperties_AbilityTwinDisable : CompProperties_AbilityEffect
    {
        public bool disableIfDead = true;
        public bool disableIfOnDifferentMap = false;
        
        public CompProperties_AbilityTwinDisable()
        {
            compClass = typeof(Comp_TwinDisable);
        }
    }
}