using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Random = System.Random;

namespace Genes40k
{
    public class Gene_InduceFear : Gene
    {
        private const int tickInterval = 625;
        private const float effectRadius = 7.9f;
        private const int chanceToFear = 50;
        
        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(tickInterval) || pawn.Faction == null)
            {
                return;
            }

            if (!pawn.Spawned || pawn.Downed || pawn.InMentalState || pawn.Crawling)
            {
                return;
            }

            if (!pawn.Drafted)
            {
                return;
            }
            
            var pawns = GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, effectRadius, useCenter: true).OfType<Pawn>().ToList();
            AffectPawns(pawn, pawns);
        }
        
        private void AffectPawns(Pawn p, List<Pawn> pawns)
        {
            if (pawns.NullOrEmpty())
            {
                return;
            }
            foreach (var otherPawn in pawns)
            {
                if (otherPawn == null || p == otherPawn || !p.RaceProps.Humanlike || otherPawn.Faction == Faction.OfPlayer || !otherPawn.Faction.HostileTo(Faction.OfPlayer))
                {
                    continue;
                }

                if (otherPawn.IsPrisoner)
                {
                    continue;
                }

                var defMod = def.GetModExtension<DefModExtension_GeneInducedFear>();
                
                if (otherPawn.genes != null && otherPawn.genes.GenesListForReading.Any(gene => defMod.genesCausesImmunityToFear.Contains(gene.def)))
                {
                    continue;
                }
                
                if (!otherPawn.story.traits.allTraits.NullOrEmpty() && defMod.traitCausesImmunityToFear.Any(traitData => otherPawn.story.traits.HasTrait(traitData.traitDef)))
                {
                    continue;
                }

                var random = new Random();
                var randomRoll = random.Next(0, 100);
                
                if (randomRoll > chanceToFear)
                {
                    continue;
                }
                
                otherPawn.jobs.StopAll(canReturnToPool: false);
                CellFinderLoose.GetFleeExitPosition(otherPawn, 999, out var intVec);
                var job = JobMaker.MakeJob(Genes40kDefOf.BEWH_InducedFearJob,intVec);
                otherPawn.jobs.StartJob(job);
                otherPawn.CurJob.playerForced = true;
                
                otherPawn.mindState.mentalStateHandler.CurState?.RecoverFromState();
                otherPawn.mindState.mentalStateHandler.TryStartMentalState(Genes40kDefOf.BEWH_InducedFear);
            }
        }
    }
}