using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(TraitSet), "GainTrait")]
    public class SerfGainTrait
    {
        public static void Postfix(Pawn ___pawn, Trait trait)
        {
            if (trait.def != Genes40kDefOf.BEWH_Serf)
            {
                return;
            }

            if (___pawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_SerfBuff) == null)
            {
                ___pawn.health.AddHediff(Genes40kDefOf.BEWH_SerfBuff);
            }
        }
    }
}