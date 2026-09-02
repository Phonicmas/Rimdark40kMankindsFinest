using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Serf : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.health?.hediffSet?.GetFirstHediffOfDef(def.hediff) is not Hediff_Serf serfHediff)
        {
            return ThoughtState.ActiveAtStage(0);
        }

        //Hediff_Serf severity runs 1 (no superhuman) to 3 (one nearby); the thought has the same
        //three stages one step lower, so it reads the already-computed value instead of rescanning.
        return ThoughtState.ActiveAtStage(Mathf.Clamp((int)serfHediff.Severity - 1, 0, 2));
    }

    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        return true;
    }
}