using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_SearWounds : CompAbilityEffect
{
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
            
        var patient = target.Pawn;
            
        var injuries = patient.health.hediffSet.hediffs.Where(hediff => hediff is Hediff_Injury { Bleeding: true }).Cast<Hediff_Injury>().ToList();
            
        foreach (var injury in injuries)
        {
            injury.Heal(-1);
            injury.Tended(0.1f, 0.1f);
        }
        
        var missingLimbs = patient.health.hediffSet.hediffs.Where(hediff => hediff is Hediff_MissingPart { Bleeding: true }).Cast<Hediff_MissingPart>().ToList();
        
        var torso = patient.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso);
        var dInfo = new DamageInfo(DamageDefOf.Crush, 1, 99999f, hitPart: torso);
        
        foreach (var missingLimb in missingLimbs)
        {
            patient.TakeDamage(dInfo);
            missingLimb.Tended(0.1f, 0.1f);
        }
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        base.Valid(target, throwMessages);
        
        return target.Thing is Pawn pawn && pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Injury { Bleeding: true } or Hediff_MissingPart { Bleeding: true });
    }

    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        if (base.ExtraLabelMouseAttachment(target) != null)
        {
            return base.ExtraLabelMouseAttachment(target);
        }
            
        if (target.Thing is not Pawn pawn)
        {
            return "BEWH.MankindsFinest.Ability.MustBePawn".Translate();
        }

        if (!pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Injury { Bleeding: true } or Hediff_MissingPart { Bleeding: true }))
        {
            return "BEWH.MankindsFinest.Ability.NoBleedingWounds".Translate(pawn.NameShortColored);
        }

        var torso = pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso);
        var torsoHealth = pawn.health.hediffSet.GetPartHealth(torso);
        if (pawn.health.hediffSet.hediffs.Count(hediff => hediff is Hediff_MissingPart { Bleeding: true }) >= torsoHealth)
        {
            return "BEWH.MankindsFinest.Ability.WouldKillPawn".Translate();
        }

        return null;
    }

}