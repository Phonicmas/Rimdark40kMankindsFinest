using Core40k;
using Verse;


namespace Genes40k
{
    public class CompProperties_Lifespan : CompProperties
    {
        public HediffDef givesHediff = null;
        public int durationOutsideRange = 250;
        public float range = 10;
        
        public CompProperties_Lifespan()
        {
            compClass = typeof(Comp_Aura);
        }
    }
}