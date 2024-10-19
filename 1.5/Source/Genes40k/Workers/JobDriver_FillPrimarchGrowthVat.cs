﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class JobDriver_FillPrimarchGrowthVat : JobDriver
    {

        private const int Duration = 200;

        protected Building_PrimarchGrowthVat PrimarchGrowthVat => (Building_PrimarchGrowthVat)job.GetTarget(TargetIndex.A).Thing;

        protected Thing PriamrchEmbryo => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(PrimarchGrowthVat, job, 1, 1, null, errorOnFailed))
            {
                if (pawn.Reserve(PriamrchEmbryo, job, 1, 1, null, errorOnFailed))
                {
                    PrimarchGrowthVat.jobDoer = pawn;
                    return true;
                }
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            job.count = 1;
            Toil reservedPrimarchEmbryo = Toils_Reserve.Reserve(TargetIndex.B, 1, 1);
            yield return reservedPrimarchEmbryo;
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.B);
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(reservedPrimarchEmbryo, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .WithProgressBarToilDelay(TargetIndex.A);
            Toil toil = ToilMaker.MakeToil("MakeNewToils");
            toil.initAction = delegate
            {
                PrimarchGrowthVat.AddPrimarcEmbryo(PriamrchEmbryo);
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil;
        }
    }
}