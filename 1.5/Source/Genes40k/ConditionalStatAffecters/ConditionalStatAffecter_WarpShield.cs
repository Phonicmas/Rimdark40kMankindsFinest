using RimWorld;
using Verse;

namespace Genes40k;

public class ConditionalStatAffecter_WarpShield : ConditionalStatAffecter
{
    public override string Label => "BEWH.MankindsFinest.WarpShield.Conditional".Translate();
        
    public override bool Applies(StatRequest req)
    {
        if (!req.HasThing || req.Thing is not Pawn pawn)
        {
            return false;
        }

        var warpShield = pawn.genes?.GetFirstGeneOfType<Gene_WarpShield>();

        return warpShield != null && warpShield.IsShielded;
    }
}