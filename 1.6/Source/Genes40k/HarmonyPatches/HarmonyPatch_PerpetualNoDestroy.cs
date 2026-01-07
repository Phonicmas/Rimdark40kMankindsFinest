using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Pawn), "Destroy")]
[HarmonyPriority(Priority.Low)]
public class PerpetualNoDestroy
{
    public static bool Prefix(Pawn __instance, ref DestroyMode mode)
    {
        if (__instance == null)
        {
            return true;
        }

        if (Current.ProgramState == ProgramState.Entry)
        {
            return true;
        }
            
        //SmashPhil Vehicle case, hopefully doesnt cause any side effects. Pawns shouldn't be targeted by deconstruct anyway.
        if (mode == DestroyMode.Deconstruct)
        {
            mode = DestroyMode.Vanish;
            GenLeaving.DoLeavingsFor(__instance, __instance.Map, DestroyMode.Deconstruct);
            return true;
        }

        var perpetualGene = __instance.genes?.GetFirstGeneOfType<Gene_Perpetual>();

        if (perpetualGene == null)
        {
            return true;
        }

        if (!__instance.Dead)
        {
            __instance.Kill(null);
            return false;
        }

        if (__instance.Spawned)
        {
            if (__instance.PawnHasAlteredCarbonStack())
            {
                perpetualGene.AddPawnToPerpetualTracker();
            }
            __instance.DeSpawn(); 
            return false;
        }

        if (__instance.Corpse != null && mode == DestroyMode.Vanish)
        {
            __instance.Corpse.DeSpawn();
            return false;
        }
            
        return true;
    }    
}