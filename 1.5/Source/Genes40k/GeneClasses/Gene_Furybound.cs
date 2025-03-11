using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Random = System.Random;

namespace Genes40k
{
    public class Gene_Furybound : Gene
    {
        private const int tickInterval = 3000;
        private const int percentChanceIncrease = 10;
        public int percentChance = 0;
        
        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(tickInterval))
            {
                return;
            }

            if (!pawn.Spawned || pawn.Downed || pawn.Crawling)
            {
                return;
            }

            percentChance += percentChanceIncrease;
            
            var random = new Random();
            if (random.Next(0, 100) > percentChance)
            {
                return;
            }
            
            pawn.mindState.mentalStateHandler.TryStartMentalState(Genes40kDefOf.BEWH_ThunderWarriorBerserk, forced: true);
            percentChance = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref percentChance, "percentChance");
        }
    }
}