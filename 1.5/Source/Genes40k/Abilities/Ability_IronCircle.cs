using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k
{
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
        
        public override bool GizmoDisabled(out string reason)
        {
            var comp = (CompProperties_AbilitySummonMechsForCaster)def.comps.First(properties => properties is CompProperties_AbilitySummonMechsForCaster);
            
            var amountToSpawn = comp.amount;
            
            if (comp.cannotHaveMoreThanAmount)
            {
                var tmpMechsInAssignedOrder = new List<Pawn>();
                
                MechanitorUtility.GetMechsInAssignedOrder(pawn, ref tmpMechsInAssignedOrder);
                amountToSpawn -= tmpMechsInAssignedOrder.Count;
            }

            if (amountToSpawn < 1)
            {
                reason = "BEWH.WillNotSummonAny".Translate(comp.mechKind.label);
                return true;
            }
            
            return base.GizmoDisabled(out reason);
        }
    }
}