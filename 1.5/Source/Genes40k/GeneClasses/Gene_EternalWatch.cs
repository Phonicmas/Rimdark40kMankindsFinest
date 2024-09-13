using Verse;


namespace Genes40k
{
    public class Gene_EternalWatch : Gene
    {
        public MentalBreakDef mentalBreak;

        public MentalStateDef stateDef;

        private string reason;

        private bool causedByMood;

        private Pawn otherPawn;

        private bool transitionSilently;

        private bool causedByDamage;

        private bool causedByPsycast;

        public void SetMentalState(MentalStateDef stateDef, string reason = null, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false, bool causedByDamage = false, bool causedByPsycast = false)
        {
            this.reason = reason;
            this.stateDef = stateDef;
            mentalBreak = null;
            this.causedByMood = causedByMood;
            this.otherPawn = otherPawn;
            this.transitionSilently = transitionSilently;
            this.causedByDamage = causedByDamage;
            this.causedByPsycast = causedByPsycast;
        }

        public void SetMentalBreak(MentalBreakDef mentalBreak, string reason, bool causedByMood)
        {
            this.reason = reason;
            this.mentalBreak = mentalBreak;
            this.causedByMood = causedByMood;
            stateDef = null;
            otherPawn = null;
            transitionSilently = false;
            causedByDamage = false;
            causedByPsycast = false;
        }

        public void TryDoMentalBreak()
        {
            if (stateDef != null)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(stateDef, reason, forced: false, forceWake: false, causedByMood: causedByMood, otherPawn, transitionSilently, causedByDamage, causedByPsycast);
            }
            else if (mentalBreak != null)
            {
                mentalBreak.Worker.TryStart(pawn, reason, causedByMood);
            }
            else
            {
                return;
            }
            ResetData();
        }

        private void ResetData()
        {
            stateDef = null;
            mentalBreak = null;
            reason = null;
            causedByMood = false;
            otherPawn = null;
            transitionSilently = false;
            causedByDamage = false;
            causedByPsycast = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref mentalBreak, "mentalBreak");
            Scribe_Defs.Look(ref stateDef, "stateDef");
            Scribe_Values.Look(ref reason, "reason");
            Scribe_Values.Look(ref causedByMood, "causedByMood", false);
            Scribe_References.Look(ref otherPawn, "otherPawn");
            Scribe_Values.Look(ref transitionSilently, "transitionSilently", false);
            Scribe_Values.Look(ref causedByDamage, "causedByDamage", false);
            Scribe_Values.Look(ref causedByPsycast, "causedByPsycast", false);
        }

    }
}