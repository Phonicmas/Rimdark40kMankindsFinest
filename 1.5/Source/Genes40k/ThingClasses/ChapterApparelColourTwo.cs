using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ChapterApparelColourTwo : ApparelColourTwo
    {
        private Genes40kModSettings modSettings = null;

        private Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());

        private CompRankInfo rankInfoComp = null;

        public CompRankInfo RankInfoComp
        {
            get
            {
                if (rankInfoComp == null && ParentHolder is Pawn pawn)
                {
                    rankInfoComp = pawn.GetComp<CompRankInfo>();
                }

                return rankInfoComp;
            }
        }
        
        private RankDef lastCheckRank = null;
        
        private string originalOverrideRankIcon = null;
        
        private string overrideRankIcon = null;

        private ChapterColourDef originalSelectedChapterIcon = null;
        
        private ChapterColourDef currentlySelectedChapterIcon = null;

        public ChapterColourDef CurrentlySelectedChapterIcon
        {
            get => currentlySelectedChapterIcon;
            set
            {
                currentlySelectedChapterIcon = value;
                Notify_ColorChanged();
            }
        }
        
        public string OverrideRankIcon
        {
            get
            {
                if (RankInfoComp != null)
                {
                    var rankCheck = RankInfoComp.HighestRankDef(true) ?? RankInfoComp.HighestRankDef(false);

                    if (lastCheckRank == null || rankCheck != null && rankCheck.rankTier > lastCheckRank.rankTier)
                    {
                        lastCheckRank = rankCheck;
                        overrideRankIcon = null;
                    }
                }
                
                return overrideRankIcon;
            }
            set
            {
                overrideRankIcon = value;
                Notify_ColorChanged();
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            DrawColor = ModSettings?.chapterColorOne ?? base.DrawColor;
            SetSecondaryColor(ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
        }

        public void ApplyColourPreset(ChapterColourDef chapterColour)
        {
            DrawColor = chapterColour.primaryColour;
            SetSecondaryColor(chapterColour.secondaryColour);
        }

        public override void SetOriginals()
        {
            originalSelectedChapterIcon = currentlySelectedChapterIcon;
            originalOverrideRankIcon = overrideRankIcon;
            base.SetOriginals();
        }
        
        public override void Reset()
        {
            overrideRankIcon = originalOverrideRankIcon;
            currentlySelectedChapterIcon = originalSelectedChapterIcon;
            base.Reset();
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref currentlySelectedChapterIcon, "currentlySelectedChapterIcon");
            Scribe_Defs.Look(ref originalSelectedChapterIcon, "originalSelectedChapterIcon");
            Scribe_Defs.Look(ref lastCheckRank, "lastCheckRank");
            Scribe_Values.Look(ref overrideRankIcon, "overrideRankIcon");
            Scribe_Values.Look(ref originalOverrideRankIcon, "originalOverrideRankIcon");
            base.ExposeData();
        }
    }
}