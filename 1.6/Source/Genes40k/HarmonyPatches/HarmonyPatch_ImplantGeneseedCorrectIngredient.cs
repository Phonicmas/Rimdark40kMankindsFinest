using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(WorkGiver_DoBill), "IsUsableIngredient")]
public class ImplantGeneseedCorrectIngredient
{
    public static void Postfix(ref bool __result, Thing t, Bill bill)
    {
        if (!__result || t is not GeneseedVial geneseedVial)
        {
            return;
        }

        var defMod = bill.recipe.GetModExtension<DefModExtension_GeneseedVialRecipe>();

        if (defMod == null)
        {
            return;
        }

        __result = geneseedVial.extraGeneFromMaterial == defMod.geneFromMaterial;
    }    
}