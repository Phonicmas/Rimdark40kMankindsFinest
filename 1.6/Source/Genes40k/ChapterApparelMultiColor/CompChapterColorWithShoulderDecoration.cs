using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class CompChapterColorWithShoulderDecoration : CompChapterColor
{
    public void TempSetInitialValues(ChapterBodyDecorativeApparelMultiColor multiColor)
    {
        rightShoulder = multiColor.RightShoulderTemp;
        originalRightShoulder = multiColor.RightShoulderTemp;
        
        leftShoulder = multiColor.LeftShoulderTemp;
        originalLeftShoulder = multiColor.LeftShoulderTemp;
        
        flipShoulderIcons = multiColor.FlipShoulderIcons;
    }
    
    public new CompProperties_ChapterColorWithShoulderDecoration Props => (CompProperties_ChapterColorWithShoulderDecoration)props;

    [Unsaved]
    private CompRankInfo rankInfoComp = null;
    private CompRankInfo RankInfoComp
    {
        get
        {
            if (rankInfoComp == null && Wearer != null)
            {
                rankInfoComp = Wearer.GetComp<CompRankInfo>();
            }

            return rankInfoComp;
        }
    }
        
    private ShoulderIconSettings originalRightShoulder = null;
    private ShoulderIconSettings rightShoulder = null;
    public ShoulderIconDef RightShoulderIcon
    {
        get
        {
            if (rightShoulder == null)
            {
                return null;
            }
            
            if (rightShoulder.ShoulderIcon != null || RankInfoComp == null)
            {
                return rightShoulder.ShoulderIcon;
            }
                
            //When pawn ranks up their icon does not auto get correct colour!!
            
            var highestRankDef = RankInfoComp.HighestRankDef(true, Genes40kDefOf.BEWH_AstartesRankCategory) ?? RankInfoComp.HighestRankDef(false, Genes40kDefOf.BEWH_AstartesRankCategory);
            return ((ChapterRankDef)highestRankDef)?.unlocksRankIcon;
        }
        set
        {
            if (value != null && value.setsNull)
            {
                rightShoulder = new ShoulderIconSettings();
                var highestRankDef = RankInfoComp.HighestRankDef(true, Genes40kDefOf.BEWH_AstartesRankCategory) ?? RankInfoComp.HighestRankDef(false, Genes40kDefOf.BEWH_AstartesRankCategory);
                rightShoulder.Color = ((ChapterRankDef)highestRankDef)?.unlocksRankIcon?.defaultColour ?? Color.white;
            }
            else
            {
                rightShoulder ??= new ShoulderIconSettings();
                rightShoulder.ShoulderIcon = value;
                rightShoulder.Color = RightShoulderIcon?.defaultColour ?? Color.white;
            }
            
            Notify_GraphicChanged();
        }
    }
    public Color RightShoulderIconColour
    {
        get => rightShoulder?.Color ?? RightShoulderIcon?.defaultColour ?? Color.white;
        set
        {
            rightShoulder ??= new ShoulderIconSettings();
            rightShoulder.Color = value;
            Notify_GraphicChanged();
        }
    }
    

    private ShoulderIconSettings originalLeftShoulder = null;
    private ShoulderIconSettings leftShoulder = null;
    public ShoulderIconDef LeftShoulderIcon
    {
        get => leftShoulder?.ShoulderIcon;
        set
        {
            if (value != null && value.setsNull)
            {
                leftShoulder = new ShoulderIconSettings();
            }
            else
            {
                leftShoulder ??= new ShoulderIconSettings();
                leftShoulder.ShoulderIcon = value;
                leftShoulder.Color = LeftShoulderIcon?.defaultColour ?? Color.white;
            }
            
            Notify_GraphicChanged();
        }
    }
    public Color LeftShoulderIconColour 
    {
        get => leftShoulder?.Color ?? LeftShoulderIcon?.defaultColour ?? Color.white;
        set
        {
            leftShoulder ??= new ShoulderIconSettings();
            leftShoulder.Color = value;
            Notify_GraphicChanged();
        }
    }
    

    private bool flipShoulderIcons = false;
    public bool FlipShoulderIcons
    {
        get => flipShoulderIcons;
        set
        {
            flipShoulderIcons = value;
            Notify_GraphicChanged();
        }
    }
    
    public override void InitialColors()
    {
        base.InitialColors();
        SetUpMisc();
    }
        
    private void SetUpMisc()
    {
        leftShoulder = new ShoulderIconSettings()
        {
            ShoulderIcon = ModSettings?.CurrentlySelectedPreset.relatedChapterIcon,
            Color = ModSettings?.chapterShoulderIconColor ?? Color.white,
        };
        rightShoulder = null;
    }
        
    public override void SetOriginals()
    {
        originalRightShoulder = rightShoulder != null ? new ShoulderIconSettings(rightShoulder) : rightShoulder;
        originalLeftShoulder = leftShoulder != null ? new ShoulderIconSettings(leftShoulder) : leftShoulder;
        base.SetOriginals();
    }
        
    public override void Reset()    
    {
        rightShoulder = originalRightShoulder != null ? new ShoulderIconSettings(originalRightShoulder) : originalRightShoulder;
        leftShoulder = originalLeftShoulder != null ? new ShoulderIconSettings(originalLeftShoulder) : originalLeftShoulder;
        base.Reset();
    }
    
    public override void PostExposeData()
    {
        Scribe_Deep.Look(ref originalRightShoulder, "originalRightShoulder");
        Scribe_Deep.Look(ref rightShoulder, "rightShoulder");
        
        Scribe_Deep.Look(ref originalLeftShoulder, "originalLeftShoulder");
        Scribe_Deep.Look(ref leftShoulder, "leftShoulder");
        
        Scribe_Values.Look(ref flipShoulderIcons, "flipShoulderIcons", false);
        base.PostExposeData();
    }
}