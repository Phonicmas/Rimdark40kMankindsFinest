using Verse;


namespace Genes40k
{
    public class CompGivesAbility : ThingComp
    {
        private CompProperties_GivesAbility Props => (CompProperties_GivesAbility)props;

        public override void Notify_Equipped(Pawn pawn)
        {
            pawn.abilities.GainAbility(Props.ability);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            pawn.abilities.RemoveAbility(Props.ability);
        }
    }
}