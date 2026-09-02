using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
public class SerfBuffOnSpawn
{
    public static void Postfix(Pawn __instance)
    {
        if (__instance.story?.traits == null || __instance.health == null)
        {
            return;
        }

        if (__instance.story.traits.GetTrait(Genes40kDefOf.BEWH_Serf) == null)
        {
            return;
        }

        if (__instance.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_SerfBuff) == null)
        {
            __instance.health.AddHediff(Genes40kDefOf.BEWH_SerfBuff);
        }
    }
}