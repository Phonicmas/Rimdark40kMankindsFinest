using RimWorld;

namespace Genes40k;

public class CompProperties_AbilityDivineRadianceCost : CompProperties_AbilityEffect
{
    public float divineRadianceCost;
        
    public CompProperties_AbilityDivineRadianceCost()
    {
        compClass = typeof(CompAbilityEffect_DivineRadianceCost);
    }
}