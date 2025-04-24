using System.Linq;
using Core40k;

namespace Genes40k;

public class Gene_Psyker : Gene_AddRandomGeneAndOrTraitByWeight
{
    public override void PostAdd()
    {
        var psykerGenes = pawn.genes.GenesListForReading.Where(gene => gene is Gene_Psyker).ToList();

        var genesToRemove = psykerGenes.Where(psykerGene => psykerGene.def.displayOrderInCategory < def.displayOrderInCategory);

        var removeSelf = psykerGenes.Count(psykerGene => psykerGene.def.displayOrderInCategory > def.displayOrderInCategory) >= 1;
            
        foreach (var gene in genesToRemove)
        {
            pawn.genes.RemoveGene(gene);
        }
            
        base.PostAdd();

        if (removeSelf)
        {
            pawn.genes.RemoveGene(this);
        }
    }
}