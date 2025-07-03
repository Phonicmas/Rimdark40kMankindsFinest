using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ChapterBodyDecorativeApparelColourTwo : BodyDecorativeApparelColourTwo
{
    private Genes40kModSettings modSettings = null;

    protected Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
        
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
                
            var highestRankDef = RankInfoComp.HighestRankDef(true, Genes40kDefOf.BEWH_AstartesRankCategory) ?? RankInfoComp.HighestRankDef(false, Genes40kDefOf.BEWH_AstartesRankCategory);
            return ((ChapterRankDef)highestRankDef)?.unlocksRankIcon;
        }
        set
        {
            if (value != null && value.setsNull)
            {
                rightShoulder = new ShoulderIconSettings();
            }
            else
            {
                rightShoulder ??= new ShoulderIconSettings();
                rightShoulder.ShoulderIcon = value;
                rightShoulder.Color = RightShoulderIcon?.defaultColour ?? Color.white;
            }
            
            Notify_ColorChanged();
        }
    }
    public Color RightShoulderIconColour
    {
        get => rightShoulder?.Color ?? RightShoulderIcon?.defaultColour ?? Color.white;
        set
        {
            rightShoulder ??= new ShoulderIconSettings();
            rightShoulder.Color = value;
            Notify_ColorChanged();
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
            
            Notify_ColorChanged();
        }
    }
    public Color LeftShoulderIconColour => leftShoulder?.Color ?? LeftShoulderIcon?.defaultColour ?? Color.white;
    

    private bool flipShoulderIcons = false;
    public bool FlipShoulderIcons
    {
        get => flipShoulderIcons;
        set
        {
            flipShoulderIcons = value;
            Notify_ColorChanged();
        }
    }
    
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (InitialColourSet)
        {
            return;
        }
        
        SetInitialColours(ModSettings?.chapterColorOne ?? base.DrawColor, ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
        SetUpMisc();
    }
        
    private void SetUpMisc()
    {
        leftShoulder = new ShoulderIconSettings()
        {
            ShoulderIcon = ModSettings?.CurrentlySelectedPreset.relatedChapterIcon,
        };
        rightShoulder = null;
    }
        
    public override void SetOriginals()
    {
        originalRightShoulder = rightShoulder;
        originalLeftShoulder = leftShoulder;
            
        base.SetOriginals();
    }
        
    public override void Reset()
    {
        rightShoulder = originalRightShoulder;
        leftShoulder = originalLeftShoulder;
            
        base.Reset();
    }
    
    public override void ExposeData()
    {
        Scribe_Deep.Look(ref originalRightShoulder, "originalRightShoulder");
        Scribe_Deep.Look(ref rightShoulder, "rightShoulder");
        Scribe_Deep.Look(ref originalLeftShoulder, "originalLeftShoulder");
        Scribe_Deep.Look(ref leftShoulder, "leftShoulder");
        
        Scribe_Values.Look(ref flipShoulderIcons, "flipShoulderIcons", false);
        
        base.ExposeData();
    }
}