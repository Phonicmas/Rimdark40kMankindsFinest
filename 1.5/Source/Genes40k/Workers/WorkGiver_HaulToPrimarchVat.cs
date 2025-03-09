using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class WorkGiver_HaulToPrimarchVat : WorkGiver_Scanner
    {

        private const float NutritionBuffer = 2.5f;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_PrimarchGrowthVat);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
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
            if (!(t is Building_PrimarchGrowthVat building_GrowthVat))
            {
                return false;
            }
            if (building_GrowthVat.NutritionNeeded > NutritionBuffer && building_GrowthVat.containedEmbryo != null)
            {
                if (FindNutrition(pawn, building_GrowthVat).Thing != null)
                {
                    return true;
                }
                
                JobFailReason.Is("BEWH.MankindsFinest.PrimarchGrowthVat.NoSlurry".Translate());
                return false;
            }
            if (building_GrowthVat.selectedEmbryo != null && !building_GrowthVat.innerContainer.Contains(building_GrowthVat.selectedEmbryo))
            {
                return CanHaulSelectedThing(pawn, building_GrowthVat.selectedEmbryo);
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_PrimarchGrowthVat building_GrowthVat))
            {
                return null;
            }
            if (building_GrowthVat.NutritionNeeded > NutritionBuffer && building_GrowthVat.containedEmbryo != null)
            {
                var thingCount = FindNutrition(pawn, building_GrowthVat);
                if (thingCount.Thing != null)
                {
                    var job = HaulAIUtility.HaulToContainerJob(pawn, thingCount.Thing, t);
                    job.count = Mathf.Min(job.count, thingCount.Count);
                    return job;
                }
            }

            if (building_GrowthVat.selectedEmbryo == null ||
                building_GrowthVat.innerContainer.Contains(building_GrowthVat.selectedEmbryo) ||
                !CanHaulSelectedThing(pawn, building_GrowthVat.selectedEmbryo))
            {
                return null;
            }
            
            var job2 = JobMaker.MakeJob(Genes40kDefOf.BEWH_FillPrimarchGrowthVat, t, building_GrowthVat.selectedEmbryo);
            job2.count = 1;
            job2.haulMode = HaulMode.ToContainer;
            return job2;
        }

        private bool CanHaulSelectedThing(Pawn pawn, Thing selectedThing)
        {
            if (!selectedThing.Spawned || selectedThing.Map != pawn.Map)
            {
                return false;
            }
            return !selectedThing.IsForbidden(pawn) && pawn.CanReserveAndReach(selectedThing, PathEndMode.OnCell, Danger.Deadly, 1, 1);
        }

        private ThingCount FindNutrition(Pawn pawn, Building_PrimarchGrowthVat vat)
        {
            var thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(Genes40kDefOf.BEWH_RawGestationalSlurry), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, Validator);
            if (thing == null)
            {
                return default;
            }
            var b = Mathf.CeilToInt(vat.NutritionNeeded / thing.GetStatValue(StatDefOf.Nutrition));
            return new ThingCount(thing, Mathf.Min(thing.stackCount, b));
            bool Validator(Thing x)
            {
                if (x.IsForbidden(pawn) || !pawn.CanReserve(x))
                {
                    return false;
                }
                if (!vat.CanAcceptNutrition(x))
                {
                    return false;
                }
                
                return !(x.def.GetStatValueAbstract(StatDefOf.Nutrition) > vat.NutritionNeeded);
            }
        }
    }
}