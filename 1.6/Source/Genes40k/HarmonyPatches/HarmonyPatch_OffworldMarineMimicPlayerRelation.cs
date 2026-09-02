using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Faction), "Notify_RelationKindChanged")]
public class OffworldMarineMimicPlayerRelation
{
    public static void Postfix(Faction __instance, Faction other)
    {
        if (__instance == null || other == null)
        {
            return;
        }
        if (__instance.IsPlayer && other.def == Genes40kDefOf.BEWH_OffworldMarinesFaction)
        {
            return;
        }
        if (other.IsPlayer && __instance.def == Genes40kDefOf.BEWH_OffworldMarinesFaction)
        {
            return;
        }
        if (!__instance.IsPlayer && !other.IsPlayer)
        {
            return;
        }

        var nonPlayerFaction = __instance.IsPlayer ? other : __instance;
        
        var offworldMarine = Find.FactionManager?.FirstFactionOfDef(Genes40kDefOf.BEWH_OffworldMarinesFaction);

        if (offworldMarine == null || offworldMarine == nonPlayerFaction)
        {
            return;
        }

        var playerRelation = Faction.OfPlayer?.RelationWith(nonPlayerFaction, true);

        if (playerRelation == null)
        {
            return;
        }

        offworldMarine.SetRelation(new FactionRelation(nonPlayerFaction, playerRelation.kind)
        {
            baseGoodwill = playerRelation.baseGoodwill,
        });
    }
}