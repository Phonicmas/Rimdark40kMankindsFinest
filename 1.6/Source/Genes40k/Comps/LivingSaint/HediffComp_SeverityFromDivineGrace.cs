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
            
        if (Props.divineGracePerHour != 0)
        {
            DivineGrace.isOvercharging = false;
        }
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
            DivineGrace.ChangeDivineGraceAmount(Props.divineGracePerHour / 2500f);
        }
    }
}