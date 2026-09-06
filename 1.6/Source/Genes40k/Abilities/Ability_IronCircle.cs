using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k;

public class Ability_IronCircle: Ability
{
    public Ability_IronCircle()
    {
    }

    public Ability_IronCircle(Pawn pawn)
    {
        this.pawn = pawn;
    }

    public Ability_IronCircle(Pawn pawn, Precept sourcePrecept)
    {
        this.pawn = pawn;
        this.sourcePrecept = sourcePrecept;
    }

    public Ability_IronCircle(Pawn pawn, AbilityDef def)
    {
        this.pawn = pawn;
        this.def = def;
        Initialize();
    }

    public Ability_IronCircle(Pawn pawn, Precept sourcePrecept, AbilityDef def)
    {
        this.pawn = pawn;
        this.def = def;
        this.sourcePrecept = sourcePrecept;
        Initialize();
    }
        
    private static List<Pawn> tmpMechsInAssignedOrder = new();

    private CompProperties_AbilitySummonMechsForCaster summonProps;
    private bool summonPropsResolved;

    private CompProperties_AbilitySummonMechsForCaster SummonProps
    {
        get
        {
            if (!summonPropsResolved && def != null)
            {
                summonProps = def.comps?.OfType<CompProperties_AbilitySummonMechsForCaster>().FirstOrDefault();
                summonPropsResolved = true;
            }

            return summonProps;
        }
    }

    public override bool GizmoDisabled(out string reason)
    {
        var comp = SummonProps;

        if (comp == null)
        {
            return base.GizmoDisabled(out reason);
        }

        var amountToSpawn = comp.amount;

        if (comp.cannotHaveMoreThanAmount)
        {
            tmpMechsInAssignedOrder.Clear();
            MechanitorUtility.GetMechsInAssignedOrder(pawn, ref tmpMechsInAssignedOrder);
            amountToSpawn -= tmpMechsInAssignedOrder.Count;
            tmpMechsInAssignedOrder.Clear();
        }

        if (amountToSpawn < 1)
        {
            reason = "BEWH.MankindsFinest.Ability.WillNotSummonAny".Translate(comp.mechKind.label);
            return true;
        }
            
        return base.GizmoDisabled(out reason);
    }
}