using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_AoeHit : CompAbilityEffect
    {
        public new CompProperties_AbilityAoeHit Props => (CompProperties_AbilityAoeHit)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;
            if (pawn == null)
            {
                return;
            }
            
            var damageAmount = Props.damageAmount;

            if (Props.scaleStat != null)
            {
                var stat = parent.pawn.GetStatValue(Props.scaleStat) * Props.scaleFactor;
                damageAmount *= stat;
            }

            var dInfo = new DamageInfo(Props.damageDef, damageAmount);
            
            pawn.TakeDamage(dInfo);
        }
    }
}