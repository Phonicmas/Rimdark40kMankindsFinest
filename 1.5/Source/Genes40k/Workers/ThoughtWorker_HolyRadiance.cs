using LudeonTK;
using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_HolyRadiance : ThoughtWorker
    {
        public float maxDistForThought = 10;

        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (other.genes == null)
            {
                return false;
            }
            if (!other.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
            {
                return false;
            }
            if (pawn.genes != null && !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
            {
                if (pawn.Position.DistanceTo(other.Position) <= maxDistForThought)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemoryFast(Genes40kDefOf.BEWH_LivingSaintHolyRadianceThought);
                }
            }
            return true; 
        }
    }
}