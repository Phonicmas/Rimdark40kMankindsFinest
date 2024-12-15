using System;
using Core40k;
using Verse;

namespace Genes40k
{
    public class Gene_ProgenoidGlands : Gene_DisabledBy
    {
        private bool firstProgenoidGlandHarvested = false;
        private bool secondProgenoidGlandHarvested = false;
        private int timeWhenHarvestable = Find.TickManager.TicksGame + 3600000;

        public bool FirstProgenoidGlandHarvested => firstProgenoidGlandHarvested;
        
        public bool SecondProgenoidGlandHarvested => secondProgenoidGlandHarvested;

        public int TicksUntilHarvestable => Math.Max(timeWhenHarvestable - Find.TickManager.TicksGame, 0);
        
        public bool HarvestFirstProgenoidGland()
        {
            if (firstProgenoidGlandHarvested)
            {
                return false;
            }
            
            firstProgenoidGlandHarvested = true;
            return true;
        }

        public bool HarvestSecondProgenoidGland()
        {
            if (secondProgenoidGlandHarvested)
            {
                return false;
            }
            
            secondProgenoidGlandHarvested = true;
            return true;
        }

        public bool CanHarvestFirstProgenoidGland()
        {
            return Find.TickManager.TicksGame >= timeWhenHarvestable && !FirstProgenoidGlandHarvested;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref firstProgenoidGlandHarvested, "firstProgenoidGlandHarvested", false);
            Scribe_Values.Look(ref secondProgenoidGlandHarvested, "secondProgenoidGlandHarvested", false);
            Scribe_Values.Look(ref timeWhenHarvestable, "timeWhenHarvestable", 0);
        }
    }
}