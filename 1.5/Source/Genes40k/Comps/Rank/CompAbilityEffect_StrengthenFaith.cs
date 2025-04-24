using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_StrengthenFaith : CompAbilityEffect
{
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
            
        target.Pawn.ideo.OffsetCertainty(99999);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (!base.Valid(target, throwMessages))
        {
            return false;
        }

        if (parent.pawn.Ideo == null)
        {
            return false;
        }

        if (target.Pawn?.Ideo == null)
        {
            return false;
        }
            
        return parent.pawn.Ideo == target.Pawn.Ideo;
    }

    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (base.ExtraLabelMouseAttachment(target) != null)
        {
            return base.ExtraLabelMouseAttachment(target);
        }

        if (parent.pawn.Ideo == null)
        {
            return "BEWH.MankindsFinest.Ability.MustHaveIdeo".Translate(parent.pawn);       
        }

        if (target.Pawn != null)
        {
            if (target.Pawn.Ideo == null)
            {
                return "BEWH.MankindsFinest.Ability.MustHaveIdeo".Translate(target.Pawn);       
            }
                
            if (parent.pawn.Ideo != target.Pawn.Ideo)
            {
                return "BEWH.MankindsFinest.Ability.MustHaveSameIdeo".Translate(target.Pawn, parent.pawn);
            }
        }

        return null;
    }

}