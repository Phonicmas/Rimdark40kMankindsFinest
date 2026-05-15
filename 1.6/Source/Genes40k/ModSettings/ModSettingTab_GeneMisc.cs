using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_GeneMisc : ModSettingTab
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

        //Only male Primarch
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.AllowFemalePrimarchs".Translate(), ref genes40KModSettings.allowFemalePrimarchBirths);
        scrollViewHeight += ListingHeightIncrease;
        
        scrollViewHeight += ListingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}