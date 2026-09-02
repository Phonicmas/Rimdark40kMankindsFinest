using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Ability_RaiseBastion : VEF.Abilities.Ability
{
    List<IntVec3> affectCellsWalls = new()
    {
        new (4, 0, 1),
        new (4, 0, -1),
        new (4, 0, 3),
        new (4, 0, -3),
        new (3, 0, 2),
        new (3, 0, -2),

        new (-4, 0, 1),
        new (-4, 0, -1),
        new (-4, 0, 3),
        new (-4, 0, -3),
        new (-3, 0, 2),
        new (-3, 0, -2),
            
        new (1, 0, 4),
        new (-1, 0, 4),
        new (3, 0, 4),
        new (-3, 0, 4),
        new (2, 0, 3),
        new (-2, 0, 3),
            
        new (1, 0, -4),
        new (-1, 0, -4),
        new (3, 0, -4),
        new (-3, 0, -4),
        new (2, 0, -3),
        new (-2, 0, -3),
            
        new (3, 0, 3),
        new (-3, 0, -3),
        new (3, 0, -3),
        new (-3, 0, 3),
    };
        
    List<IntVec3> affectCellsTurrets = new()
    {
        new (4, 0, 2),
        new (4, 0, -2),
            
        new (-4, 0, 2),
        new (-4, 0, -2),
            
        new (2, 0, 4),
        new (-2, 0, 4),
            
        new (2, 0, -4),
        new (-2, 0, -4),
    };
        
    List<IntVec3> affectCellsBarricade = new()
    {
        new (4, 0, 0),
        new (-4, 0, 0),
        new (0, 0, 4),
        new (0, 0, -4),
    };

    internal IEnumerable<IntVec3> TotalAffectedCells(LocalTargetInfo target, Map map, IEnumerable<IntVec3> affectedCells)
    {
        return from intVec in affectedCells
            select target.Cell + new IntVec3(intVec.x, 0, intVec.z) into intVec2
            where intVec2.InBounds(map)
            select intVec2;
    }
        
    private const int MaxDisplacementSteps = 8;

    private const int MaxItemSearchCells = 30;

    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        for (var i = 0; i < targets.Length; i++)
        {
            var globalTargetInfo = targets[i];
            var map = globalTargetInfo.Map;

            if (map == null)
            {
                continue;
            }

            var target = globalTargetInfo.HasThing ? new LocalTargetInfo(globalTargetInfo.Thing) : new LocalTargetInfo(globalTargetInfo.Cell);

            var wallCells = TotalAffectedCells(target, map, affectCellsWalls).ToList();
            var barricadeCells = TotalAffectedCells(target, map, affectCellsBarricade).ToList();
            var turretCells = TotalAffectedCells(target, map, affectCellsTurrets).ToList();

            var structureCells = new HashSet<IntVec3>(wallCells);
            structureCells.UnionWith(barricadeCells);
            structureCells.UnionWith(turretCells);

            //Cells still holding something that could not be moved clear. Nothing is built on these,
            //so the bastion never closes on top of a pawn and never destroys an item it cannot place.
            var blockedCells = new HashSet<IntVec3>();

            DisplacePawns(target.Cell, map, structureCells, blockedCells);
            DisplaceItems(map, structureCells, blockedCells);

            foreach (var cell in structureCells)
            {
                if (blockedCells.Contains(cell))
                {
                    continue;
                }

                cell.GetPlant(map)?.Destroy();
            }

            SpawnStructures(wallCells, Genes40kDefOf.BEWH_RaisedWall, map, blockedCells);
            SpawnStructures(barricadeCells, Genes40kDefOf.BEWH_RaisedBarricade, map, blockedCells);
            SpawnStructures(turretCells, Genes40kDefOf.BEWH_RaisedTurret, map, blockedCells);
        }
    }

    private static void SpawnStructures(List<IntVec3> cells, ThingDef structureDef, Map map, HashSet<IntVec3> blockedCells)
    {
        foreach (var cell in cells)
        {
            if (blockedCells.Contains(cell))
            {
                continue;
            }

            var structure = GenSpawn.Spawn(structureDef, cell, map);
            structure.SetFactionDirect(Faction.OfPlayer);
            FleckMaker.ThrowDustPuffThick(cell.ToVector3Shifted(), map, Rand.Range(1.5f, 3f), CompAbilityEffect_Wallraise.DustColor);
        }
    }

    /// <summary>
    /// Clears pawns off the footprint before anything is built: anyone not hostile to the caster is
    /// pushed in towards the centre of the bastion, hostiles are pushed out of it.
    /// </summary>
    private void DisplacePawns(IntVec3 centre, Map map, HashSet<IntVec3> structureCells, HashSet<IntVec3> blockedCells)
    {
        var casterFaction = pawn.Faction;

        foreach (var cell in structureCells)
        {
            foreach (var thing in cell.GetThingList(map).ToList())
            {
                if (thing is not Pawn otherPawn)
                {
                    continue;
                }

                var towardsCentre = casterFaction == null || !otherPawn.HostileTo(casterFaction);

                if (!TryFindPawnCell(otherPawn.Position, centre, towardsCentre, map, structureCells, out var destination))
                {
                    blockedCells.Add(cell);
                    continue;
                }

                otherPawn.Position = destination;
                otherPawn.Notify_Teleported(false);
            }
        }
    }

    private static bool TryFindPawnCell(IntVec3 from, IntVec3 centre, bool towardsCentre, Map map, HashSet<IntVec3> structureCells, out IntVec3 result)
    {
        result = IntVec3.Invalid;

        var offset = towardsCentre ? centre - from : from - centre;

        if (offset == IntVec3.Zero)
        {
            offset = IntVec3.North;
        }

        var step = new IntVec3(Mathf.Clamp(offset.x, -1, 1), 0, Mathf.Clamp(offset.z, -1, 1));

        var candidate = from;

        for (var i = 0; i < MaxDisplacementSteps; i++)
        {
            candidate += step;

            if (!candidate.InBounds(map))
            {
                return false;
            }

            if (structureCells.Contains(candidate) || !candidate.Standable(map))
            {
                continue;
            }

            result = candidate;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Moves loose items clear of the footprint. The item is only despawned once a destination is
    /// known, so unlike the previous behaviour a failed move can never leave it in limbo.
    /// </summary>
    private static void DisplaceItems(Map map, HashSet<IntVec3> structureCells, HashSet<IntVec3> blockedCells)
    {
        foreach (var cell in structureCells)
        {
            foreach (var thing in cell.GetThingList(map).ToList())
            {
                if (thing.def.category != ThingCategory.Item)
                {
                    continue;
                }

                if (!TryFindItemCell(thing.Position, map, structureCells, out var destination))
                {
                    blockedCells.Add(cell);
                    continue;
                }

                thing.DeSpawn();
                GenSpawn.Spawn(thing, destination, map);
            }
        }
    }

    private static bool TryFindItemCell(IntVec3 from, Map map, HashSet<IntVec3> structureCells, out IntVec3 result)
    {
        var searchCells = Mathf.Min(MaxItemSearchCells, GenRadial.RadialPattern.Length);

        for (var i = 0; i < searchCells; i++)
        {
            var candidate = from + GenRadial.RadialPattern[i];

            if (!candidate.InBounds(map) || structureCells.Contains(candidate) || !candidate.Walkable(map))
            {
                continue;
            }

            if (candidate.GetThingList(map).Any(t => t.def.category == ThingCategory.Item))
            {
                continue;
            }

            result = candidate;
            return true;
        }

        result = IntVec3.Invalid;
        return false;
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
        return base.ValidateTarget(target, showMessages);
    }
}