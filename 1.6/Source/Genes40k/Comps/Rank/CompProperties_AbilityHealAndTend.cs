using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompProperties_AbilityHealAndTend : CompProperties_AbilityEffect
    {
        public IntRange? healAmount;

        public float maxTendValue = 1f;
        
        public CompProperties_AbilityHealAndTend()
        {
            compClass = typeof(CompAbilityEffect_HealAndTend);
        }
    }
}