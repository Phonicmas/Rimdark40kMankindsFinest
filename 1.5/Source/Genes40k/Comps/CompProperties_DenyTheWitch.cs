using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompProperties_DenyTheWitch : CompProperties_AbilityEffect
    {
        public float range;

        public HediffDef hediffDef;

        public CompProperties_DenyTheWitch()
        {
            compClass = typeof(CompAbilityEffect_DenyTheWitch);
        }
    }
}