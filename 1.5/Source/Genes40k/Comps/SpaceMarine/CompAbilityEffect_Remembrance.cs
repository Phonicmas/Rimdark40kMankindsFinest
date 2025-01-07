using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompAbilityEffect_Remembrance : CompAbilityEffect
    {
        private new CompProperties_AbilityRemembrance Props => (CompProperties_AbilityRemembrance)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = parent.pawn;

            if (!(target.Thing is Corpse corpse))
            {
                return;
            }

            foreach (var allDef in DefDatabase<SkillDef>.AllDefs)
            {
                var pawnSkill = pawn.skills.GetSkill(allDef);
                var corpseSkill = corpse.InnerPawn.skills.GetSkill(allDef);

                var xpToGive = (corpseSkill.XpTotalEarned * 0.1f);

                pawnSkill.Learn(xpToGive);
            }
            corpse.Strip();
            corpse.Destroy();
        }
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var corpse = target.Thing as Corpse;
            return corpse != null && corpse.InnerPawn.RaceProps.Humanlike;
        }

    }
}