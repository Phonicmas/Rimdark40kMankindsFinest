using HarmonyLib;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public class SpaceMarineChapterSpawn
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Faction == null || __instance.Faction.IsPlayer || __instance.Faction.def == Genes40kDefOf.BEWH_OffworldMarinesFaction)
            {
                return;
            }

            if (__instance.NonHumanlikeOrWildMan())
            {
                return;
            }

            Genes40kUtils.SetupChapterForPawn(__instance, true);
        }
    }
}