using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_DenyTheWitch : CompAbilityEffect
{
    private new CompProperties_AbilityDenyTheWitch Props => (CompProperties_AbilityDenyTheWitch)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        if (target.Pawn?.health == null)
        {
            return;
        }

        target.Pawn.health.AddHediff(Props.hediffDef);

        base.Apply(target, dest);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        return base.Valid(target, throwMessages) && target.Pawn?.health != null;
    }
}