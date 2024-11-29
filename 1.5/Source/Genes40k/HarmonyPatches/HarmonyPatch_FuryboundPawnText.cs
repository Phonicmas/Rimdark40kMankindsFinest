using System.Text;
using HarmonyLib;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "GetInspectString")]
    public class FuryboundPawnText
    {
        public static void Postfix(ref string __result, Pawn __instance)
        {
            if (__instance.genes == null || !__instance.genes.HasActiveGene(Genes40kDefOf.BEWH_Furybound))
            {
                return;
            }
            
            var stringBuilder = new StringBuilder(__result);

            var furybound = (Gene_Furybound)__instance.genes.GetGene(Genes40kDefOf.BEWH_Furybound);

            stringBuilder.AppendLine("\n");
            stringBuilder.AppendLine("BEWH.WarriorChanceToBerserk".Translate(furybound.percentChance));
            
            __result = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}