using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(MechanitorUtility), "CanControlMech")]
public class DomitarMechExclusivePatch
{
    public static void Postfix(ref AcceptanceReport __result, Pawn pawn, Pawn mech)
    {
        if (!__result)
        {
            return;
        }

        if (!mech.def.HasModExtension<DefModExtension_ExclusiveMech>() || pawn.genes == null)
        {
            return;
        }

        var defMod = mech.def.GetModExtension<DefModExtension_ExclusiveMech>();
        var requiredGene = defMod.requiredGeneToControl;

        if (requiredGene != null && !pawn.genes.HasActiveGene(requiredGene))
        {
            __result = "BEWH.MankindsFinest.Ability.PawnDoesNotHaveGeneToControl".Translate(pawn, requiredGene.label);
            return;
        }
            
        var tmpMechsInAssignedOrder = new List<Pawn>();

        MechanitorUtility.GetMechsInAssignedOrder(pawn, ref tmpMechsInAssignedOrder);

        var pawnHasAmount = Enumerable.Count(tmpMechsInAssignedOrder, controlledMech => controlledMech.def == mech.def);

        if (pawnHasAmount >= defMod.totalAmountAllowedToHave)
        {
            __result = "BEWH.MankindsFinest.Ability.PawnCannotControlMoreMechsOfType".Translate(pawn, mech.def.label, defMod.totalAmountAllowedToHave);
            return;
        }

        __result = true;
    }
}