using AlteredCarbon;
using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Recipe_RemoveNeuralStack), "ApplyOnPawn")]
public class StackRemovalDontTriggerPerpetual
{
    public static void Prefix(Pawn pawn)
    {
        var perpetualGene = pawn?.genes?.GetFirstGeneOfType<Gene_Perpetual>();
        if (perpetualGene == null)
        {
            return;
        }

        perpetualGene.DontAddToPerpetualTracker = true;
    }
}