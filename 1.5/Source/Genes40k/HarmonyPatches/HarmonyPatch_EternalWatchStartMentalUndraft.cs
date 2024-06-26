using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
    public class EternalWatchStartMentalUndraft
    {
        public static void Postfix(Pawn ___pawn, ref bool value)
        {
            if (!value)
            {
                if (___pawn.genes != null)
                {
                    Gene_EternalWatch gene_EternalWatch = ___pawn.genes.GetFirstGeneOfType<Gene_EternalWatch>();
                    if (gene_EternalWatch != null)
                    {
                        gene_EternalWatch.TryDoMentalBreak();
                    }
                }
            }
        }
    }
}