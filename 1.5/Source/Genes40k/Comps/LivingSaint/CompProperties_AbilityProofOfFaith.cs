using RimWorld;

namespace Genes40k;

public class CompProperties_AbilityProofOfFaith : CompProperties_AbilityEffectWithDuration
{
    public float divineRadianceGain;
        
    public CompProperties_AbilityProofOfFaith()
    {
        compClass = typeof(CompAbilityEffect_ProofOfFaith);
    }
}