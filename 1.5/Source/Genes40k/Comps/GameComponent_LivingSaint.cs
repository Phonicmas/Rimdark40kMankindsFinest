using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class GameComponent_LivingSaint : GameComponent
    {
        private List<Pawn> livingSaints = new List<Pawn>();

        public GameComponent_LivingSaint(Game game)
        {
        }

        public void TrySpawnSaint(IncidentCategoryDef categoryDef)
        {
            int chance;
            if (categoryDef == IncidentCategoryDefOf.ThreatBig)
            {
                chance = 65;
            }
            else if (categoryDef == IncidentCategoryDefOf.ThreatSmall)
            {
                chance = 35;
            }
            else
            {
                return;
            }
            if (Prefs.DevMode && DebugSettings.godMode)
            {
                chance = 200;
            }
            Random rand = new Random();
            if (rand.Next(0, 100) <= chance)
            {
                SpawmSaint();
            }
        }

        private void SpawmSaint()
        {
            Pawn toSpawn = livingSaints.RandomElement();

            Map map = Find.CurrentMap;

            ResurrectionUtility.TryResurrect(toSpawn);

            if (!GenPlace.TryPlaceThing(toSpawn, CellFinder.RandomEdgeCell(map), map, ThingPlaceMode.Near))
            {
                return;
            }
            
            livingSaints.Remove(toSpawn);

            ChoiceLetter letter = LetterMaker.MakeLetter("BEWH.LivingSaintReturn".Translate(), "BEWH.LivingSaintReturnMessage".Translate(toSpawn), Genes40kDefOf.BEWH_GoldenPositive, toSpawn);
            Find.LetterStack.ReceiveLetter(letter);
        }

        public void AddSaintToSpawnable(Pawn pawn)
        {
            if (pawn.genes == null || !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintBeingOfFaith) || livingSaints.Contains(pawn))
            {
                return;
            }
            livingSaints.Add(pawn);
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref livingSaints, "livingSaints", LookMode.Reference);
        }
    }
}