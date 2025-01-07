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
                var possibleCells = GenRadial.RadialCellsAround(globalTargetInfo.Cell, GetRadiusForPawn(), useCenter: true).Where(c => c.InBounds(pawn.Map) && !c.Fogged(pawn.Map)).ToList();

                var cellsToSpawn = new List<IntVec3>();
                var initialCell = possibleCells.Where(c => c.GetEdifice(pawn.Map) == null).RandomElement();
                
                cellsToSpawn.Add(initialCell);
                possibleCells.Remove(initialCell);
                
                for (var i = 0; i < def.power-1; i++)
                {
                    var spawnCell = possibleCells.Where(c => cellsToSpawn.All(c2 => c2.DistanceTo(c) > 5) && c.GetEdifice(pawn.Map) == null).RandomElement();
                    cellsToSpawn.Add(spawnCell);
                    possibleCells.Remove(spawnCell);
                }

                foreach (var cell in cellsToSpawn)
                {
                    SpawnSkyfaller(cell);
                }
            }
        }
        
        private void SpawnSkyfaller(IntVec3 cell)
        {
            var innerThing = ThingMaker.MakeThing(Genes40kDefOf.BEWH_SteelRainDropPodBuilding);
            SkyfallerMaker.SpawnSkyfaller(Genes40kDefOf.BEWH_SteelRainDropPodSkyfaller, innerThing, cell, pawn.Map);
        }

    }
}