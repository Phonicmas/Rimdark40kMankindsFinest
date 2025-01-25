using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_DropDrop : Building_TurretGun
    {
        private bool hasSpawnedMarines = false;

        private Lord lord = null;

        private const string LeaveSignal = "BEWH_SpaceMarineHelpEnd";

        public List<Pawn> MarinesToSpawn = new List<Pawn>();

        [Unsaved]
        private Graphic cachedOpenGraphic;
        
        private Graphic OpenGraphic =>
            cachedOpenGraphic ?? (cachedOpenGraphic =
                GraphicDatabase.Get<Graphic_Single>(def.GetModExtension<DefModExtension_DropPod>().openGraphic, def.graphicData.shaderType.Shader,
                    def.graphicData.drawSize, DrawColor, Color.white, DefaultGraphic.data, def.GetModExtension<DefModExtension_DropPod>().openGraphicMask));
        
        public override Graphic Graphic => hasSpawnedMarines ? OpenGraphic : DefaultGraphic;

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
            
            def.GetModExtension<DefModExtension_DropPod>().openSound.PlayOneShot(new TargetInfo(Position, Map));

            for (var i = 0; i < MarinesToSpawn.Count; i++)
            {
                var actualPosition = Position;
                if (i+1 < MarinesToSpawn.Count)
                {
                    actualPosition += positions[i].ToIntVec3();
                }
                else
                {
                    actualPosition = actualPosition.RandomAdjacentCell8Way();
                }

                FleckMaker.ThrowDustPuff(actualPosition, Map, 1f);
                GenSpawn.Spawn(MarinesToSpawn[i], actualPosition, Map);
                
                pawns.Add(MarinesToSpawn[i]);
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
            Scribe_Collections.Look(ref MarinesToSpawn, "MarinesToSpawn");
            Scribe_References.Look(ref lord, "lord");
        }
    }
}