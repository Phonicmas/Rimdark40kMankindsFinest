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

            pawn.ideo.SetIdeo(parent.pawn.ideo.Ideo);
        }
        
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            base.Valid(target, throwMessages);
            if (parent.pawn.ideo?.Ideo == null)
            {
                return false;
            }

            return target.Pawn.ideo.Ideo != parent.pawn.ideo.Ideo;
        }
        
        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (parent.pawn.ideo?.Ideo == null)
            {
                return "BEWH.LorgarDivinationNoIdeo".Translate(parent.pawn);
            }
            
            return target.Pawn.ideo.Ideo == parent.pawn.ideo.Ideo ? "BEWH.LorgarDivination".Translate(target.Pawn, parent.pawn) : null;
        }
    }
}