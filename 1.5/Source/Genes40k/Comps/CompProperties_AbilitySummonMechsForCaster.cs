using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompProperties_AbilitySummonMechsForCaster : CompProperties_AbilityEffect
    {
        public PawnKindDef mechKind;

        public int amount = 1;

        public bool cannotHaveMoreThanAmount = false;

        public List<IntVec3> trySpawnHere = new List<IntVec3>();

        public CompProperties_AbilitySummonMechsForCaster()
        {
            compClass = typeof(CompAbilityEffect_SummonMechsForCaster);
        }
    }
}