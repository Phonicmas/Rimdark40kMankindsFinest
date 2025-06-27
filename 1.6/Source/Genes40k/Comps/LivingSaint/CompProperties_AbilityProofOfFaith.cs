using RimWorld;

namespace Genes40k;

public class CompProperties_AbilityProofOfFaith : CompProperties_AbilityEffectWithDuration
{
    public float divineGraceGain;
        
    public CompProperties_AbilityProofOfFaith()
    {
        compClass = typeof(CompAbilityEffect_ProofOfFaith);
    }
}