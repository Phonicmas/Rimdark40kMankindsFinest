using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class GameComponent_Perpetual : GameComponent
    {
        private Dictionary<Pawn ,int> perpetuals = new Dictionary<Pawn, int>();
        
        private const int CheckInterval = 5000;
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
            
            foreach (var perpetual in perpetuals.Where(perpetual => perpetual.Key.Dead && Find.TickManager.TicksGame >= perpetual.Value))
            {
                if (!perpetual.Key.Spawned && perpetual.Key.Corpse != null && !perpetual.Key.Corpse.Spawned && perpetual.Key.Corpse.MapHeld == null)
                {
                    var map = GetMapToSpawnIn(perpetual.Key);
                    CellFinder.TryFindRandomCell(map, cell => cell.Walkable(map), out var cell2);
                    GenSpawn.Spawn(perpetual.Key, cell2, map);
                }
                
                ResurrectionUtility.TryResurrect(perpetual.Key);
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
}