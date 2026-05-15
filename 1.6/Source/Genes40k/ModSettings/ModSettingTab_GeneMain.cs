using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_GeneMain : ModSettingTab
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

        //Psychic Crafting
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicCrafting".Translate(), ref genes40KModSettings.psychicCrafting, tooltip: "BEWH.MankindsFinest.ModSettings.PsychicCraftingDesc".Translate());
        scrollViewHeight += ListingHeightIncrease;
        scrollViewHeight += ListingHeightIncrease;
        
        //Check VEF patches
        listingStandard.GapLine(36);
        scrollViewHeight += ListingHeightIncreaseGap;
        listingStandard.Label("\n" + "BEWH.ModSettings.CheckVEFPatches".Translate());
        scrollViewHeight += ListingHeightIncrease;
        
        scrollViewHeight += ListingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}