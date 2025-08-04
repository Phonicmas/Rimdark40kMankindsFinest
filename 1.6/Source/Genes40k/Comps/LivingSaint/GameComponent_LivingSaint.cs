using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_LivingSaint : GameComponent
{
    private List<Pawn> livingSaints = new List<Pawn>();
    public List<Pawn> LivingSaints => livingSaints;
        
    private Genes40kModSettings modSettings;
        
    public int LivingSaintsCount => livingSaints.Count;

    public GameComponent_LivingSaint(Game game)
    {
        modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
    }

    public void TrySpawnSaint(IncidentCategoryDef categoryDef)
    {
        if (livingSaints.Count <= 0)
        {
            return;
        }

        if (!Enumerable.Any(livingSaints, p => p.Dead))
        {
            return;
        }
            
        int chance;
        if (categoryDef == IncidentCategoryDefOf.ThreatBig)
        {
            chance = modSettings.livingSaintBigThreat;
        }
        else if (categoryDef == IncidentCategoryDefOf.ThreatSmall)
        {
            chance = modSettings.livingSaintSmallThreat;
        }
        else
        {
            return;
        }
        if (Prefs.DevMode && DebugSettings.godMode)
        {
            chance = 200;
        }
        var rand = new Random();
        if (rand.Next(0, 100) <= chance)
        {
            SpawnSaint();
        }
    }

    private void SpawnSaint()
    {
        var toSpawn = livingSaints.RandomElement();

        if (!toSpawn.Dead)
        {
            return;
        }

        var map = Find.CurrentMap;

        ResurrectionUtility.TryResurrect(toSpawn);

        if (!GenPlace.TryPlaceThing(toSpawn, CellFinder.RandomEdgeCell(map), map, ThingPlaceMode.Near))
        {
            return;
        }

        var letter = LetterMaker.MakeLetter("BEWH.MankindsFinest.LivingSaint.LivingSaintReturn".Translate(), "BEWH.MankindsFinest.LivingSaint.LivingSaintReturnMessage".Translate(toSpawn), Genes40kDefOf.BEWH_GoldenPositive, toSpawn);
        Find.LetterStack.ReceiveLetter(letter);
    }

    public void AddSaintToSpawnable(Pawn pawn)
    {
        if (livingSaints.Contains(pawn))
        {
            return;
        }
            
        livingSaints.Add(pawn);
    }
        
    public void RemoveSaintFromSpawnable(Pawn pawn)
    {
        if (!livingSaints.Contains(pawn))
        {
            return;
        }
            
        livingSaints.Remove(pawn);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref livingSaints, "livingSaints", LookMode.Reference);
    }
}