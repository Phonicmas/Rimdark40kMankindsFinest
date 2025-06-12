using Verse;

namespace Genes40k;

public class CompProperties_LifespanSkyfallerLeave : CompProperties
{
    public int lifespanTicks = 100;

    public EffecterDef expireEffect;

    public ThingDef skyfallerLeaving;

    public CompProperties_LifespanSkyfallerLeave()
    {
        compClass = typeof(Comp_LifespanSkyfallerLeave);
    }
}