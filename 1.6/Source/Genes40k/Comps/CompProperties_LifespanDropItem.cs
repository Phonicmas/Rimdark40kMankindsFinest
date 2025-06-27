using Verse;

namespace Genes40k;

public class CompProperties_LifespanDropItem: CompProperties
{
    public int lifespanTicks = 1000;

    public EffecterDef expireEffect;

    public ThingDef droppedThingDef = null;
    
    public int amountDropped = 1;

    public CompProperties_LifespanDropItem()
    {
        compClass = typeof(Comp_LifespanDropItem);
    }
}