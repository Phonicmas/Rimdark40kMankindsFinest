using Core40k;
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
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            DrawColor = ModSettings?.chapterColorOne ?? base.DrawColor;
            SetSecondaryColor(ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
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

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref lastCheckRank, "lastCheckRank");
            Scribe_Defs.Look(ref leftShoulderIcon, "leftShoulderIcon");
            Scribe_Defs.Look(ref originalLeftShoulderIcon, "originalSelectedChapterIcon");
            Scribe_Defs.Look(ref rightShoulderIcon, "rightShoulderIcon");
            Scribe_Defs.Look(ref originalRightShoulderIcon, "originalRightShoulderIcon");
            base.ExposeData();
        }
    }
}