using Verse;

namespace Genes40k;

public class HediffCompProperties_SeverityFromDivineGrace : HediffCompProperties
{
    public float severityPerHourEmpty = 0;

    public float severityPerHourDivineGrace = 0;
        
    public float divineGracePerHour = 0;

    public HediffCompProperties_SeverityFromDivineGrace()
    {
        compClass = typeof(HediffComp_SeverityFromDivineGrace);
    }
}