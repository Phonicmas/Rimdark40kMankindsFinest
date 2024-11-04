﻿using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Corpse), "GetInspectString")]
    public class SecondProgenoidGlandProgress
    {
        public static void Postfix(ref string __result, Corpse __instance)
        {
            if (__instance.InnerPawn.genes == null || !__instance.InnerPawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
            {
                return;
            }
            
            var stringBuilder = new StringBuilder(__result);

            var progenoidGlands = (Gene_ProgenoidGlands)__instance.InnerPawn.genes.GetGene(Genes40kDefOf.BEWH_ProgenoidGlands);

            stringBuilder.AppendLine("\n");

            stringBuilder.AppendLine(progenoidGlands.SecondProgenoidGlandHarvested
                ? "BEWH.SecondGeneseedsHarvested".Translate()
                : "BEWH.SecondGeneseedsHarvestable".Translate());

            __result = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}