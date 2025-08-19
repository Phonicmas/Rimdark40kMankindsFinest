using System.Linq;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Xenophobia : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        if (!other.RaceProps.Humanlike)
        {
            return false;
        }

        if (other.genes == null)
        {
            return false;
        }

        if (other.genes.Xenotype == XenotypeDefOf.Baseliner)
        {
            return false;
        }
        
        var defMod = def.GetModExtension<DefModExtension_ThoughtXenophobiaWhitelist>();
        
        if (defMod != null)
        {
            if (Enumerable.Any(defMod.xenotypesImpure, xenotype => other.genes.GenesListForReading.Select(gene => gene.def).ContainsAllItems(xenotype.genes)))
            {
                return ThoughtState.ActiveAtStage(0);
            }

            if (Enumerable.Any(defMod.xenotypesNotHated, xenotype => other.genes.GenesListForReading.Select(gene => gene.def).ContainsAllItems(xenotype.genes)))
            {
                return false;
            }
        }

        if (!ModsConfig.IdeologyActive)
        {
            return ThoughtState.ActiveAtStage(1);;
        }

        if (defMod != null)
        {
            if (defMod.hateIfDifferentIdeo && other.Ideo != pawn.Ideo)
            {
                return ThoughtState.ActiveAtStage(0);
            }
        }
        
        return ThoughtState.ActiveAtStage(1);;
    }
}