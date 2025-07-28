using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(RecipeDef), "AvailableNow", MethodType.Getter)]
public class HarmonyPatch_LockedByResearch
{
    public static bool Prefix(ref bool __result, RecipeDef __instance)
    {
        if (!__instance.HasModExtension<DefModExtension_LockedByResearch>())
        {
            return true;
        }
        
        var defMod = __instance.GetModExtension<DefModExtension_LockedByResearch>();
        if (defMod.researchs.Any(research => research.IsFinished))
        {
            __result = false;
            return false;
        }

        return true;
    }
}