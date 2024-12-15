using RimWorld;
using Verse;


namespace Genes40k
{
    public class HediffCompProperties_SeverityFromDivineRadiance : HediffCompProperties
    {
        public float severityPerHourEmpty;

        public float severityPerHourDivineRadiance;

        public HediffCompProperties_SeverityFromDivineRadiance()
        {
            compClass = typeof(HediffComp_SeverityFromDivineRadiance);
        }
    }
}