using RimWorld;
using Verse;

namespace Genes40k;

public class CompAbilityEffect_ProofOfFaith : CompAbilityEffect_WithDuration
{
    private new CompProperties_AbilityProofOfFaith Props => (CompProperties_AbilityProofOfFaith)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        var pawn = target.Pawn;
        if (pawn == null)
        {
            return;
        }
            
        var gainedAmount = Props.divineGraceGain;
        gainedAmount *= pawn.GetStatValue(Props.durationMultiplier);
        Genes40kUtils.OffsetDivineGrace(parent.pawn, gainedAmount);
            
        base.Apply(target, dest);
    }
        
    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (!base.Valid(target, throwMessages))
        {
            return false;
        }
            
        var pawn = target.Pawn;

        if (pawn != null && !pawn.Faction.IsPlayer)
        {
            return false;
        }
            
        var requiredStat = pawn?.GetStatValue(Props.durationMultiplier);
            
        return requiredStat > 0;
    }
        
    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        var pawn = target.Pawn;
        if (pawn == null)
        {
            return null;
        }
            
        var requiredStat = pawn.GetStatValue(Props.durationMultiplier);

        if (requiredStat <= 0)
        {
            return "BEWH.MankindsFinest.Ability.ProofOfFaithNoStat".Translate(pawn, Props.durationMultiplier.label);
        }
            
        var gainedAmount = Props.divineGraceGain;
        gainedAmount *= pawn.GetStatValue(Props.durationMultiplier);

        return "BEWH.MankindsFinest.Ability.ProofOfFaithGain".Translate(pawn, gainedAmount * 100, requiredStat.ToString("0.0"));
    }
}