using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_WordOfDivination : CompAbilityEffect
    {
        public new CompProperties_AbilityWordOfDivination Props => (CompProperties_AbilityWordOfDivination)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;

            pawn.ideo.SetIdeo(parent.pawn.Ideo);
        }
        
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (parent.pawn.Ideo == null)
            {
                return false;
            }

            if (target.Pawn.Ideo == parent.pawn.Ideo)
            {
                return false;
            }

            return base.Valid(target, throwMessages);
        }
        
        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (parent.pawn.Ideo == null)
            {
                return "BEWH.MankindsFinest.Ability.LorgarDivinationNoIdeo".Translate(parent.pawn);
            }
            
            return target.Pawn.Ideo == parent.pawn.Ideo ? "BEWH.MankindsFinest.Ability.LorgarDivination".Translate(target.Pawn, parent.pawn) : null;
        }
    }
}