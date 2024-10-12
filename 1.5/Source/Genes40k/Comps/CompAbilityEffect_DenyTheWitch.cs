using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_DenyTheWitch : CompAbilityEffect
    {
        private new CompProperties_DenyTheWitch Props => (CompProperties_DenyTheWitch)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            target.Pawn.health.AddHediff(Props.hediffDef);

            base.Apply(target, dest);
        }
    }
}