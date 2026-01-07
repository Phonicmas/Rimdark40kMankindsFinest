using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Corpse), "Destroy")]
[HarmonyPriority(Priority.Low)]
public class PerpetualCorpseNoDestroy
{
    public static bool Prefix(Corpse __instance, ref DestroyMode mode)
    {
        if (__instance?.InnerPawn == null)
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

        var perpetualGene = __instance.InnerPawn.genes?.GetFirstGeneOfType<Gene_Perpetual>();

        if (perpetualGene == null)
        {
            return true;
        }
        
        if (!__instance.Spawned)
        {
            return true;
        }
        
        if (__instance.InnerPawn.PawnHasAlteredCarbonStack())
        {
            perpetualGene.AddPawnToPerpetualTracker();
        }
        
        __instance.DeSpawn(); 
        return false;

    }    
}