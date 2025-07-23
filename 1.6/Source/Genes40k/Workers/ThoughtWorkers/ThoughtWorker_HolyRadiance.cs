using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_HolyRadiance : ThoughtWorker
{
    private const float MaxDistForThought = 10;
    private const float MoodBuffThreshold = 0.5f;

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

        var gene = other.genes.GetFirstGeneOfType<Gene_DivineGrace>();

        if (gene == null)
        {
            return false;
        }
        
        if (gene.Value <= MoodBuffThreshold)
        {
            return true;
        }

        if (pawn.genes == null || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
        {
            return true;
        }
        
        if (pawn.Position.DistanceTo(other.Position) <= MaxDistForThought)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemoryFast(Genes40kDefOf.BEWH_LivingSaintHolyRadianceThought);
        }

        return true; 
    }
}