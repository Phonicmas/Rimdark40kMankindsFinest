using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompProperties_SummonMechsForCaster : CompProperties_AbilityEffect
    {
        public PawnKindDef mechKind;

        public int amount = 1;

        public bool cannotHaveMoreThanAmount = false;

        public List<IntVec3> trySpawnHere = new List<IntVec3>();

        public CompProperties_SummonMechsForCaster()
        {
            compClass = typeof(CompAbilityEffect_SummonMechsForCaster);
        }
    }
}