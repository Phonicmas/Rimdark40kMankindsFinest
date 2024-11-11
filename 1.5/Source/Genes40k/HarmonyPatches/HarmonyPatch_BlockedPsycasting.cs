using System.Collections.Generic;
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
            if (!(__instance.ability is Psycast) || __instance.CasterPawn == null) return __result;
            
            var blockingHediffs = new List<HediffDef> { Genes40kDefOf.BEWH_DeniedWitch, Genes40kDefOf.BEWH_PsychicConnectionSevered};

            for (var i = 0; i < blockingHediffs.Count; i++)
            {
                var hediff = blockingHediffs[i];
                if (__instance.CasterPawn.health.hediffSet.HasHediff(hediff))
                {
                    break;
                }
                if (i + 1 == blockingHediffs.Count)
                {
                    return __result;
                }
            }
            
            __result = false;
            if (__instance.CasterPawn.Faction == Faction.OfPlayer)
            {
                Messages.Message("BEWH.DeniedWitch".Translate(__instance.CasterPawn), __instance.CasterPawn, MessageTypeDefOf.NeutralEvent);
            }
            return false;
        }
    }
}