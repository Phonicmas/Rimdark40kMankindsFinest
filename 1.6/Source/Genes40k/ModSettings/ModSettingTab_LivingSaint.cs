using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_LivingSaint : ModSettingTab
{
    private Vector2 scrollPos;

    private float scrollViewHeight = 0f;
    private const float listingHeightIncrease = 24f;
    private const float listingHeightIncreaseGap = 36f;
    
    public override void DrawTab(Rect inRect, Genes40kModSettings settings)
    {
        var viewRect = new Rect(inRect.x, inRect.y, inRect.width - 16f, scrollViewHeight);
        scrollViewHeight = 0f;
            
        Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(viewRect);
        listingStandard.Gap(36);
        scrollViewHeight += listingHeightIncreaseGap;
        scrollViewHeight += listingHeightIncrease;
        
        //Living Saint System
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintSystem".Translate(), ref settings.livingSaintSystem);
        scrollViewHeight += listingHeightIncrease;
        if (settings.livingSaintSystem)
        {
            listingStandard.GapLine(36);
            scrollViewHeight += listingHeightIncreaseGap;
            settings.livingSaintLimit = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintPawnLimit".Translate(settings.livingSaintLimit), settings.livingSaintLimit, 1, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintPawnLimitDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintMale".Translate(), ref settings.livingSaintMale, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintMaleDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintAscensionLimit".Translate(), ref settings.livingSaintOnlyBaselinerAscension, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintAscensionLimitDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.GapLine(36);
            scrollViewHeight += listingHeightIncreaseGap;
            settings.livingSaintBaseChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintBaseChance".Translate(settings.livingSaintBaseChance), settings.livingSaintBaseChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintBaseChanceDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintExtraChanceFromViolence".Translate(), ref settings.livingSaintAddedChanceFromViolenceSkills, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintExtraChanceFromViolenceDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.GapLine(36);
            scrollViewHeight += listingHeightIncreaseGap;
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintChance".Translate());
            scrollViewHeight += listingHeightIncrease;
            settings.livingSaintBigThreat = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintBigThreat".Translate(settings.livingSaintBigThreat), settings.livingSaintBigThreat, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintBigThreatDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
            settings.livingSaintSmallThreat = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreat".Translate(settings.livingSaintSmallThreat), settings.livingSaintSmallThreat, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreatDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
        }
        scrollViewHeight += listingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}