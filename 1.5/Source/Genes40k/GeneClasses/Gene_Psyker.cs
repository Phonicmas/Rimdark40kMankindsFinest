using System.Linq;
using Verse;

namespace Genes40k;

public class Gene_Psyker : Gene
{
    public override void PostAdd()
    {
        base.PostAdd();
        
        var psykerGenes = pawn.genes.GenesListForReading.Where(gene => gene is Gene_Psyker).ToList();

        var genesToRemove = psykerGenes.Where(psykerGene => psykerGene.def.displayOrderInCategory < def.displayOrderInCategory);

        var removeSelf = psykerGenes.Count(psykerGene => psykerGene.def.displayOrderInCategory > def.displayOrderInCategory) >= 1;
            
        foreach (var gene in genesToRemove)
        {
            pawn.genes.RemoveGene(gene);
        }

        if (removeSelf)
        {
            pawn.genes.RemoveGene(this);
        }
    }
}