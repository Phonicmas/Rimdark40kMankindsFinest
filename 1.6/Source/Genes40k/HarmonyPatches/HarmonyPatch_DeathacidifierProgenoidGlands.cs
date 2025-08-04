using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(HediffComp_DissolveGearOnDeath), "Notify_PawnKilled")]
public class DeathacidifierProgenoidGlands
{
    public static void Postfix(HediffComp_DissolveGearOnDeath __instance)
    {
        var gene = __instance.Pawn.genes?.GetFirstGeneOfType<Gene_ProgenoidGlands>();

        if (gene == null)
        {
            return;
        }
        
        gene.HarvestFirstProgenoidGland();
        gene.HarvestSecondProgenoidGland();
    }
}