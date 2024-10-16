﻿using RimWorld;
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
            NoPrimarchEmbryo = "BEWH.NoPrimarchEmbryo".Translate();
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_PrimarchGrowthVat building_PrimarchGrowthVat) || building_PrimarchGrowthVat.hasBeenStarted || building_PrimarchGrowthVat.selectedEmbryo == null || building_PrimarchGrowthVat.containedEmbryo != null || !building_PrimarchGrowthVat.haulJobStarted)
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
            if (FindPrimarchEmbryo(pawn, building_PrimarchGrowthVat) == null)
            {
                JobFailReason.Is(NoPrimarchEmbryo);
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_PrimarchGrowthVat building_PrimarchGrowthVat = (Building_PrimarchGrowthVat)t;
            Thing thing = FindPrimarchEmbryo(pawn, building_PrimarchGrowthVat);
            return JobMaker.MakeJob(Genes40kDefOf.BEWH_FillPrimarchGrowthVat, t, thing);
        }

        private Thing FindPrimarchEmbryo(Pawn pawn, Building_PrimarchGrowthVat building_PrimarchGrowthVat)
        {
            Predicate<Thing> validator = (Thing x) => (!x.IsForbidden(pawn) && pawn.CanReserve(x)) ? true : false;
            Thing thing = GenClosest.ClosestThingReachable_NewTemp(pawn.Position, pawn.Map, ThingRequest.ForDef(building_PrimarchGrowthVat.selectedEmbryo.def), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator, lookInHaulSources: true);
            if (thing == null && building_PrimarchGrowthVat.Map.listerBuildings.ColonistsHaveBuilding(Genes40kDefOf.BEWH_PrimarchEmbryoContainer))
            {
                foreach (Building_GeneStorage building in building_PrimarchGrowthVat.Map.listerBuildings.AllBuildingsColonistOfDef(Genes40kDefOf.BEWH_PrimarchEmbryoContainer))
                {
                    thing = building.SearchableContents.Where(x => x == building_PrimarchGrowthVat.selectedEmbryo).FirstOrDefault();
                    if (thing != null)
                    {
                        building.DropThingToReserve(thing);
                        break;
                    }
                }
            }
            return thing;
        }
    }
}