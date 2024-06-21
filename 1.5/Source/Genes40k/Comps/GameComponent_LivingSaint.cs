using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Noise;

namespace Genes40k
{
    public class GameComponent_LivingSaint : GameComponent
    {
        private List<Pawn> livingSaints = new List<Pawn>();

        public GameComponent_LivingSaint(Game game)
        {
        }

        public override void GameComponentTick()
        {
            //Check if raid in progress, if so spawn a living saint with some % chance (50%?) maybe scale of severity of raid

            /*Pawn toSpawn = livingSaints.RandomElement();

            Map map = Find.CurrentMap;

            GenPlace.TryPlaceThing(toSpawn, CellFinder.RandomEdgeCell(map), map, ThingPlaceMode.Near);
            livingSaints.Remove(toSpawn);*/
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