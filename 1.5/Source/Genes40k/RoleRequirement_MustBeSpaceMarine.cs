using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class RoleRequirement_MustBeSpaceMarine : RoleRequirement
    {
        public List<List<GeneDef>> genes;

        [NoTranslate]
        private string labelCached;

        public override string GetLabel(Precept_Role role)
        {
            if (labelCached == null)
            {
                labelCached = "BEWH.MustBeSpaceMarine".Translate();
            }
            return labelCached;
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if (p.genes == null)
            {
                return false;
            }
            foreach (List<GeneDef> list in genes)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (!p.genes.HasActiveGene(list[i]))
                    {
                        break;
                    }
                    if (i == list.Count-1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}