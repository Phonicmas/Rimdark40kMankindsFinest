using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_HealAndTend : CompAbilityEffect
{
    public new CompProperties_AbilityHealAndTend Props => (CompProperties_AbilityHealAndTend)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
            
        var patient = target.Pawn;
            
        var injuries = patient.health.hediffSet.hediffs.Where(hediff => hediff is Hediff_Injury).Cast<Hediff_Injury>().ToList();
            
        foreach (var injury in injuries)
        {
            if (Props.healAmount != null)
            {
                injury.Heal(Props.healAmount.Value.RandomInRange);
            }
            var tendValue = parent.pawn?.GetStatValue(StatDefOf.MedicalTendQuality) ?? 0.75f;
            var buildingBed = patient.CurrentBed();
            if (buildingBed != null)
            {
                tendValue += buildingBed.GetStatValue(StatDefOf.MedicalTendQualityOffset);
            }
            tendValue = Mathf.Clamp(tendValue, 0f, Props.maxTendValue);
            injury.Tended(tendValue, Props.maxTendValue);
        }
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        base.Valid(target, throwMessages);
        if (target.Thing is not Pawn pawn)
        {
            return false;
        }

        return pawn.Downed || pawn.InBed();
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

        if (!pawn.Downed && !pawn.InBed())
        {
            return "BEWH.MankindsFinest.Ability.MustBeDownedOrInBed".Translate(pawn.NameShortColored);
        }

        return null;
    }

}