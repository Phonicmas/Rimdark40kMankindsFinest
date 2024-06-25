using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn_RoyaltyTracker), "AddPermit")]
    public class KeepPrerequisitePermit
    {
        public static void Postfix(RoyalTitlePermitDef permit, Faction faction, Pawn_RoyaltyTracker __instance)
        {
            if (!permit.HasModExtension<DefModExtension_KeepPrerequisitePermit>())
            {
                return;
            }

            if (permit.prerequisite == null)
            {
                return;
            }
            __instance.AllFactionPermits.Add(new FactionPermit(faction, __instance.GetCurrentTitle(faction), permit.prerequisite));
        }
    }
}