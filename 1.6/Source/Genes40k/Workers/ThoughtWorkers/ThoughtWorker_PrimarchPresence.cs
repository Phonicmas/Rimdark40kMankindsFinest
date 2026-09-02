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

        var relatedPrimarchGene = p.RelatedPrimarchGeneFor();

        if (relatedPrimarchGene == null)
        {
            return false;
        }

        var presence = p.Map.GetComponent<MapComponent_PrimarchPresence>();

        if (presence == null)
        {
            return false;
        }

        //A primarch carries their own gene and is counted by the map component, so they only feel
        //the presence of a second carrier.
        var selfCounted = p.Spawned && p.genes.HasActiveGene(relatedPrimarchGene);

        return presence.CountOf(relatedPrimarchGene) > (selfCounted ? 1 : 0);
    }
}