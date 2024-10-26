using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
    public class MoreArtJoy
    {
        public static void Prefix(Pawn pawn, float extraJoyGainFactor)
        {
            if (pawn.genes == null)
            {
                return;
            }

            var genes = pawn.genes.GenesListForReading.Where(gene => gene.def.HasModExtension<DefModExtension_IncreasedJoyFromArt>());

            if (!genes.Any()) return;
            
            var multiplier = genes.Aggregate(1f, (current, gene) => current * gene.def.GetModExtension<DefModExtension_IncreasedJoyFromArt>().joyFromArtFactor);
            extraJoyGainFactor = multiplier;
        }
    }
}