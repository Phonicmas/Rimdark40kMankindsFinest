using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Verb_CastAbility), "ValidateTarget")]
public class BlockedPsycasting
{
    public static void Postfix(ref bool __result, Verb_CastAbility __instance, Thing ___caster, bool showMessages)
    {
        if (__instance.ability is not Psycast || __instance.ability.def.category != Genes40kDefOf.Psychic || ___caster is not Pawn pawn)
        {
            return;
        }

        if (!__instance.ability.def.statBases.ContainsAny(stat => stat.stat == StatDefOf.Ability_EntropyGain || stat.stat == StatDefOf.Ability_PsyfocusCost))
        {
            return;
        }
            
        if (pawn.health == null)
        {
            return;
        }

        if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_DeniedWitch) && !pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_PsychicConnectionSevered))
        {
            return;
        }
            
        if (showMessages && pawn.Faction != null && pawn.Faction == Faction.OfPlayer)
        {
            Messages.Message("BEWH.MankindsFinest.Ability.DeniedWitch".Translate(pawn), pawn, MessageTypeDefOf.NeutralEvent);
        }
            
        __result = false;
    }
}