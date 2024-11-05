using System;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "GetInspectString")]
    public class FirstProgenoidGlandProgress
    {
        public static void Postfix(ref string __result, Pawn __instance)
        {
            if (__instance.genes == null || !__instance.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
            {
                return;
            }
            
            var stringBuilder = new StringBuilder(__result);

            var progenoidGlands = (Gene_ProgenoidGlands)__instance.genes.GetGene(Genes40kDefOf.BEWH_ProgenoidGlands);

            stringBuilder.AppendLine("\n");
            
            if (progenoidGlands.FirstProgenoidGlandHarvested)
            {
                stringBuilder.AppendLine("BEWH.FirstGeneseedsHarvested".Translate());
            }
            else
            {
                var secondProgenoid = !progenoidGlands.SecondProgenoidGlandHarvested
                    ? " " + (string)"BEWH.SecondGeneseedsHarvestableUponDeath".Translate()
                    : string.Empty;

                float ticksLeft = progenoidGlands.TicksUntilHarvestable;
                stringBuilder.AppendLine(ticksLeft > 0
                    ? "BEWH.FirstGeneseedsHarvestableIn".Translate((ticksLeft / 60000).ToString("0.00"), secondProgenoid)
                    : "BEWH.FirstGeneseedsHarvestable".Translate());
                
            }
            
            __result = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}