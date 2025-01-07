using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompProperties_AbilityDenyTheWitch : CompProperties_AbilityEffect
    {
        public float range;

        public HediffDef hediffDef;

        public CompProperties_AbilityDenyTheWitch()
        {
            compClass = typeof(CompAbilityEffect_DenyTheWitch);
        }
    }
}