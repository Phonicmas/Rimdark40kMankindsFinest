using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_CarryPrimarchEmbryoToVat : WorkGiver_Scanner
{
    private static readonly string NoPrimarchEmbryo = "BEWH.MankindsFinest.PrimarchGrowthVat.NoPrimarchEmbryo".Translate();

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_PrimarchGrowthVat);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PrimarchGrowthVat building_PrimarchGrowthVat || building_PrimarchGrowthVat.Working || building_PrimarchGrowthVat.selectedEmbryo == null || building_PrimarchGrowthVat.ContainedEmbryo != null)
        {
            return false;
        }
        if (building_PrimarchGrowthVat.IsForbidden(pawn) || !pawn.CanReserve(building_PrimarchGrowthVat, 1, 1, null, forced))
        {
            return false;
        }
        if (pawn.Map.designationManager.DesignationOn(building_PrimarchGrowthVat, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }
        
        return !building_PrimarchGrowthVat.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building_PrimarchGrowthVat = (Building_PrimarchGrowthVat)t;
        return JobMaker.MakeJob(Genes40kDefOf.BEWH_CarryPrimarchEmbryoToVat, t, building_PrimarchGrowthVat.selectedEmbryo);
    }
}