using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class GameComponent_MankindFinestUtils : GameComponent
    {

        private const int CheckInterval = 500;
        private int currentTick;

        public bool useNewRandomChapter = true;
        
        private Genes40kModSettings modSettings = null;

        private Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());

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

        public GameComponent_MankindFinestUtils(Game game)
        {
        }

        public override void GameComponentTick()
        {
            if (currentTick != CheckInterval)
            {
                currentTick++;
                return;
            }

            currentTick = 0;

            if (!useNewRandomChapter)
            {
                useNewRandomChapter = true;
            }
        }
        
        private ChapterColourDef GetRandomChapterForRaid()
        {
            var chapterColours = DefDatabase<ChapterColourDef>.AllDefsListForReading;

            if (ModSettings.currentlySelectedPreset != null)
            {
                chapterColours.Remove(ModSettings.currentlySelectedPreset);
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
}