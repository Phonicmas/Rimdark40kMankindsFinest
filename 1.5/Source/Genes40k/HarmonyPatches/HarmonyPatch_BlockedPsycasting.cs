using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Verb_CastAbility), "ValidateTarget")]
    public class BlockedPsycasting
    {
        public static void Postfix(ref bool __result, Verb_CastAbility __instance, Thing ___caster)
        {
            if (!(__instance.ability is Psycast) || __instance.ability.def.category != Genes40kDefOf.Psychic || !(___caster is Pawn pawn))
            {
                return;
            }

            if (!__instance.ability.def.statBases.ContainsAny(stat => stat.stat == StatDefOf.Ability_EntropyGain || stat.stat == StatDefOf.Ability_PsyfocusCost))
            {
                return;
            }
            
            var blockingHediffs = new List<HediffDef> { Genes40kDefOf.BEWH_DeniedWitch, Genes40kDefOf.BEWH_PsychicConnectionSevered };
            
            for (var i = 0; i < blockingHediffs.Count; i++)
            {
                var hediff = blockingHediffs[i];
                if (pawn.health != null && pawn.health.hediffSet.HasHediff(hediff))
                {
                    break;
                }
                if (i + 1 == blockingHediffs.Count)
                {
                    return;
                }
            }
            
            if (pawn.Faction != null && pawn.Faction == Faction.OfPlayer)
            {
                Messages.Message("BEWH.MankindsFinest.Ability.DeniedWitch".Translate(pawn), pawn, MessageTypeDefOf.NeutralEvent);
            }
            
            __result = false;
        }
    }
}