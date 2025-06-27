using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_ThunderWarrior : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var gene = p?.genes?.GetFirstGeneOfType<Gene_Furybound>();
        return gene == null ? false : ThoughtState.ActiveAtStage(gene.CurrentThoughtStage);
    }
}