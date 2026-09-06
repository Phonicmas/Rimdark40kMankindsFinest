using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Xenophobia : ThoughtWorker
{
    private static readonly HashSet<GeneDef> tmpGeneDefs = new();

    private DefModExtension_ThoughtXenophobiaWhitelist cachedDefMod;
    private bool defModResolved;

    private DefModExtension_ThoughtXenophobiaWhitelist DefMod
    {
        get
        {
            if (!defModResolved)
            {
                cachedDefMod = def.GetModExtension<DefModExtension_ThoughtXenophobiaWhitelist>();
                defModResolved = true;
            }

            return cachedDefMod;
        }
    }

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

        var defMod = DefMod;

        if (defMod != null)
        {
            tmpGeneDefs.Clear();

            foreach (var gene in other.genes.GenesListForReading)
            {
                tmpGeneDefs.Add(gene.def);
            }

            var impure = AnyXenotypeMatches(defMod.xenotypesImpure);
            var notHated = !impure && AnyXenotypeMatches(defMod.xenotypesNotHated);

            tmpGeneDefs.Clear();

            if (impure)
            {
                return ThoughtState.ActiveAtStage(0);
            }

            if (notHated)
            {
                return false;
            }
        }

        if (!ModsConfig.IdeologyActive)
        {
            return ThoughtState.ActiveAtStage(1);
        }

        if (defMod != null && defMod.hateIfDifferentIdeo && other.Ideo != pawn.Ideo)
        {
            return ThoughtState.ActiveAtStage(0);
        }

        return ThoughtState.ActiveAtStage(1);
    }

    /// <summary>
    /// True if every gene of any listed xenotype is in tmpGeneDefs. Same result as ContainsAllItems
    /// over the gene list, without an enumerator and set per xenotype.
    /// </summary>
    private static bool AnyXenotypeMatches(List<XenotypeDef> xenotypes)
    {
        if (xenotypes.NullOrEmpty())
        {
            return false;
        }

        foreach (var xenotype in xenotypes)
        {
            if (xenotype.genes.NullOrEmpty())
            {
                return true;
            }

            var all = true;

            foreach (var geneDef in xenotype.genes)
            {
                if (!tmpGeneDefs.Contains(geneDef))
                {
                    all = false;
                    break;
                }
            }

            if (all)
            {
                return true;
            }
        }

        return false;
    }
}