using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class Building_GeneTable : Building_WorkTable
    {

        private int tickAmount = 0;

        private const int tickAmountDrain = 1000;

        private Pawn workingPawn = null;

        private const float psyfocusDrain = -0.05f;

        private const float severityAdd = 0.1f;

        public Building_GeneTable()
        {
            billStack = new BillStack(this);
        }

        public void PawnDidWork(Pawn p)
        {
            if (workingPawn != p && p != null)
            {
                workingPawn = p;
            }
        }

        public override void UsedThisTick()
        {
            base.UsedThisTick();
            if (workingPawn == null)
            {
                return;
            }
            tickAmount++;
            if (tickAmount < tickAmountDrain) return;
            
            if (ModsConfig.RoyaltyActive)
            {
                if (workingPawn.psychicEntropy.CurrentPsyfocus >= Math.Abs(psyfocusDrain))
                {
                    workingPawn.psychicEntropy.OffsetPsyfocusDirectly(psyfocusDrain);
                }
                else
                {
                    workingPawn.psychicEntropy.OffsetPsyfocusDirectly(workingPawn.psychicEntropy.CurrentPsyfocus * -1);
                    DoComaHediff();
                }
            }
            else
            {
                DoComaHediff();
            }
            tickAmount = 0;
        }

        private void DoComaHediff()
        {
            if (workingPawn == null)
            {
                return;
            }
            var hediff = workingPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_PsychicCrafting);
            if (hediff == null)
            {
                workingPawn.health.AddHediff(Genes40kDefOf.BEWH_PsychicCrafting);
            }
            else
            {
                if (hediff.Severity + severityAdd >= 3f)
                {
                    workingPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    workingPawn.jobs.ClearQueuedJobs(false);
                }
                hediff.Severity += severityAdd;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickAmount, "tickAmount", 0);
            Scribe_References.Look(ref workingPawn, "workingPawn");
        }
    }
}