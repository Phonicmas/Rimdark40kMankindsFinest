﻿using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI;

namespace Genes40k
{
    public class WorkGiver_FillGeneGestator : WorkGiver_Scanner
    {
        private static string NoGeneMatrix;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Genes40kDefOf.BEWH_GeneseedGestator);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public static void ResetStaticData()
        {
            NoGeneMatrix = "BEWH.MankindsFinest.GeneGestator.NoGeneMatrix".Translate();
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_GeneGestator building_GeneGestator) || building_GeneGestator.hasBeenStarted || building_GeneGestator.selectedMatrix == null || building_GeneGestator.containedMatrix != null || !building_GeneGestator.haulJobStarted)
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
            return JobMaker.MakeJob(Genes40kDefOf.BEWH_FillGeneGestator, t, thing);
        }

        private static Thing FindGeneMatrix(Pawn pawn, Building_GeneGestator gestator)
        {
            Predicate<Thing> validator = x => !x.IsForbidden(pawn) && pawn.CanReserve(x);
            var thing = GenClosest.ClosestThingReachable_NewTemp(pawn.Position, pawn.Map, ThingRequest.ForDef(gestator.selectedMatrix), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator, lookInHaulSources: true);

            return thing;
        }
    }
}