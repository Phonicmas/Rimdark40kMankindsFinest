using RimWorld;
using Verse;

namespace Genes40k;

public class CompProperties_DisableInactiveGene : CompProperties_AbilityEffect
{
    public GeneDef geneDef = null;
        
    public CompProperties_DisableInactiveGene()
    {
        compClass = typeof(Comp_DisableInactiveGene);
    }
}