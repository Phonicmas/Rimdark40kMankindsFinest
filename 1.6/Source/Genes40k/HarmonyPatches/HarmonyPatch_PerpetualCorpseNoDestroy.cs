using HarmonyLib;
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
        
        if (ModsConfig.IsActive("SmashPhil.VehicleFramework"))
        {
            if (__instance.GetType().Name == "VehiclePawn")
            {
                return true;
            }
        }

        var perpetualGene = __instance.InnerPawn.genes?.GetFirstGeneOfType<Gene_Perpetual>();

        if (perpetualGene == null)
        {
            return true;
        }

        if (perpetualGene.DontAddToPerpetualTracker)
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