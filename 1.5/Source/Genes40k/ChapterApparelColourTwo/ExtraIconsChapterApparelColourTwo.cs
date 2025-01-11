using Core40k;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class ExtraIconsChapterApparelColourTwo : ChapterApparelColourTwo
    {
        [Unsaved]
        private CompRankInfo rankInfoComp = null;

        private CompRankInfo RankInfoComp
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
        
        
        private ShoulderIconDef originalRightShoulderIcon = null;
        
        private ShoulderIconDef rightShoulderIcon = null;

        public ShoulderIconDef RightShoulderIcon
        {
            get
            {
                if (RankInfoComp != null)
                {
                    var rankCheck = RankInfoComp.HighestRankDef(true) ?? RankInfoComp.HighestRankDef(false);

                    if (lastCheckRank == null || rankCheck != null && rankCheck.rankTier > lastCheckRank.rankTier)
                    {
                        lastCheckRank = rankCheck;
                        rightShoulderIcon = null;
                    }
                }
                
                return rightShoulderIcon;
            }
            set
            {
                rightShoulderIcon = value;
                Notify_ColorChanged();
            }
        }
        
        
        private ShoulderIconDef originalLeftShoulderIcon = null;
        
        private ShoulderIconDef leftShoulderIcon = null;

        public ShoulderIconDef LeftShoulderIcon
        {
            get => leftShoulderIcon;
            set
            {
                leftShoulderIcon = value;
                Notify_ColorChanged();
            }
        }
        
        private BodyTypeDef originalBodyType = null;

        public override void SetUpMisc()
        {
            leftShoulderIcon = ModSettings?.CurrentlySelectedPreset.relatedChapterIcon;
            base.SetUpMisc();
        }

        public override void ApplyColourPreset(ChapterColourDef chapterColour)
        {
            if (chapterColour.relatedChapterIcon != null)
            {
                leftShoulderIcon = chapterColour.relatedChapterIcon;
            }
            base.ApplyColourPreset(chapterColour);
        }
        
        public override void SetOriginals()
        {
            originalLeftShoulderIcon = leftShoulderIcon;
            originalRightShoulderIcon = rightShoulderIcon;
            base.SetOriginals();
        }
        
        public override void Reset()
        {
            rightShoulderIcon = originalRightShoulderIcon;
            leftShoulderIcon = originalLeftShoulderIcon;
            base.Reset();
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            if (pawn.story.bodyType != BodyTypeDefOf.Hulk)
            {
                originalBodyType = pawn.story.bodyType;
                pawn.story.bodyType = BodyTypeDefOf.Hulk;
            }
            base.Notify_Equipped(pawn);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            if (originalBodyType != null)
            {
                pawn.story.bodyType = originalBodyType;
            }
            base.Notify_Unequipped(pawn);
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref lastCheckRank, "lastCheckRank");
            Scribe_Defs.Look(ref leftShoulderIcon, "leftShoulderIcon");
            Scribe_Defs.Look(ref originalLeftShoulderIcon, "originalSelectedChapterIcon");
            Scribe_Defs.Look(ref rightShoulderIcon, "rightShoulderIcon");
            Scribe_Defs.Look(ref originalRightShoulderIcon, "originalRightShoulderIcon");
            Scribe_Defs.Look(ref originalBodyType, "originalBodyType");
            base.ExposeData();
        }
    }
}