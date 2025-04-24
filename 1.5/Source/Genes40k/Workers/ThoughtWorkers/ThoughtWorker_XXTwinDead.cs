using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_XXTwinDead : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.genes == null)
        {
            return false;
        }
            
        var gene = p.genes.GetGene(Genes40kDefOf.BEWH_PrimarchSpecificGeneXX);
        if (gene == null)
        {
            return false;
        }

        var twinGene = (Gene_TwinConnected)gene;
        return twinGene.Twin.Dead;
    }
}