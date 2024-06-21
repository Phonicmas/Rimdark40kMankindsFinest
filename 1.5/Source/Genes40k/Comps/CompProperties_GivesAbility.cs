using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompProperties_GivesAbility : CompProperties
    {
        public AbilityDef ability;

        public CompProperties_GivesAbility()
        {
            compClass = typeof(CompGivesAbility);
        }
    }
}