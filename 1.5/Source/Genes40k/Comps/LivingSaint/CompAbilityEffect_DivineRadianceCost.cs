using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_DivineRadianceCost : CompAbilityEffect
    {
        private new CompProperties_AbilityDivineRadianceCost Props => (CompProperties_AbilityDivineRadianceCost)props;

        private bool HasEnoughDivineRadiance
        {
            get
            {
                var geneDivineRadiance = parent.pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>();
                return geneDivineRadiance != null && !(geneDivineRadiance.Value < Props.divineRadianceCost);
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Genes40kUtils.OffsetDivineRadiance(parent.pawn, 0f - Props.divineRadianceCost);
        }

        public override bool GizmoDisabled(out string reason)
        {
            var geneDivineRadiance = parent.pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>();
            if (geneDivineRadiance == null)
            {
                reason = "BEWH.MankindsFinest.Ability.NoDivineRadianceGene".Translate(parent.pawn);
                return true;
            }
            if (geneDivineRadiance.Value < Props.divineRadianceCost)
            {
                reason = "BEWH.MankindsFinest.Ability.NoDivineRadiance".Translate(parent.pawn);
                return true;
            }
            var num2 = Props.divineRadianceCost;
            if (Props.divineRadianceCost > float.Epsilon && num2 > geneDivineRadiance.Value)
            {
                reason = "BEWH.MankindsFinest.Ability.NoDivineRadiance".Translate(parent.pawn);
                return true;
            }
            reason = null;
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return HasEnoughDivineRadiance;
        }
    }
}