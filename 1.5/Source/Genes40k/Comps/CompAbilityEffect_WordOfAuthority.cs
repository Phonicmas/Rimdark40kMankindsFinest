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
            base.Valid(target, throwMessages);
            return !target.Pawn.Faction.IsPlayer;
        }
        
        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return target.Pawn.Faction.IsPlayer ? "BEWH.LorgarAuthority".Translate(target.Pawn, parent.pawn) : null;
        }
    }
}