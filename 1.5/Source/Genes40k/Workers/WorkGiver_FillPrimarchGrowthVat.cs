using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class WorkGiver_FillPrimarchGrowthVat : WorkGiver_Scanner
    {
        private static string NoPrimarchEmbryo;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_PrimarchGrowthVat);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public static void ResetStaticData()
        {
            NoPrimarchEmbryo = "BEWH.MankindsFinest.PrimarchGrowthVat.NoPrimarchEmbryo".Translate();
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_PrimarchGrowthVat building_PrimarchGrowthVat) || building_PrimarchGrowthVat.hasBeenStarted || building_PrimarchGrowthVat.selectedEmbryo == null || building_PrimarchGrowthVat.containedEmbryo != null || !building_PrimarchGrowthVat.haulJobStarted)
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

            if (FindPrimarchEmbryo(pawn, building_PrimarchGrowthVat) != null)
            {
                return !building_PrimarchGrowthVat.IsBurning();
            }
            
            JobFailReason.Is(NoPrimarchEmbryo);
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var building_PrimarchGrowthVat = (Building_PrimarchGrowthVat)t;
            var thing = FindPrimarchEmbryo(pawn, building_PrimarchGrowthVat);
            return JobMaker.MakeJob(Genes40kDefOf.BEWH_FillPrimarchGrowthVat, t, thing);
        }

        private static Thing FindPrimarchEmbryo(Pawn pawn, Building_PrimarchGrowthVat building_PrimarchGrowthVat)
        {
            Predicate<Thing> validator = x => !x.IsForbidden(pawn) && pawn.CanReserve(x);
            var thing = GenClosest.ClosestThingReachable_NewTemp(pawn.Position, pawn.Map, ThingRequest.ForDef(building_PrimarchGrowthVat.selectedEmbryo.def), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator, lookInHaulSources: true);
            
            return thing;
        }
    }
}