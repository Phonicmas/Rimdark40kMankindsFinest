using Verse;

namespace Genes40k;

public class HediffComp_SeverityFromDivineGrace : HediffComp
{
    private Gene_DivineGrace cachedDivineGraceGene;

    public HediffCompProperties_SeverityFromDivineGrace Props => (HediffCompProperties_SeverityFromDivineGrace)props;

    public override bool CompShouldRemove => Pawn.genes?.GetFirstGeneOfType<Gene_DivineGrace>() == null;

    private Gene_DivineGrace DivineGrace => cachedDivineGraceGene ??= Pawn.genes.GetFirstGeneOfType<Gene_DivineGrace>();

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        if (DivineGrace == null)
        {
            return;
        }
            
        if (Props.divineGracePerHour != 0)
        {
            DivineGrace.isOvercharging = true;
        }
    }

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        if (DivineGrace == null)
        {
            return;
        }
            
        if (Props.divineGracePerHour != 0 && !AnyOtherOverchargingHediff())
        {
            DivineGrace.isOvercharging = false;
        }
    }

    /// <summary>
    /// True while another hediff on this pawn is also draining divine grace, so the flag is only
    /// cleared by the last one to be removed.
    /// </summary>
    private bool AnyOtherOverchargingHediff()
    {
        var hediffs = Pawn.health?.hediffSet?.hediffs;

        if (hediffs == null)
        {
            return false;
        }

        foreach (var hediff in hediffs)
        {
            if (hediff == parent || hediff is not HediffWithComps hediffWithComps || hediffWithComps.comps == null)
            {
                continue;
            }

            foreach (var comp in hediffWithComps.comps)
            {
                if (comp is HediffComp_SeverityFromDivineGrace graceComp && graceComp.Props.divineGracePerHour != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (DivineGrace == null)
        {
            return;
        }
            
        severityAdjustment += (DivineGrace.Value > 0f ? Props.severityPerHourDivineGrace : Props.severityPerHourEmpty) / 2500f;
                
        if (Props.divineGracePerHour != 0)
        {
            DivineGrace.ChangeDivineGraceAmountPeriodic(Props.divineGracePerHour / 2500f);
        }
    }
}