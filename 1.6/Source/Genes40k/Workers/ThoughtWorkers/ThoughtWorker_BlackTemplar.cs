using RimWorld;
using Verse;

namespace Genes40k;

public class ThoughtWorker_BlackTemplar : ThoughtWorker
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
        
        if (!other.IsFirstborn() && !other.IsPrimaris() && !other.IsPrimarch() && !other.IsCustodes())
        {
            return true;
        }

        if (!ModsConfig.IdeologyActive)
        {
            return false;
        }
        
        if (other.Ideo != pawn.Ideo)
        {
            return true;
        }

        return false;
    }
}