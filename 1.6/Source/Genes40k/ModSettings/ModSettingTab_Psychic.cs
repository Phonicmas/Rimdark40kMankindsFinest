using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_Psychic : ModSettingTab
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

        //Psychic Phenomena
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicPhenomena".Translate(), ref settings.psychicPhenomena, tooltip: "BEWH.MankindsFinest.ModSettings.PsychicPhenomenaDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
  
        //Psyker/Pariah Birth
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirth".Translate(), ref settings.psykerPariahBirth);
        scrollViewHeight += listingHeightIncrease;
        if (settings.psykerPariahBirth)
        {
            settings.psykerPariahBirthChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChance".Translate(settings.psykerPariahBirthChance), settings.psykerPariahBirthChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChanceDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
        }
            
        //Perpetual Birth
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirth".Translate(), ref settings.perpetualBirth);
        scrollViewHeight += listingHeightIncrease;
        if (settings.perpetualBirth)
        {
            settings.perpetualBirthChance = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirthChance".Translate(settings.perpetualBirthChance), settings.perpetualBirthChance, 0, 100, tooltip: "BEWH.MankindsFinest.ModSettings.PerpetualBirthChanceDesc".Translate());
            scrollViewHeight += listingHeightIncrease;
        }
        
        scrollViewHeight += listingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}