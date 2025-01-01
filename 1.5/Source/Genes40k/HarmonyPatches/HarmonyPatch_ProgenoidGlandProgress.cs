    using System.Text;
using HarmonyLib;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "GetInspectString")]
    public class ProgenoidGlandProgress
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
                stringBuilder.AppendLine("BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvested".Translate());
            }
            else
            {
                var secondProgenoid = !progenoidGlands.SecondProgenoidGlandHarvested
                    ? " " + (string)"BEWH.MankindsFinest.SpaceMarine.SecondGeneseedsHarvestableUponDeath".Translate()
                    : string.Empty;

                float ticksLeft = progenoidGlands.TicksUntilHarvestable;
                stringBuilder.AppendLine(ticksLeft > 0
                    ? "BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvestableIn".Translate((ticksLeft / 60000).ToString("0.00"), secondProgenoid)
                    : "BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvestable".Translate());
                
            }
            
            __result = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}