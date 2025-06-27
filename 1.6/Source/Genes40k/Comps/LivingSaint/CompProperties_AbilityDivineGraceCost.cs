using RimWorld;

namespace Genes40k;

public class CompProperties_AbilityDivineGraceCost : CompProperties_AbilityEffect
{
    public float divineGraceCost;
        
    public CompProperties_AbilityDivineGraceCost()
    {
        compClass = typeof(CompAbilityEffect_DivineGraceCost);
    }
}