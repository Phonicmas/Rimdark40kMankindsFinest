using System.Linq;
using Core40k;
using Verse;

namespace Genes40k;

public class Gene_Psyker : Gene_GiveVEFAbility
{
    public override void PostAdd()
    {
        base.PostAdd();

        var otherPsykerGenes = pawn.genes.GenesListForReading.Where(gene => gene is Gene_Psyker && gene != this).ToList();
        var removeSelf = false;

        foreach (var gene in otherPsykerGenes)
        {
            var comparison = CompareTier(gene.def, def);

            if (comparison > 0)
            {
                removeSelf = true;
            }
            else if (comparison < 0 || gene.def != def)
            {
                pawn.genes.RemoveGene(gene);
            }
        }

        if (removeSelf)
        {
            pawn.genes.RemoveGene(this);
        }
    }

    /// <summary>
    /// Orders two psyker genes by DefModExtension_Psyker.tier. If either has no tier set, both fall back
    /// to displayOrderInCategory so genes from other mods keep their old ordering.
    /// </summary>
    private static int CompareTier(GeneDef a, GeneDef b)
    {
        var tierA = a.GetModExtension<DefModExtension_Psyker>()?.tier ?? 0;
        var tierB = b.GetModExtension<DefModExtension_Psyker>()?.tier ?? 0;

        if (tierA > 0 && tierB > 0)
        {
            return tierA.CompareTo(tierB);
        }

        return a.displayOrderInCategory.CompareTo(b.displayOrderInCategory);
    }
}