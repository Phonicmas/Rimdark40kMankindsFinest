using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(SurgeryOutcomeEffectDef), "GetQuality")]
public class RubiconSurgeryChanceIncrease
{
    public static void Postfix(ref float __result, RecipeDef recipe, Pawn patient)
    {
        if (recipe != Genes40kDefOf.BEWH_RubiconSurgery)
        {
            return;
        }
        if (patient.genes == null)
        {
            return;
        }

        var extraOffset = patient.genes.GenesListForReading.Where(gene => gene.def.HasModExtension<DefModExtension_GeneseedPurity>()).Sum(gene => gene.def.GetModExtension<DefModExtension_GeneseedPurity>().rubiconAdditionalChanceOffset);
            
        __result += extraOffset;
    }
}