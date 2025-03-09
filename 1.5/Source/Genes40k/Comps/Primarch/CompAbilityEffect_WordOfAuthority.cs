using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_WordOfAuthority : CompAbilityEffect
    {
        public new CompProperties_AbilityWordOfAuthority Props => (CompProperties_AbilityWordOfAuthority)props;
        
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;

            pawn.SetFaction(Faction.OfPlayer, parent.pawn);
        }
        
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (target.Pawn?.Faction == null)
            {
                return false;
            }
            
            return !target.Pawn.Faction.IsPlayer && base.Valid(target, throwMessages);
        }
        
        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (target.Pawn?.Faction == null)
            {
                return null;
            }
            
            return target.Pawn.Faction.IsPlayer ? "BEWH.MankindsFinest.Ability.LorgarAuthority".Translate(target.Pawn, parent.pawn) : null;
        }
    }
}