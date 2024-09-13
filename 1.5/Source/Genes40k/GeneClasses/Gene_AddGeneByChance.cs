using Core40k;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class Gene_AddGeneByChance : Gene
    {
        private static GeneDef chosenGene;

        public override void PostAdd()
        {
            base.PostAdd();

            chosenGene = SelectGeneToGive();

            if (chosenGene != null)
            {
                pawn.genes.AddGene(chosenGene, true);
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            if (chosenGene != null)
            {
                var gene = pawn.genes.GetGene(chosenGene);
                if (gene != null)
                {
                    pawn.genes.RemoveGene(gene);
                }
            }
        }

        public virtual GeneDef SelectGeneToGive()
        {
            var weightedSelection = new WeightedSelection<GeneDef>();
            var possibleGenes = Genes40kDefOf.BEWH_ChapterXVThousandSons.GetModExtension<DefModExtension_AddGeneByChance>().possibleGenesToGive.Where(g => !pawn.genes.HasActiveGene(g.Key));

            if (possibleGenes.EnumerableNullOrEmpty())
            {
                return null;
            }
            
            foreach (var gene in possibleGenes)
            {
                weightedSelection.AddEntry(gene.Key, gene.Value);
            }

            return weightedSelection.GetRandom();
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref chosenGene, "chosenGene");
        }
    }
}