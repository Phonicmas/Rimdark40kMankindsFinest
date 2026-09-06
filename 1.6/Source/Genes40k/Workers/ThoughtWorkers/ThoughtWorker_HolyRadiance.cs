using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_HolyRadiance : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        if (other.genes == null || !other.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
        {
            return false;
        }

        if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
        {
            return false;
        }

        //The nearby mood memory is granted from Gene_DivineGrace's tick; this worker only reports the opinion.
        return other.genes.GetFirstGeneOfType<Gene_DivineGrace>() != null;
    }
}