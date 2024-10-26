using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(MentalBreakWorker), "TryStart")]
    public class EternalWatchStopMentalBreak
    {
        public static bool Prefix(ref bool __result, MentalBreakWorker __instance, Pawn pawn, string reason, bool causedByMood)
        {
            if (pawn.genes == null) return true;
            
            var gene_EternalWatch = pawn.genes.GetFirstGeneOfType<Gene_EternalWatch>();
            if (gene_EternalWatch == null || !pawn.Drafted) return true;
            
            gene_EternalWatch.SetMentalBreak(__instance.def, reason, causedByMood);
            __result = false;
            return false;
        }
    }
}