using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Genes40k
{
    public class Ability_SteelRain : VFECore.Abilities.Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (var globalTargetInfo in targets)
            {
                var list = GenRadial.RadialCellsAround(globalTargetInfo.Cell, GetRadiusForPawn(), useCenter: true).Where(c => c.InBounds(pawn.Map) && !c.Fogged(pawn.Map) && c.GetEdifice(pawn.Map) == null).ToList();
                
                var firstSpawnCell = list.RandomElement();
                
                var secondSpawnCell = list.Where(cell => cell.DistanceTo(firstSpawnCell) > 5).RandomElement();
                
                SpawnSkyfaller(firstSpawnCell);
                SpawnSkyfaller(secondSpawnCell);
            }
        }
        
        private void SpawnSkyfaller(IntVec3 cell)
        {
            var innerThing = ThingMaker.MakeThing(Genes40kDefOf.BEWH_SteelRainDropPodBuilding);
            SkyfallerMaker.SpawnSkyfaller(Genes40kDefOf.BEWH_SteelRainDropPodSkyfaller, innerThing, cell, pawn.Map);
        }

    }
}