using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_DoBillPsychic : WorkGiver_DoBill
{
    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (pawn.story?.traits == null)
        {
            return null;
        }

        if (pawn.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, -1) || pawn.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, -2) || pawn.GetStatValue(StatDefOf.PsychicSensitivity) == 0)
        {
            return null;
        }
        
        return base.JobOnThing(pawn, thing, forced);
    }
}