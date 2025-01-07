using Verse;

namespace Genes40k
{
    public class HediffComp_SeverityFromDivineRadiance : HediffComp
    {
        private Gene_DivineRadiance cachedDivineRadianceGene;

        public HediffCompProperties_SeverityFromDivineRadiance Props => (HediffCompProperties_SeverityFromDivineRadiance)props;

        public override bool CompShouldRemove => Pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>() == null;

        private Gene_DivineRadiance DivineRadiance
        {
            get
            {
                if (cachedDivineRadianceGene == null)
                {
                    cachedDivineRadianceGene = Pawn.genes.GetFirstGeneOfType<Gene_DivineRadiance>();
                }
                return cachedDivineRadianceGene;
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (Props.divineRadiancePerHour != 0)
            {
                DivineRadiance.isOvercharging = true;
            }
        }

        public override void CompPostPostRemoved()
        {
            if (Props.divineRadiancePerHour != 0)
            {
                DivineRadiance.isOvercharging = false;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (DivineRadiance == null)
            {
                return;
            }
            
            severityAdjustment += (DivineRadiance.Value > 0f ? Props.severityPerHourDivineRadiance : Props.severityPerHourEmpty) / 2500f;
                
            if (Props.divineRadiancePerHour != 0)
            {
                DivineRadiance.ChangeDivineRadianceAmount(Props.divineRadiancePerHour / 2500f);
            }
        }
    }
}