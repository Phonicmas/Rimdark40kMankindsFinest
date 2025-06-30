using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k;

public class JobDriver_CarryMatrixToGeneGestator : JobDriver
{

    private const int Duration = 200;

    private Building_GeneGestator GeneGestator => (Building_GeneGestator)job.GetTarget(TargetIndex.A).Thing;

    private Thing GeneMatrix => job.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (!pawn.Reserve(GeneGestator, job, 1, 1, null, errorOnFailed)) return false;
        if (!pawn.Reserve(GeneMatrix, job, 1, 1, null, errorOnFailed)) return false;
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        job.count = 1;
        var reserveGeneMatrix = Toils_Reserve.Reserve(TargetIndex.B, 1, 1);
        yield return reserveGeneMatrix;
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.B);
        yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveGeneMatrix, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);
        var toil = ToilMaker.MakeToil("MakeNewToils");
        toil.initAction = delegate
        {
            GeneGestator.AddGeneMatrix(GeneMatrix);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
    }
}