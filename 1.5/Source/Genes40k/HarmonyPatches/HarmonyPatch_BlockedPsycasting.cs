using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Verb), "Available")]
    public class BlockedPsycasting
    {
        public static bool Postfix(bool __result, Verb_CastAbility __instance)
        {
            if (__instance.ability is Psycast && __instance.CasterPawn != null && __instance.CasterPawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_DeniedWitch))
            {
                __result = false;
                if (__instance.CasterPawn.Faction == Faction.OfPlayer)
                {
                    Messages.Message("BEWH.DeniedWitch".Translate(__instance.CasterPawn), __instance.CasterPawn, MessageTypeDefOf.NeutralEvent);
                }
                return __result;
            }
            return __result;
        }
    }
}