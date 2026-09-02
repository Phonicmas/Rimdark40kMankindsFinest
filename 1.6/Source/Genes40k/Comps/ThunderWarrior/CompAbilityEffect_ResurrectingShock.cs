using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_ResurrectingShock : CompAbilityEffect
{
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
            
        if (target.Thing is not Corpse corpse || corpse.InnerPawn == null)
        {
            return;
        }

        var timeDead = corpse.Age;

        var pawn = corpse.InnerPawn;

        var resurrectionParams = new ResurrectionParams
        {
            restoreMissingParts = false
        };

        ResurrectionUtility.TryResurrect(pawn, resurrectionParams);
        
        if (timeDead <= 5000)
        {
            return;
        }
        
        var brain = pawn.health.hediffSet.GetBrain();
        if (brain == null)
        {
            return;
        }
        
        var num = Rand.RangeInclusive(2, 5);
        pawn.TakeDamage(new DamageInfo(DamageDefOf.Flame, num, 0f, -1f, null, brain));
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (!base.Valid(target, throwMessages))
        {
            return false;
        }
        
        return target.Thing is Corpse corpse && corpse.InnerPawn?.genes?.HasActiveGene(Genes40kDefOf.BEWH_Vigoranis) == true;
    }

    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (base.ExtraLabelMouseAttachment(target) != null)
        {
            return base.ExtraLabelMouseAttachment(target);
        }
            
        if (target.Thing is not Corpse corpse)
        {
            return "BEWH.MankindsFinest.Ability.MustBeCorpse".Translate();
        }

        if (corpse.InnerPawn?.genes?.HasActiveGene(Genes40kDefOf.BEWH_Vigoranis) != true)
        {
            return "BEWH.MankindsFinest.Ability.MustHaveVigoranis".Translate(corpse.InnerPawn.NameShortColored, Genes40kDefOf.BEWH_Vigoranis);
        }

        return null;
    }

}