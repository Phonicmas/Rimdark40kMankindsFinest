using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k;

/// <summary>
/// Shared hauling driver: reserve a thing, carry it to a building, then hand it over. The toil
/// sequence must stay exactly seven toils in this order - curToilIndex is persisted, so a save made
/// mid-job would resume at the wrong step if the count or order changed.
/// </summary>
public abstract class JobDriver_CarryThingToBuilding : JobDriver
{
    protected const int Duration = 200;

    protected Thing Building => job.GetTarget(TargetIndex.A).Thing;

    protected Thing CarriedThing => job.GetTarget(TargetIndex.B).Thing;

    protected virtual string ArrivalToilLabel => "MakeNewToils";

    /// <summary>
    /// Runs once the hauler has carried the thing all the way to the building.
    /// </summary>
    protected abstract void OnArrived();

    /// <summary>
    /// Up-front validity beyond simply having both targets, checked before either is reserved.
    /// </summary>
    protected virtual bool TargetsValid()
    {
        return Building != null && CarriedThing != null;
    }

    /// <summary>
    /// Extra fail conditions installed on the driver before any toil runs.
    /// </summary>
    protected virtual void AddExtraFailConditions()
    {
    }

    protected virtual Toil GotoBuildingToil()
    {
        return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (!TargetsValid())
        {
            return false;
        }

        if (!pawn.Reserve(Building, job, 1, 1, null, errorOnFailed))
        {
            return false;
        }

        if (!pawn.Reserve(CarriedThing, job, 1, 1, null, errorOnFailed))
        {
            return false;
        }

        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);

        AddExtraFailConditions();

        job.count = 1;

        var reserveCarriedThing = Toils_Reserve.Reserve(TargetIndex.B, 1, 1);
        yield return reserveCarriedThing;

        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B)
            .FailOnSomeonePhysicallyInteracting(TargetIndex.B);

        yield return Toils_Haul.StartCarryThing(TargetIndex.B)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);

        yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveCarriedThing, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);

        yield return GotoBuildingToil();

        yield return Toils_General.Wait(Duration)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);

        var toil = ToilMaker.MakeToil(ArrivalToilLabel);
        toil.initAction = OnArrived;
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
    }
}