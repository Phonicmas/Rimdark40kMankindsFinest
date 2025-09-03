using UnityEngine;
using Verse;

namespace Genes40k;

public class ModSettingTab_Geneseed : ModSettingTab
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
        
        //Default Chapter Colour
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.DefaultChapterColoursDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        listingStandard.Indent(inRect.width * 0.25f);
        if (listingStandard.ButtonText("BEWH.MankindsFinest.ModSettings.DefaultChapterColours".Translate(settings.CurrentlySelectedPreset.label), widthPct: 0.5f))
        {
            Find.WindowStack.Add(new Dialog_ChangeDefaultChapterColour(settings));
        }
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Outdent(inRect.width * 0.25f);
        
        //Geneseed Implantation Offset
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.ImplantationDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        //Geneseed Implantation Success Offset
        settings.implantationSuccessOffset = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.ImplantationSuccessOffset".Translate(settings.implantationSuccessOffset),settings.implantationSuccessOffset, -200, 200, tooltip: "BEWH.MankindsFinest.ModSettings.ImplantationSuccessOffsetDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        //Geneseed Implantation Cap Offset
        settings.implantationCapOffset = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.ImplantationCapOffset".Translate(settings.implantationCapOffset), settings.implantationCapOffset, -100, 100, tooltip: "BEWH.MankindsFinest.ModSettings.ImplantationCapOffsetDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        
        //Matrix gestation time factor
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        settings.matrixGestationTimeFactor = (int)listingStandard.SliderLabeled("BEWH.MankindsFinest.ModSettings.MatrixGestationFactor".Translate(settings.matrixGestationTimeFactor), settings.matrixGestationTimeFactor, 0, 200, tooltip: "BEWH.MankindsFinest.ModSettings.MatrixGestationFactorDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        
        scrollViewHeight += listingHeightIncrease;
        
        listingStandard.End();
        Widgets.EndScrollView();
    }
}