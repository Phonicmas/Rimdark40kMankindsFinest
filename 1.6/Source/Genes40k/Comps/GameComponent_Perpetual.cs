using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_Perpetual : GameComponent
{
    private Dictionary<Pawn ,int> perpetuals = new ();
    public Dictionary<Pawn ,int> Perpetuals => perpetuals;

    private List<Pawn> perpetualsKeysWorkingList;
    private List<int> perpetualsValuesWorkingList;
        
    private const int CheckInterval = 4000;
    private int currentTick;

    public GameComponent_Perpetual(Game game)
    {
    }

    public override void GameComponentTick()
    {
        if (perpetuals.Count > 0)
        {
            List<Pawn> unrecoverable = null;

            foreach (var trackedPawn in perpetuals.Keys)
            {
                if (trackedPawn.Destroyed || trackedPawn.Discarded)
                {
                    unrecoverable ??= new List<Pawn>();
                    unrecoverable.Add(trackedPawn);
                    continue;
                }

                KeepPawnForResurrection(trackedPawn);
            }

            if (unrecoverable != null)
            {
                foreach (var lostPawn in unrecoverable)
                {
                    perpetuals.Remove(lostPawn);
                }
            }
        }

        if (currentTick != CheckInterval)
        {
            currentTick++;  
            return;
        }

        currentTick = 0;
            
        var removeAfterResurrection = new List<Pawn>();
            
        foreach (var perpetual in perpetuals.Where(perpetual => Find.TickManager.TicksGame >= perpetual.Value))
        {
            if (perpetual.Key.genes?.GetFirstGeneOfType<Gene_Perpetual>() == null)
            {
                removeAfterResurrection.Add(perpetual.Key);
                continue;
            }
            if (perpetual.Key.Dead)
            {
                ResurrectionUtility.TryResurrect(perpetual.Key);
            }
                
            if (!perpetual.Key.Spawned && perpetual.Key.Corpse is { Spawned: false } or null)
            {
                var map = GetMapToSpawnIn(perpetual.Key);
                CellFinder.TryFindRandomCell(map, cell => cell.Walkable(map), out var cell2);
                var pawn = GenSpawn.Spawn(perpetual.Key, cell2, map);
                    
                var letter = LetterMaker.MakeLetter("BEWH.MankindsFinest.Perpetual.PerpetualReturn".Translate(), "BEWH.MankindsFinest.Perpetual.PerpetualReturnMessage".Translate(pawn), Genes40kDefOf.BEWH_GoldenPositive, pawn);
                Find.LetterStack.ReceiveLetter(letter);
            }
            removeAfterResurrection.Add(perpetual.Key);
        }

        foreach (var pawn in removeAfterResurrection)
        {
            RemovePerpetual(pawn);
        }
    }

    private static Map GetMapToSpawnIn(Pawn pawn)
    {
        if (pawn.Map != null)
        {
            return pawn.Map;
        }

        if (pawn.Corpse?.Map != null)
        {
            return pawn.Corpse.Map;
        }
            
        var map = Find.AnyPlayerHomeMap;
        if (map != null)
        {
            return map;
        }
            
        return Find.CurrentMap ?? Find.Maps.First();
    }
        
    public void AddPerpetual(Pawn pawn, int resurrectIn)
    {
        if (!perpetuals.ContainsKey(pawn))
        {
            perpetuals.Add(pawn, resurrectIn);
        }

        KeepPawnForResurrection(pawn);
    }

    private static void KeepPawnForResurrection(Pawn pawn)
    {
        if (pawn == null || pawn.Spawned || pawn.Discarded)
        {
            return;
        }

        if (Find.WorldPawns.Contains(pawn))
        {
            return;
        }

        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
    }
        
    public void RemovePerpetual(Pawn pawn)
    {
        perpetuals.Remove(pawn);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref perpetuals, "perpetuals", LookMode.Reference, LookMode.Value, ref perpetualsKeysWorkingList, ref perpetualsValuesWorkingList);
        Scribe_Values.Look(ref currentTick, "currentTick");

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            perpetuals ??= new Dictionary<Pawn, int>();
        }
    }
}