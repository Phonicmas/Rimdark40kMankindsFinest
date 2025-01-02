using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class DefModExtension_GeneInducedFear : DefModExtension
    {
        public List<GeneDef> genesCausesImmunityToFear = new List<GeneDef>();
        
        public List<TraitData> traitCausesImmunityToFear = new List<TraitData>();
    }
}