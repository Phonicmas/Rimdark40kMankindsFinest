using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Bill), "Notify_PawnDidWork")]
    public class BillPsycost
    {
        public static void Postfix(Pawn p, Bill __instance)
        {
            DefModExtension_GeneMatrixRecipe defMod = __instance.recipe.GetModExtension<DefModExtension_GeneMatrixRecipe>();
            if (defMod != null && defMod.drainsUserWhenMaking && __instance.billStack != null && __instance.billStack.billGiver is Building_GeneTable geneTable)
            {
                geneTable.PawnDidWork(p);
            }
        }
    }
}