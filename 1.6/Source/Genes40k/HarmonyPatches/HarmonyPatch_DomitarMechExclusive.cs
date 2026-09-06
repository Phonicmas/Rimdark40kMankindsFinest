using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(MechanitorUtility), "CanControlMech")]
public class DomitarMechExclusivePatch
{
    private static List<Pawn> tmpMechsInAssignedOrder = new();

    public static void Postfix(ref AcceptanceReport __result, Pawn pawn, Pawn mech)
    {
        if (!__result)
        {
            return;
        }

        if (pawn.genes == null)
        {
            return;
        }

        var defMod = mech.def.GetModExtension<DefModExtension_ExclusiveMech>();

        if (defMod == null)
        {
            return;
        }

        var requiredGene = defMod.requiredGeneToControl;

        if (requiredGene != null && !pawn.genes.HasActiveGene(requiredGene))
        {
            __result = "BEWH.MankindsFinest.Ability.PawnDoesNotHaveGeneToControl".Translate(pawn, requiredGene.label);
            return;
        }

        tmpMechsInAssignedOrder.Clear();
        MechanitorUtility.GetMechsInAssignedOrder(pawn, ref tmpMechsInAssignedOrder);

        var pawnHasAmount = 0;

        for (var i = 0; i < tmpMechsInAssignedOrder.Count; i++)
        {
            if (tmpMechsInAssignedOrder[i].def == mech.def)
            {
                pawnHasAmount++;
            }
        }

        tmpMechsInAssignedOrder.Clear();

        if (pawnHasAmount >= defMod.totalAmountAllowedToHave)
        {
            __result = "BEWH.MankindsFinest.Ability.PawnCannotControlMoreMechsOfType".Translate(pawn, mech.def.label, defMod.totalAmountAllowedToHave);
            return;
        }

        __result = true;
    }
}