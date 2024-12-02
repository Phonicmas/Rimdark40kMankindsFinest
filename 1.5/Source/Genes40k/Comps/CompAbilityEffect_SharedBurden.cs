using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_SharedBurden : CompAbilityEffect
    {
        public new CompProperties_AbilitySharedBurden Props => (CompProperties_AbilitySharedBurden)props;

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;
            return pawn.needs.mood.thoughts.memories.Memories.Any(memory => memory.MoodOffset() < 0) || pawn.InMentalState;
        }
        
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;

            if (pawn.InMentalState)
            {
                pawn.mindState.mentalStateHandler.CurState.RecoverFromState();
            }
            
            var pawnThoughts = pawn.needs.mood.thoughts;

            var negativeMoodThought = pawnThoughts.memories.Memories.Where(memory => memory.MoodOffset() < 0).ToList();

            if (!negativeMoodThought.Any())
            {
                return;
            }
            
            var worstMoodDebuff = negativeMoodThought.MinBy(memory => memory.MoodOffset());

            var angronThought = ThoughtMaker.MakeThought(Genes40kDefOf.BEWH_PrimarchSpecificXIIMood, null);

            angronThought.durationTicksOverride = worstMoodDebuff.DurationTicks;
            angronThought.moodOffset = (int)worstMoodDebuff.MoodOffset();
            
            pawn.needs.mood.thoughts.memories.RemoveMemory(worstMoodDebuff);
            
            parent.pawn.needs.mood.thoughts.memories.TryGainMemory(angronThought);
        }
        
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            base.Valid(target, throwMessages);
            if (target.Pawn == null)
            {
                return false;
            }

            return target.Pawn.needs.mood.thoughts.memories.Memories.Any(memory => memory.MoodOffset() < 0) || target.Pawn.InMentalState;
        }
        
        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (target.Pawn == null)
            {
                return null;
            }

            if (target.Pawn.needs.mood.thoughts.memories.Memories.Any(memory => memory.MoodOffset() >= 0) && !target.Pawn.InMentalState)
            {
                return "BEWH.ChillgronAbility".Translate(target.Pawn);
            }

            return null;
        }
    }
}