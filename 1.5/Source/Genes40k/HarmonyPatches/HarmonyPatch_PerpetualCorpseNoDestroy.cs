using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
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

            if (__instance.InnerPawn.genes == null || !__instance.InnerPawn.genes.GenesListForReading.Any(gene => gene.def.HasModExtension<DefModExtension_PerpetualGene>()))
            {
                return true;
            }

            if (__instance.Spawned)
            {
                __instance.DeSpawn(); 
                return false;
            }
            
            return true;
        }    
    }
}