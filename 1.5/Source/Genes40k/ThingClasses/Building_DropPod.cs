using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_DropDrop : Building_TurretGun
    {
        private bool hasSpawnedMarines = false;

        private Lord lord = null;

        private const string LeaveSignal = "BEWH_SpaceMarineHelpEnd";

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            var offworldFaction = Find.FactionManager.FirstFactionOfDef(Genes40kDefOf.BEWH_OffworldMarinesFaction);
            if (offworldFaction == null)
            {
                var faction = new Faction
                {
                    def = Genes40kDefOf.BEWH_OffworldMarinesFaction,
                };
                offworldFaction = faction;
                //Log.Error("Steel Rain: Could not find offworld faction. Missing from game? Report to Phonicmas");
            }
            SetFactionDirect(offworldFaction);
        }

        public override void Tick()
        {
            base.Tick();
            if (hasSpawnedMarines || !Spawned || Map == null || !this.IsHashIntervalTick(125))
            {
                return;
            }

            hasSpawnedMarines = true;
            
            var positions = new List<Vector3>
            {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1)
            };

            var pawns = new List<Pawn>(); 
            
            foreach (var position in positions)
            {
                var actualPosition = Position + position.ToIntVec3();
                
                var spawnPawn = PawnGenerator.GeneratePawn(Genes40kDefOf.BEWH_FirstbornPawn, Faction);
                
                Genes40kUtils.SetupChapterForPawn(spawnPawn);
                
                GenSpawn.Spawn(spawnPawn, actualPosition, Map);
                
                pawns.Add(spawnPawn);
            }
            
            var lordJob = new LordJob_AssistColony(Faction, Position + positions.First().ToIntVec3());
            lord = LordMaker.MakeNewLord(Faction, lordJob, Map, pawns);
            lord.inSignalLeave = LeaveSignal;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            lord?.Notify_SignalReceived(new Signal(LeaveSignal));
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hasSpawnedMarines, "hasSpawnedMarines");
            Scribe_References.Look(ref lord, "lord");
        }
    }
}