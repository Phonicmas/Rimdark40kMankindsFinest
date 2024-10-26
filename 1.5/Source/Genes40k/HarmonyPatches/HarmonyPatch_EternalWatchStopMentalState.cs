using HarmonyLib;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public class EternalWatchStopMentalState
    {
        public static bool Prefix(ref bool __result, Pawn ___pawn, MentalStateDef stateDef, string reason = null, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false, bool causedByDamage = false, bool causedByPsycast = false)
        {
            if (___pawn.genes == null) return true;
            
            var gene_EternalWatch = ___pawn.genes.GetFirstGeneOfType<Gene_EternalWatch>();
            if (gene_EternalWatch == null || !___pawn.Drafted) return true;
            
            gene_EternalWatch.SetMentalState(stateDef, reason, causedByMood, otherPawn, transitionSilently, causedByDamage, causedByPsycast);
            __result = false;
            return false;
        }
    }
}