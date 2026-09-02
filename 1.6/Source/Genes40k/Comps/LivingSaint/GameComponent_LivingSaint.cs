using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_LivingSaint : GameComponent
{
    private List<Pawn> livingSaints = new();
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

        if (!Enumerable.Any(livingSaints, p => p is { Dead: true }))
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
        if (Rand.Chance(chance / 100f))
        {
            SpawnSaint();
        }
    }

    private void SpawnSaint()
    {
        var deadSaints = livingSaints.Where(saint => saint is { Dead: true }).ToList();

        if (!deadSaints.Any())
        {
            return;
        }

        var toSpawn = deadSaints.RandomElement();

        var map = Find.CurrentMap ?? Find.AnyPlayerHomeMap;

        if (map == null)
        {
            return;
        }

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

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        livingSaints ??= new List<Pawn>();
        livingSaints.RemoveAll(saint => saint == null);
    }
}