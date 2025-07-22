using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Xenophobia : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        if (!other.RaceProps.Humanlike)
        {
            return false;
        }

        if (other.genes == null)
        {
            return false;
        }
        
        var defMod = def.GetModExtension<DefModExtension_ThoughtXenophobiaWhitelist>();
        
        if (defMod != null)
        {
            if (!defMod.xenotypesNotHated.Contains(other.genes.Xenotype))
            {
                return true;
            }
        }

        if (!ModsConfig.IdeologyActive)
        {
            return false;
        }

        if (defMod != null)
        {
            if (defMod.hateIfDifferentIdeo && other.Ideo != pawn.Ideo)
            {
                return true;
            }
        }
        
        return false;
    }
}