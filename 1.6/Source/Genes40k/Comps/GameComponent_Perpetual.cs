using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_Perpetual : GameComponent
{
    private Dictionary<Pawn ,int> perpetuals = new ();
        
    private const int CheckInterval = 4000;
    private int currentTick;

    public GameComponent_Perpetual(Game game)
    {
    }

    public override void GameComponentTick()
    {
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
    }
        
    public void RemovePerpetual(Pawn pawn)
    {
        perpetuals.Remove(pawn);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref perpetuals, "perpetuals", LookMode.Deep);
        Scribe_Values.Look(ref currentTick, "currentTick");
    }
}