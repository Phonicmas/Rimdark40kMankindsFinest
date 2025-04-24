using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_DenyTheWitch : CompAbilityEffect
{
    private new CompProperties_AbilityDenyTheWitch Props => (CompProperties_AbilityDenyTheWitch)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        target.Pawn.health.AddHediff(Props.hediffDef);

        base.Apply(target, dest);
    }
}