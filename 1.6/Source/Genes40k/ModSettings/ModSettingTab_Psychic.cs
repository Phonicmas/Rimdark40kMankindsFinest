using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_Psychic : ModSettingTab
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

        //Psychic Phenomena
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicPhenomena".Translate(), ref genes40KModSettings.psychicPhenomena, tooltip: "BEWH.MankindsFinest.ModSettings.PsychicPhenomenaDesc".Translate());
        scrollViewHeight += ListingHeightIncrease;
  
        //Psyker/Pariah Birth
        listingStandard.GapLine(36);
        scrollViewHeight += ListingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirth".Translate(), ref genes40KModSettings.psykerPariahBirth);
        scrollViewHeight += ListingHeightIncrease;
        if (genes40KModSettings.psykerPariahBirth)
        {
            genes40KModSettings.psykerPariahBirthChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChance".Translate(genes40KModSettings.psykerPariahBirthChance), genes40KModSettings.psykerPariahBirthChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChanceDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
        }
            
        //Perpetual Birth
        listingStandard.GapLine(36);
        scrollViewHeight += ListingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirth".Translate(), ref genes40KModSettings.perpetualBirth);
        scrollViewHeight += ListingHeightIncrease;
        if (genes40KModSettings.perpetualBirth)
        {
            genes40KModSettings.perpetualBirthChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirthChance".Translate(genes40KModSettings.perpetualBirthChance), genes40KModSettings.perpetualBirthChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.PerpetualBirthChanceDesc".Translate());
            scrollViewHeight += ListingHeightIncrease;
        }
        
        scrollViewHeight += ListingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}