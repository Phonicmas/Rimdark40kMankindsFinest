using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Ability_RaiseBastion : VFECore.Abilities.Ability
    {
        List<IntVec3> affectCellsWalls = new List<IntVec3>
        {
            new IntVec3(4, 0, 1),
            new IntVec3(4, 0, -1),
            new IntVec3(4, 0, 3),
            new IntVec3(4, 0, -3),
            new IntVec3(3, 0, 2),
            new IntVec3(3, 0, -2),

            new IntVec3(-4, 0, 1),
            new IntVec3(-4, 0, -1),
            new IntVec3(-4, 0, 3),
            new IntVec3(-4, 0, -3),
            new IntVec3(-3, 0, 2),
            new IntVec3(-3, 0, -2),
            
            new IntVec3(1, 0, 4),
            new IntVec3(-1, 0, 4),
            new IntVec3(3, 0, 4),
            new IntVec3(-3, 0, 4),
            new IntVec3(2, 0, 3),
            new IntVec3(-2, 0, 3),
            
            new IntVec3(1, 0, -4),
            new IntVec3(-1, 0, -4),
            new IntVec3(3, 0, -4),
            new IntVec3(-3, 0, -4),
            new IntVec3(2, 0, -3),
            new IntVec3(-2, 0, -3),
            
            new IntVec3(3, 0, 3),
            new IntVec3(-3, 0, -3),
            new IntVec3(3, 0, -3),
            new IntVec3(-3, 0, 3),
        };
        
        List<IntVec3> affectCellsTurrets = new List<IntVec3>
        {
            new IntVec3(4, 0, 2),
            new IntVec3(4, 0, -2),
            
            new IntVec3(-4, 0, 2),
            new IntVec3(-4, 0, -2),
            
            new IntVec3(2, 0, 4),
            new IntVec3(-2, 0, 4),
            
            new IntVec3(2, 0, -4),
            new IntVec3(-2, 0, -4),
        };
        
        List<IntVec3> affectCellsBarricade = new List<IntVec3>
        {
            new IntVec3(4, 0, 0),
            new IntVec3(-4, 0, 0),
            new IntVec3(0, 0, 4),
            new IntVec3(0, 0, -4),
        };

        internal IEnumerable<IntVec3> TotalAffectedCells(LocalTargetInfo target, Map map, IEnumerable<IntVec3> affectedCells)
        {
            return from intVec in affectedCells
                select target.Cell + new IntVec3(intVec.x, 0, intVec.z) into intVec2
                where intVec2.InBounds(map)
                select intVec2;
        }
        
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            for (var i = 0; i < targets.Length; i++)
            {
                var globalTargetInfo = targets[i];
                var map = globalTargetInfo.Map;
                var target = (globalTargetInfo.HasThing ? new LocalTargetInfo(globalTargetInfo.Thing) : new LocalTargetInfo(globalTargetInfo.Cell));
                
                var totalAffectedCells = TotalAffectedCells(target, map,
                    affectCellsBarricade.Concat(affectCellsWalls).Concat(affectCellsTurrets));

                var affectedCells = totalAffectedCells.ToList();
                
                var list = new List<Thing>();
                list.AddRange(affectedCells.SelectMany(c => from t in c.GetThingList(map) where t.def.category == ThingCategory.Item select t));
                
                foreach (var item in list)
                {
                    item.DeSpawn();
                }
                
                var list2 = new List<Plant>();
                list2.AddRange(affectedCells.Where(c => c.GetPlant(map) != null).Select(c=> c.GetPlant(map)));
                
                foreach (var plant in list2)
                {
                    plant.Destroy();
                }
                
                foreach (var item2 in TotalAffectedCells(target, map, affectCellsWalls))
                {
                    var wall = GenSpawn.Spawn(Genes40kDefOf.BEWH_RaisedWall, item2, map);
                    wall.SetFactionDirect(Faction.OfPlayer);
                    FleckMaker.ThrowDustPuffThick(item2.ToVector3Shifted(), map, Rand.Range(1.5f, 3f), CompAbilityEffect_Wallraise.DustColor);
                }
                
                foreach (var item2 in TotalAffectedCells(target, map, affectCellsBarricade))
                {
                    var barricade = GenSpawn.Spawn(Genes40kDefOf.BEWH_RaisedBarricade, item2, map);
                    barricade.SetFactionDirect(Faction.OfPlayer);
                    FleckMaker.ThrowDustPuffThick(item2.ToVector3Shifted(), map, Rand.Range(1.5f, 3f), CompAbilityEffect_Wallraise.DustColor);
                }
                
                foreach (var item2 in TotalAffectedCells(target, map, affectCellsTurrets))
                {
                    var turret = GenSpawn.Spawn(Genes40kDefOf.BEWH_RaisedTurret, item2, map);
                    turret.SetFactionDirect(Faction.OfPlayer);
                    FleckMaker.ThrowDustPuffThick(item2.ToVector3Shifted(), map, Rand.Range(1.5f, 3f), CompAbilityEffect_Wallraise.DustColor);
                }
                
                foreach (var item3 in list)
                {
                    var intVec = IntVec3.Invalid;
                    for (var j = 0; j < 9; j++)
                    {
                        var intVec2 = item3.Position + GenRadial.RadialPattern[j];
                        if (intVec2.InBounds(map) && intVec2.Walkable(map) && map.thingGrid.ThingsListAtFast(intVec2).Count <= 0)
                        {
                            intVec = intVec2;
                            break;
                        }
                    }
                    if (intVec != IntVec3.Invalid)
                    {
                        GenSpawn.Spawn(item3, intVec, map);
                    }
                    else
                    {
                        GenPlace.TryPlaceThing(item3, item3.Position, map, ThingPlaceMode.Near);
                    }
                }
            }
        }
        
        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
            GenDraw.DrawFieldEdges(TotalAffectedCells(target, pawn.Map, affectCellsBarricade.Concat(affectCellsWalls).Concat(affectCellsTurrets)).ToList(), ValidateTarget(target, showMessages: false) ? Color.white : Color.red);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = false)
        {
            if (TotalAffectedCells(target, pawn.Map, affectCellsBarricade.Concat(affectCellsWalls).Concat(affectCellsTurrets)).Any(c => c.Filled(pawn.Map)))
            {
                if (showMessages)
                {
                    Messages.Message("AbilityOccupiedCells".Translate(def.LabelCap), target.ToTargetInfo(pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            /*if (TotalAffectedCells(target, pawn.Map, affectCellsBarricade.Concat(affectCellsWalls).Concat(affectCellsTurrets)).Any(c => !c.Standable(pawn.Map)))
            {
                if (showMessages)
                {
                    Messages.Message("AbilityUnwalkable".Translate(def.LabelCap), target.ToTargetInfo(pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }*/
            return true;
        }
    }
}