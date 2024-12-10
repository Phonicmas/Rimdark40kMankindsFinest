using Core40k;
using Verse;


namespace Genes40k
{
    public class CompProperties_Aura : CompProperties
    {
        public HediffDef givesHediff = null;
        public int durationOutsideRange = 250;
        public float range = 10;
        
        public CompProperties_Aura()
        {
            compClass = typeof(Comp_Aura);
        }
    }
}