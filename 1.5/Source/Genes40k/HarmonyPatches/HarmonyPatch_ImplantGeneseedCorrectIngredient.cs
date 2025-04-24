using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(WorkGiver_DoBill), "IsUsableIngredient")]
public class ImplantGeneseedCorrectIngredient
{
    public static void Postfix(ref bool __result, Thing t, Bill bill)
    {
        if (!__result || !bill.recipe.HasModExtension<DefModExtension_GeneseedVialRecipe>() || !(t is GeneseedVial geneseedVial))
        {
            return;
        }
            
        var defMod = bill.recipe.GetModExtension<DefModExtension_GeneseedVialRecipe>();
            
        if (defMod.geneFromMaterial != null && geneseedVial.GeneSet.GenesListForReading.Contains(defMod.geneFromMaterial))
        {
            __result = true;
            return;
        }

        if (defMod.geneFromMaterial == null && geneseedVial.extraGeneFromMaterial == null)
        {
            __result = true;
        }
    }    
}