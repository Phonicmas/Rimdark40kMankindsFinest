using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_CarryMaterialToSangprimus : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_SangprimusPortum);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_SangprimusPortum)
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

        return FindMaterial(pawn, forced) != null;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_SangprimusPortum building_SangprimusPortum)
        {
            return null;
        }

        var material = FindMaterial(pawn, forced);

        if (material == null)
        {
            return null;
        }

        var job = JobMaker.MakeJob(Genes40kDefOf.BEWH_CarryMaterialToSangprimus, building_SangprimusPortum, material);
        job.count = 1;
        return job;
    }

    /// <summary>
    /// Looks up each still-locked material def in the lister instead of walking every thing on the map.
    /// Re-run in JobOnThing so no state is kept on the shared worker instance.
    /// </summary>
    private static Thing FindMaterial(Pawn pawn, bool forced)
    {
        var gameComp = Current.Game?.GetComponent<GameComponent_UnlockedMaterials>();

        if (gameComp == null)
        {
            return null;
        }

        var listerThings = pawn.Map.listerThings;
        var maxDanger = pawn.NormalMaxDanger();

        foreach (var materialDef in Genes40kUtils.GeneMaterialDefs)
        {
            if (gameComp.HasMaterial(materialDef))
            {
                continue;
            }

            var things = listerThings.ThingsOfDef(materialDef);

            for (var i = 0; i < things.Count; i++)
            {
                var thing = things[i];

                if (thing.IsForbidden(pawn) || thing.IsBurning() || !pawn.CanReserveAndReach(thing, PathEndMode.ClosestTouch, maxDanger, 1, -1, null, forced))
                {
                    continue;
                }

                return thing;
            }
        }

        return null;
    }
}