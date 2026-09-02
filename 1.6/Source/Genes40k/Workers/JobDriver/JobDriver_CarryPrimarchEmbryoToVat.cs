using Verse;
using Verse.AI;

namespace Genes40k;

public class JobDriver_CarryPrimarchEmbryoToVat : JobDriver_CarryThingToBuilding
{
    private bool inserted;

    //"as" rather than a hard cast: the target can have gone away or be a stale duplicate.
    private Building_PrimarchGrowthVat PrimarchGrowthVat => Building as Building_PrimarchGrowthVat;

    private PrimarchEmbryo Embryo => CarriedThing as PrimarchEmbryo;

    protected override string ArrivalToilLabel => "InsertPrimarchEmbryo";

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inserted, "inserted", false);
    }

    protected override bool TargetsValid()
    {
        var embryo = Embryo;

        return PrimarchGrowthVat != null && embryo != null && !embryo.Destroyed && embryo.Spawned;
    }

    protected override void AddExtraFailConditions()
    {
        //Bail out cleanly if the vat was started, already filled, or the embryo vanished mid-job,
        //instead of running to the end and no-opping.
        this.FailOn(() =>
        {
            if (inserted)
            {
                return false;
            }

            var vat = PrimarchGrowthVat;
            return vat == null || vat.Working || vat.ContainedEmbryo != null || Embryo == null || Embryo.Destroyed;
        });
    }

    protected override Toil GotoBuildingToil()
    {
        return base.GotoBuildingToil().FailOnDestroyedNullOrForbidden(TargetIndex.B);
    }

    protected override void OnArrived()
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
    }
}