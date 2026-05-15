using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_LivingSaint : ModSettingTab
{
    public override void DrawTab(Rect inRect, ModSettings settings)
    {
        if (settings is not Genes40kModSettings genes40KModSettings)
        {
            Log.Error("Settings not correct type");
            return;
        }
        
        var viewRect = new Rect(inRect.x, inRect.y, inRect.width - 16f, scrollViewHeight);
        scrollViewHeight = 0f;
            
        Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(viewRect);
        listingStandard.Gap(36);
        scrollViewHeight += ListingHeightIncreaseGap;
        scrollViewHeight += ListingHeightIncrease;
        
        //Living Saint System
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintSystem".Translate(), ref genes40KModSettings.livingSaintSystem);
        scrollViewHeight += ListingHeightIncrease;
        if (genes40KModSettings.livingSaintSystem)
        {
            listingStandard.GapLine(36);
            scrollViewHeight += ListingHeightIncreaseGap;
            genes40KModSettings.livingSaintLimit = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintPawnLimit".Translate(genes40KModSettings.livingSaintLimit), genes40KModSettings.livingSaintLimit, 1, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintPawnLimitDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintMale".Translate(), ref genes40KModSettings.livingSaintMale, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintMaleDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintAscensionLimit".Translate(), ref genes40KModSettings.livingSaintOnlyBaselinerAscension, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintAscensionLimitDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            
            listingStandard.GapLine(36);
            scrollViewHeight += ListingHeightIncreaseGap;
            genes40KModSettings.livingSaintBaseChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintBaseChance".Translate(genes40KModSettings.livingSaintBaseChance), genes40KModSettings.livingSaintBaseChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintBaseChanceDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintExtraChanceFromViolence".Translate(), ref genes40KModSettings.livingSaintAddedChanceFromViolenceSkills, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintExtraChanceFromViolenceDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            
            listingStandard.GapLine(36);
            scrollViewHeight += ListingHeightIncreaseGap;
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintChance".Translate());
            scrollViewHeight += ListingHeightIncrease;
            genes40KModSettings.livingSaintBigThreat = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintBigThreat".Translate(genes40KModSettings.livingSaintBigThreat), genes40KModSettings.livingSaintBigThreat, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintBigThreatDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
            genes40KModSettings.livingSaintSmallThreat = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreat".Translate(genes40KModSettings.livingSaintSmallThreat), genes40KModSettings.livingSaintSmallThreat, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreatDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
        }
        scrollViewHeight += ListingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}