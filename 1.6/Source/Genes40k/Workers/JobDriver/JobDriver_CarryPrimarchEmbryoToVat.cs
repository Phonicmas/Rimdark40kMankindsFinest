using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k;

public class JobDriver_CarryPrimarchEmbryoToVat : JobDriver
{
    private const int Duration = 200;

    private bool inserted;

    // FIX: hard casts threw / mis-typed when the target had gone away or was a
    // stale duplicate left behind by the old save bug. "as" + null checks instead.
    private Building_PrimarchGrowthVat PrimarchGrowthVat => job.GetTarget(TargetIndex.A).Thing as Building_PrimarchGrowthVat;

    private PrimarchEmbryo Embryo => job.GetTarget(TargetIndex.B).Thing as PrimarchEmbryo;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inserted, "inserted", false);
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var vat = PrimarchGrowthVat;
        var embryo = Embryo;

        if (vat == null || embryo == null || embryo.Destroyed || !embryo.Spawned)
        {
            return false;
        }

        if (!pawn.Reserve(vat, job, 1, 1, null, errorOnFailed))
        {
            return false;
        }

        if (!pawn.Reserve(embryo, job, 1, 1, null, errorOnFailed))
        {
            return false;
        }

        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);

        // FIX: bail out cleanly if the vat was started, already filled, or the
        // embryo vanished mid-job, instead of running to the end and no-opping.
        this.FailOn(() =>
        {
            if (inserted)
            {
                return false;
            }

            var vat = PrimarchGrowthVat;
            return vat == null || vat.Working || vat.ContainedEmbryo != null || Embryo == null || Embryo.Destroyed;
        });

        job.count = 1;

        var reservedPrimarchEmbryo = Toils_Reserve.Reserve(TargetIndex.B, 1, 1);
        yield return reservedPrimarchEmbryo;

        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B)
            .FailOnSomeonePhysicallyInteracting(TargetIndex.B);

        yield return Toils_Haul.StartCarryThing(TargetIndex.B)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);

        yield return Toils_Haul.CheckForGetOpportunityDuplicate(reservedPrimarchEmbryo, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);

        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);

        yield return Toils_General.Wait(Duration)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);

        var toil = ToilMaker.MakeToil("InsertPrimarchEmbryo");
        toil.initAction = delegate
        {
            var vat = PrimarchGrowthVat;
            var embryo = Embryo;

            if (vat == null || embryo == null || embryo.Destroyed)
            {
                EndJobWith(JobCondition.Incompletable);
                return;
            }

            inserted = true;
            vat.InsertEmbryo(embryo);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
    }
}
