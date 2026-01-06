using RimWorld.Planet;
using VEF.Abilities;
using Verse;

namespace Genes40k;

public class AbilityExtension_PsyfocusCost : AbilityExtension_AbilityMod
{
    public float psyfocusCost;
    public float entropyGain;
    
    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        if (ability.pawn.psychicEntropy.PsychicSensitivity < float.Epsilon)
        {
            reason = "CommandPsycastZeroPsychicSensitivity".Translate();
            return false;
        }
        if (ability.pawn.psychicEntropy.CurrentPsyfocus < psyfocusCost)
        {
            reason = "CommandPsycastNotEnoughPsyfocus".Translate(psyfocusCost.ToStringPercent("#.0"), ability.pawn.psychicEntropy.CurrentPsyfocus.ToStringPercent("#.0"), ability.def.label.Named("PSYCASTNAME"), ability.pawn.Named("CASTERNAME"));
            return false;
        }
        if (ability.pawn.psychicEntropy.WouldOverflowEntropy(entropyGain))
        {
            reason = "CommandPsycastWouldExceedEntropy".Translate(ability.def.label);
            return false;
        }
        
        reason = string.Empty;
        return true;
    }
    
    public override void Cast(GlobalTargetInfo[] targets, Ability ability)
    {
        base.Cast(targets, ability);
        ability.pawn.psychicEntropy.TryAddEntropy(entropyGain);
        ability.pawn.psychicEntropy.OffsetPsyfocusDirectly(0f - psyfocusCost);
    }
}