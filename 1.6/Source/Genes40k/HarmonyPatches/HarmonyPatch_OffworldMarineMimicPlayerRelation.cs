using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Faction), "Notify_RelationKindChanged")]
public class OffworldMarineMimicPlayerRelation
{
    public static void Postfix(Faction __instance, Faction other)
    {
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
        
        var playerFaction = Faction.OfPlayer;
        var offworldMarine = Find.FactionManager.FirstFactionOfDef(Genes40kDefOf.BEWH_OffworldMarinesFaction);
        offworldMarine.SetRelation(playerFaction.RelationWith(nonPlayerFaction));
    }
}