using System.Collections.Generic;
using Core40k;
using Verse;

namespace Genes40k;

public class DefModExtension_GeneInducedFear : DefModExtension
{
    public List<GeneDef> genesCausesImmunityToFear = new();
        
    public List<TraitData> traitCausesImmunityToFear = new();
}