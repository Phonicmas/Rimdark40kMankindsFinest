using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Ability_SteelRain : VFECore.Abilities.Ability
{
    private DefModExtension_DropPod defMod = null;
        
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

            defMod = def.GetModExtension<DefModExtension_DropPod>();
                
            for (var i = 0; i < defMod.dropPodAmount-1; i++)
            {
                var spawnCell = possibleCells.Where(c => cellsToSpawn.All(c2 => c2.DistanceTo(c) > 5) && c.GetEdifice(pawn.Map) == null).RandomElement();
                cellsToSpawn.Add(spawnCell);
                possibleCells.Remove(spawnCell);
            }

            SpawnSkyfaller(cellsToSpawn);
        }
    }
        
    private void SpawnSkyfaller(List<IntVec3> cellsToSpawn)
    {
        if (defMod.fromFaction == null)
        {
            return;
        }

        var faction = Find.FactionManager.FirstFactionOfDef(defMod.fromFaction);
            
        foreach (var cell in cellsToSpawn)
        {
            var innerThing = (Building_DropDrop)ThingMaker.MakeThing(Genes40kDefOf.BEWH_SteelRainDropPodBuilding);
                
            innerThing.SetFactionDirect(faction);
                
            var pawnsToSpawn = new List<Pawn>();

            var drawColor = new Color(1f, 1f, 1f, 0.2f);

            for (var i = 0; i < defMod.marinesToSpawn; i++)
            {
                var spawnPawn = PawnGenerator.GeneratePawn(Genes40kDefOf.BEWH_FirstbornPawn, faction);
                
                var chapter = Genes40kUtils.SetupChapterForPawn(spawnPawn, !defMod.usePlayerColours);

                drawColor = chapter.primaryColour;
                    
                pawnsToSpawn.Add(spawnPawn);
            }

            innerThing.DrawColor = drawColor;
            innerThing.MarinesToSpawn = pawnsToSpawn;
                
            var skyfaller = SkyfallerMaker.SpawnSkyfaller(Genes40kDefOf.BEWH_SteelRainDropPodSkyfaller, innerThing, cell, pawn.Map);
            skyfaller.DrawColor = drawColor;
        }
    }
}