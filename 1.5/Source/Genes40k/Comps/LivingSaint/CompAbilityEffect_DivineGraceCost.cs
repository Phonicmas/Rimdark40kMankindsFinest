using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_DivineGraceCost : CompAbilityEffect
{
    private new CompProperties_AbilityDivineGraceCost Props => (CompProperties_AbilityDivineGraceCost)props;

    private bool HasEnoughDivineGrace
    {
        get
        {
            var geneDivineGrace = parent.pawn.genes?.GetFirstGeneOfType<Gene_DivineGrace>();
            return geneDivineGrace != null && !(geneDivineGrace.Value < Props.divineGraceCost);
        }
    }
        
    public override void PostApplied(List<LocalTargetInfo> targets, Map map)
    {
        Genes40kUtils.OffsetDivineGrace(parent.pawn, 0f - Props.divineGraceCost);
    }
        
    public override string ExtraTooltipPart()
    {
        return "BEWH.MankindsFinest.LivingSaint.DivineGraceCost".Translate(Props.divineGraceCost * 100);
    }

    public override bool GizmoDisabled(out string reason)
    {
        var geneDivineGrace = parent.pawn.genes?.GetFirstGeneOfType<Gene_DivineGrace>();
        if (geneDivineGrace == null)
        {
            reason = "BEWH.MankindsFinest.Ability.NoDivineGraceGene".Translate(parent.pawn);
            return true;
        }
        if (geneDivineGrace.Value < Props.divineGraceCost)
        {
            reason = "BEWH.MankindsFinest.Ability.NoDivineGrace".Translate(parent.pawn);
            return true;
        }
        var num2 = Props.divineGraceCost;
        if (Props.divineGraceCost > float.Epsilon && num2 > geneDivineGrace.Value)
        {
            reason = "BEWH.MankindsFinest.Ability.NoDivineGrace".Translate(parent.pawn);
            return true;
        }
        reason = null;
        return false;
    }

    public override bool AICanTargetNow(LocalTargetInfo target)
    {
        return HasEnoughDivineGrace;
    }
}