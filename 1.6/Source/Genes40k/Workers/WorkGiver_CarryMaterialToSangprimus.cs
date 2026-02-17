using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_CarryMaterialToSangprimus : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_SangprimusPortum);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public Thing thingToCarry = null;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_SangprimusPortum building_SangprimusPortum)
        {
            return false;
        }
        if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
        {
            return false;
        }
        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }
        if (t.IsBurning())
        {
            return false;
        }
        
        foreach (var geneMaterial in t.Map.listerThings.GetThingsOfType<GeneMaterialExtra>())
        {
            if (!building_SangprimusPortum.CanAcceptMaterial(geneMaterial))
            {
                continue;
            }
            if (geneMaterial.IsForbidden(pawn) || !pawn.CanReserve(geneMaterial, 1, -1, null, forced))
            {
                continue;
            }
            if (geneMaterial.IsBurning())
            {
                continue;
            }
            
            thingToCarry = geneMaterial;
            return true;
        }

        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_SangprimusPortum building_SangprimusPortum)
        {
            return null;
        }

        if (thingToCarry != null)
        {
            var job = JobMaker.MakeJob(Genes40kDefOf.BEWH_CarryMaterialToSangprimus, building_SangprimusPortum, thingToCarry);
            job.count = 1;
            return job;
        }
        
        return null;
    }
}