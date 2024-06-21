using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Pariah : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (pawn.story.traits.HasTrait(TraitDefOf.Kind))
            {
                return false;
            }
            if (other.genes != null)
            {
                foreach (Gene gene in other.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_Pariah>() && def.HasModExtension<DefModExtension_Pariah>())
                    {
                        if (gene.def.GetModExtension<DefModExtension_Pariah>().pariahGene == def.GetModExtension<DefModExtension_Pariah>().pariahGene)
                        {
                            if (Genes40kUtils.IsPariah(pawn))
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}