using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_SummonMechsForCaster : CompAbilityEffect
    {
        private new CompProperties_SummonMechsForCaster Props => (CompProperties_SummonMechsForCaster)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            var caster = parent.pawn;

            var amountToSpawn = Props.amount;

            if (Props.cannotHaveMoreThanAmount)
            {
                var tmpMechsInAssignedOrder = new List<Pawn>();

                MechanitorUtility.GetMechsInAssignedOrder(caster, ref tmpMechsInAssignedOrder);
                amountToSpawn -= tmpMechsInAssignedOrder.Count;
            }

            for (var i = 0; i < amountToSpawn; i++)
            {
                var mechToSummon = PawnGenerator.GeneratePawn(Props.mechKind, caster.Faction);

                var pos = caster.Position.RandomAdjacentCell8Way();

                if (!Props.trySpawnHere.NullOrEmpty())
                {
                    pos = caster.Position + Props.trySpawnHere[i];
                }
                
                var map = caster.Map;

                if (!pos.Walkable(map))
                {
                    pos = caster.Position;
                }
                
                GenSpawn.Spawn(mechToSummon, pos, map);
                
                caster.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mechToSummon);
                
                mechToSummon.drafter.Drafted = true;
            }
        }
    }
}