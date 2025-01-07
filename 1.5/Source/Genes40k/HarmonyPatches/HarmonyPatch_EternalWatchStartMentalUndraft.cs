using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
    public class EternalWatchStartMentalUndraft
    {
        public static void Postfix(Pawn ___pawn, ref bool value)
        {
            if (value || ___pawn.genes == null) return;
            
            var gene_EternalWatch = ___pawn.genes.GetFirstGeneOfType<Gene_EternalWatch>();
            gene_EternalWatch?.TryDoMentalBreak();
        }
    }
}