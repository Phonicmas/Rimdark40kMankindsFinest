using Verse;

namespace Genes40k;

public class HediffCompProperties_SeverityFromDivineRadiance : HediffCompProperties
{
    public float severityPerHourEmpty = 0;

    public float severityPerHourDivineRadiance = 0;
        
    public float divineRadiancePerHour = 0;

    public HediffCompProperties_SeverityFromDivineRadiance()
    {
        compClass = typeof(HediffComp_SeverityFromDivineRadiance);
    }
}