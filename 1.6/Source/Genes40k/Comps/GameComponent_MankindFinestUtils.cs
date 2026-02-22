using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class GameComponent_MankindFinestUtils : GameComponent
{
    private const int CheckInterval = 500;
    private int currentTick;
    private bool useNewRandomChapter = true;
        
    private Genes40kModSettings modSettings = null;
    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    private ChapterColourDef currentChapterColour;
    public ChapterColourDef CurrentChapterColour
    {
        get
        {
            if (useNewRandomChapter)
            {
                useNewRandomChapter = false;
                currentChapterColour = GetRandomChapterForRaid();
            }

            return currentChapterColour;
        }
    }

    public GameComponent_MankindFinestUtils(Game game) { }
    
    public override void GameComponentTick()
    {
        if (currentTick != CheckInterval)
        {
            currentTick++;
            return;
        }

        currentTick = 0;
        useNewRandomChapter = true;
    }
    
    private ChapterColourDef GetRandomChapterForRaid()
    {   
        var chapterColours = DefDatabase<ChapterColourDef>.AllDefsListForReading.ToList();

        if (ModSettings.CurrentlySelectedPreset != null && ModSettings.CurrentlySelectedPreset != ModSettings.CustomPreset)
        {
            chapterColours.Remove(ModSettings.CurrentlySelectedPreset);
        }

        if (currentChapterColour != null && chapterColours.Contains(currentChapterColour))
        {
            chapterColours.Remove(currentChapterColour);
        }

        return chapterColours.RandomElement();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref currentTick, "currentTick");
    }
}