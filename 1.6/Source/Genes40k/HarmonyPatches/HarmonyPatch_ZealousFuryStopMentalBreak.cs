using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

[HarmonyPatch(typeof(MentalBreakWorker), "TryStart")]
public class ZealousFuryStopMentalBreak
{
    public static bool Prefix(ref bool __result, MentalBreakWorker __instance, Pawn pawn, string reason, bool causedByMood)
    {
        var geneZealousFury = pawn?.genes?.GetFirstGeneOfType<Gene_ZealousFury>();
        if (geneZealousFury == null || !pawn.IsCombatant())
        {
            return true;
        }
            
        geneZealousFury.SetMentalBreak(__instance.def, reason, causedByMood);
        __result = false;
        return false;
    }
}