using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_CarryPrimarchEmbryoToVat : WorkGiver_Scanner
{
    // NOTE: was a static readonly field. Static readonly + Translate() caches the
    // string for the whole session and goes stale if the language is changed.
    private static string NoPrimarchEmbryo => "BEWH.MankindsFinest.PrimarchGrowthVat.NoPrimarchEmbryo".Translate();

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_PrimarchGrowthVat);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PrimarchGrowthVat vat)
        {
            return false;
        }

        if (vat.Working || vat.ContainedEmbryo != null)
        {
            return false;
        }

        var embryo = vat.selectedEmbryo;
        if (embryo == null)
        {
            return false;
        }

        // FIX: a selection that can no longer be hauled used to keep returning true
        // here forever. JobOnThing then handed out a job that died on its first toil,
        // so every hauler (and every VQE hauler drone) re-took it every tick:
        //   "VQE_HaulerDrone... started 10 jobs in one tick. newJob=BEWH_CarryPrimarchEmbryoToVat"
        // Clear the dead selection instead so the vat goes back to offering
        // "Implant embryo..." in its gizmo bar.
        if (embryo.Destroyed || !embryo.Spawned || embryo.MapHeld != vat.Map)
        {
            vat.selectedEmbryo = null;
            return false;
        }

        if (vat.IsBurning() || vat.IsForbidden(pawn))
        {
            return false;
        }

        if (pawn.Map.designationManager.DesignationOn(vat, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        if (!pawn.CanReserve(vat, 1, 1, null, forced))
        {
            return false;
        }

        if (embryo.IsForbidden(pawn))
        {
            JobFailReason.Is(NoPrimarchEmbryo);
            return false;
        }

        // FIX: this is the check vanilla's WorkGiver_HaulToGrowthVat does
        // (CanHaulSelectedThing). Without it the work giver happily hands out jobs
        // for embryos that are unreachable, already reserved by another hauler,
        // or held by a pawn.
        if (!pawn.CanReserveAndReach(embryo, PathEndMode.ClosestTouch, forced ? Danger.Deadly : pawn.NormalMaxDanger(), 1, 1, null, forced))
        {
            JobFailReason.Is(NoPrimarchEmbryo);
            return false;
        }

        return true;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PrimarchGrowthVat vat || vat.selectedEmbryo == null)
        {
            return null;
        }

        var job = JobMaker.MakeJob(Genes40kDefOf.BEWH_CarryPrimarchEmbryoToVat, vat, vat.selectedEmbryo);
        job.count = 1;
        return job;
    }
}
