using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Genes40kMod : Mod
{
    public static Harmony harmony;

    private Genes40kModSettings settings;

    public Genes40kModSettings Settings => settings ??= GetSettings<Genes40kModSettings>();

    public Genes40kMod(ModContentPack content) : base(content)
    {
        harmony = new Harmony("Genes40k.Mod");
        harmony.PatchAll();
    }
        
    private Vector2 scrollPos;

    private float scrollViewHeight = 0f;
    private const float listingHeightIncrease = 24f;
    private const float listingHeightIncreaseGap = 36f;
        
    public override void DoSettingsWindowContents(Rect inRect)
    {
        var viewRect = new Rect(inRect.x, inRect.y, inRect.width - 16f, scrollViewHeight);
        scrollViewHeight = 0f;
            
        Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(viewRect);
        scrollViewHeight += listingHeightIncrease;

        //Default Chapter Colour
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.DefaultChapterColoursDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        listingStandard.Indent(inRect.width * 0.25f);
        if (listingStandard.ButtonText("BEWH.MankindsFinest.ModSettings.DefaultChapterColours".Translate(Settings.CurrentlySelectedPreset.label), widthPct: 0.5f))
        {
            Find.WindowStack.Add(new Dialog_ChangeDefaultChapterColour(Settings));
        }
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Outdent(inRect.width * 0.25f);

        //Psychic Phenomena
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicPhenomena".Translate(), ref Settings.psychicPhenomena);
        scrollViewHeight += listingHeightIncrease;
  
        //Psyker/Pariah Birth
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirth".Translate(), ref Settings.psykerPariahBirth);
        scrollViewHeight += listingHeightIncrease;
        if (Settings.psykerPariahBirth)
        {
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChance".Translate(Settings.psykerPariahBirthChance));
            scrollViewHeight += listingHeightIncrease;
            Settings.psykerPariahBirthChance = (int)listingStandard.Slider(Settings.psykerPariahBirthChance, 0, 100);
            scrollViewHeight += listingHeightIncrease;
        }
            
        //Perpetual Birth
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirth".Translate(), ref Settings.perpetualBirth);
        scrollViewHeight += listingHeightIncrease;
        if (Settings.perpetualBirth)
        {
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.PerpetualBirthChance".Translate(Settings.perpetualBirthChance));
            scrollViewHeight += listingHeightIncrease;
            Settings.perpetualBirthChance = (int)listingStandard.Slider(Settings.perpetualBirthChance, 0, 100);
            scrollViewHeight += listingHeightIncrease;
        }

        //Living Saint System
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintSystem".Translate(), ref Settings.livingSaintSystem);
        scrollViewHeight += listingHeightIncrease;
        if (Settings.livingSaintSystem)
        {
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintMale".Translate(), ref Settings.livingSaintMale);
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.LivingSaintExtraChanceFromViolence".Translate(), ref Settings.livingSaintAddedChanceFromViolenceSkills);
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintChance".Translate());
            scrollViewHeight += listingHeightIncrease;
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintBigThreat".Translate(Settings.livingSaintBigThreat));
            scrollViewHeight += listingHeightIncrease;
            Settings.livingSaintBigThreat = (int)listingStandard.Slider(Settings.livingSaintBigThreat, 0, 100);
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreat".Translate(Settings.livingSaintSmallThreat));
            scrollViewHeight += listingHeightIncrease;
            Settings.livingSaintSmallThreat = (int)listingStandard.Slider(Settings.livingSaintSmallThreat, 0, 100);
            scrollViewHeight += listingHeightIncrease;
            
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintBaseChance".Translate(Settings.livingSaintBaseChance));
            scrollViewHeight += listingHeightIncrease;
            Settings.livingSaintBaseChance = (int)listingStandard.Slider(Settings.livingSaintBaseChance, 0, 100);
            scrollViewHeight += listingHeightIncrease;    
            
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintPawnLimit".Translate(Settings.livingSaintLimit));
            scrollViewHeight += listingHeightIncrease;
            Settings.livingSaintLimit = (int)listingStandard.Slider(Settings.livingSaintLimit, 1, 100);
            scrollViewHeight += listingHeightIncrease;
        }

        //Geneseed Implantation Offset
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.ImplantationDesc".Translate());
        scrollViewHeight += listingHeightIncrease;
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.ImplantationSuccessChange".Translate(Settings.implantationSuccessOffset));
        scrollViewHeight += listingHeightIncrease;
        Settings.implantationSuccessOffset = (int)listingStandard.Slider(Settings.implantationSuccessOffset, -200, 200);
        scrollViewHeight += listingHeightIncrease;
        //Geneseed Implantation Cap Offset
        listingStandard.Label("BEWH.MankindsFinest.ModSettings.ImplantationCapOffset".Translate(Settings.implantationCapOffset));
        scrollViewHeight += listingHeightIncrease;
        Settings.implantationCapOffset = (int)listingStandard.Slider(Settings.implantationCapOffset, -100, 100);
        scrollViewHeight += listingHeightIncrease;

        //Only male Primarch
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.AllowFemalePrimarchs".Translate(), ref Settings.allowFemalePrimarchBirths);
        scrollViewHeight += listingHeightIncrease;
        
        //Psychic Crafting
        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicCrafting".Translate(), ref Settings.psychicCrafting);
        scrollViewHeight += listingHeightIncrease;
        scrollViewHeight += listingHeightIncrease;

        listingStandard.GapLine(36);
        scrollViewHeight += listingHeightIncreaseGap;
        listingStandard.Label("\n" + "BEWH.ModSettings.CheckVEFPatches".Translate());
        scrollViewHeight += listingHeightIncrease;
        listingStandard.Gap(24);
        scrollViewHeight += listingHeightIncrease;
        listingStandard.End();
        
        Widgets.EndScrollView();
    }

    public override string SettingsCategory()
    {
        return "BEWH.MankindsFinest.ModSettings.ModName".Translate();
    }
}