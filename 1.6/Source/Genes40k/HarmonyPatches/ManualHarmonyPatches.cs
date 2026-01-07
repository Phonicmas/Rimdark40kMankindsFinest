using Verse;

namespace Genes40k;

//Is manually patched
public class ManualHarmonyPatches
{
    public static void StackRemovalDontTriggerPerpetual(Pawn pawn)
    {
        var perpetualGene = pawn?.genes?.GetFirstGeneOfType<Gene_Perpetual>();
        if (perpetualGene == null)
        {
            return;
        }

        perpetualGene.DontAddToPerpetualTracker = true;
    }
}
