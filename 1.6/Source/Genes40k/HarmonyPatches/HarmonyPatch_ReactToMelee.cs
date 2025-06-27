using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

[HarmonyPatch(typeof(JobGiver_ReactToCloseMeleeThreat), "TryGiveJob")]
public class ReactToMelee
{
    public static void Postfix(ref Job __result, Pawn pawn)
    {
        if (__result == null)
        {
            return;
        }

        if (pawn.mindState.mentalStateHandler.CurState == null || pawn.mindState.mentalStateHandler.CurState.def != Genes40kDefOf.BEWH_InducedFear)
        {
            return;
        }
            
        CellFinderLoose.GetFleeExitPosition(pawn, 999, out var intVec);
        __result = JobMaker.MakeJob(Genes40kDefOf.BEWH_InducedFearJob, intVec);
    }
}