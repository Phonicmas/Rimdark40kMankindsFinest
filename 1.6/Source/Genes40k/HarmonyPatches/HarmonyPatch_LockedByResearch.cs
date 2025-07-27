using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(RecipeDef), "AvailableNow", MethodType.Getter)]
public class HarmonyPatch_LockedByResearch
{
    public static void Postfix(ref bool __result, RecipeDef __instance)
    {
        if (!__instance.HasModExtension<DefModExtension_LockedByResearch>())
        {
            return;
        }
        
        var defMod = __instance.GetModExtension<DefModExtension_LockedByResearch>();

        if (defMod.researchs.Any(research => research.IsFinished))
        {
            __result = false;
        }
    }
}