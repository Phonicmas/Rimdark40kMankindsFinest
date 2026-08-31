using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_PrimarchPresence : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p?.genes == null || p.Map == null)
        {
            return false;
        }

        var chapterGene = p.genes.GenesListForReading
            .FirstOrDefault(gene => gene.def.HasModExtension<DefModExtension_ChapterGene>());

        var relatedPrimarchGene = chapterGene?.def
            .GetModExtension<DefModExtension_ChapterGene>()?.relatedPrimarchGene;

        if (relatedPrimarchGene == null)
        {
            return false;
        }

        var primarchPresent = p.Map.mapPawns.AllPawnsSpawned.Any(other =>
            other != p
            && !other.Dead
            && other.RaceProps.Humanlike
            && other.genes != null
            && other.genes.HasActiveGene(relatedPrimarchGene));

        return primarchPresent;
    }
}
