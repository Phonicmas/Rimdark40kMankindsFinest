using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
public class ZealousFuryStopMentalState
{
    public static bool Prefix(ref bool __result, Pawn ___pawn, MentalStateDef stateDef, string reason = null, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false, bool causedByDamage = false, bool causedByPsycast = false)
    {
        var geneZealousFury = ___pawn.genes?.GetFirstGeneOfType<Gene_ZealousFury>();
        if (geneZealousFury == null || !___pawn.IsCombatant())
        {
            return true;
        }
            
        geneZealousFury.SetMentalState(stateDef, reason, causedByMood, otherPawn, transitionSilently, causedByDamage, causedByPsycast);
        __result = false;
        return false;
    }
}