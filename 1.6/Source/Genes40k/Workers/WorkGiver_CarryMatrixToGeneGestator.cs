using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_CarryMatrixToGeneGestator : WorkGiver_Scanner
{
    private static readonly string NoGeneMatrix = "BEWH.MankindsFinest.GeneGestator.ContainsNoGeneMatrix".Translate();

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_GeneseedGestator);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_GeneGestator building_GeneGestator || building_GeneGestator.selectedMatrix == null || building_GeneGestator.containedMatrix != null)
        {
            return false;
        }
        if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, 1, null, forced))
        {
            return false;
        }
        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        if (FindGeneMatrix(pawn, building_GeneGestator) != null)
        {
            return !t.IsBurning();
        }
            
        JobFailReason.Is(NoGeneMatrix);
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building_GeneGestator = (Building_GeneGestator)t;
        var thing = FindGeneMatrix(pawn, building_GeneGestator);
        return JobMaker.MakeJob(Genes40kDefOf.BEWH_CarryMatrixToGeneGestator, t, thing);
    }

    private static Thing FindGeneMatrix(Pawn pawn, Building_GeneGestator gestator)
    {
        var thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(gestator.selectedMatrix), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, Validator, lookInHaulSources: true);

        return thing;
        bool Validator(Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x);
    }
}