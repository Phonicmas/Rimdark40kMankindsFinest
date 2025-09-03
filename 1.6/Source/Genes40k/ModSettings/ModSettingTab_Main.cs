using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_Main : ModSettingTab
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

        //Psychic Crafting
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicCrafting".Translate(), ref settings.psychicCrafting, tooltip: "BEWH.MankindsFinest.ModSettings.PsychicCraftingDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        scrollViewHeight += listingHeightIncrease;
        
        //Check VEF patches
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Label("\n" + "BEWH.ModSettings.CheckVEFPatches".Translate());
        scrollViewHeight += listingHeightIncrease;
        
        scrollViewHeight += listingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}