using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Pariah : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        if (!other.RaceProps.Humanlike)
        {
            return false;
        }

        if (other.genes == null || pawn?.genes == null)
        {
            return false;
        }

        var thoughtPariah = def.GetModExtension<DefModExtension_Pariah>();

        if (thoughtPariah == null)
        {
            return false;
        }
            
        foreach (var gene in other.genes.GenesListForReading)
        {
            var genePariah = gene.def.GetModExtension<DefModExtension_Pariah>();

            if (genePariah == null)
            {
                continue;
            }
                
            if (genePariah.pariahGene == thoughtPariah.pariahGene)
            {
                return !pawn.IsPariah() && !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_MnemosyneMindshield);
            }
        }
        return false;
    }
}