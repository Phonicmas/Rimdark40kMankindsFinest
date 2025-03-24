using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
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
            
            //SmashPhil Vehicle case, hopefully doesnt cause any side effects. Pawns shouldn't be targeted by deconstruct anyway.
            if (mode == DestroyMode.Deconstruct)
            {
                mode = DestroyMode.Vanish;
                GenLeaving.DoLeavingsFor(__instance, __instance.Map, DestroyMode.Deconstruct);
                return true;
            }
            
            if (__instance.genes == null || !__instance.genes.GenesListForReading.Any(gene => gene.def.HasModExtension<DefModExtension_PerpetualGene>()))
            {
                return true;
            }

            if (!__instance.Dead)
            {
                __instance.Kill(null);
            }

            if (__instance.Spawned)
            {
                __instance.DeSpawn(); 
            }

            if (__instance.Corpse != null && __instance.Corpse.Spawned)
            {
                __instance.Corpse.DeSpawn();
            }
            
            return false;
        }    
    }
}