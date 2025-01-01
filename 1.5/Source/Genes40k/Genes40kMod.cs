using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;


namespace Genes40k
{
    public class Genes40kMod : Mod
    {
        public static Harmony harmony;

        private Genes40kModSettings settings;

        public Genes40kModSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<Genes40kModSettings>();
                }

                return settings;
            }
        }

        public Genes40kMod(ModContentPack content) : base(content)
        {
            //settings = GetSettings<Genes40kModSettings>();
            harmony = new Harmony("Genes40k.Mod");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsychicPhenomena".Translate(), ref Settings.psychicPhenomena);
            
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PsykerPariahBirth".Translate(), ref Settings.psykerPariahBirth);
            if (Settings.psykerPariahBirth)
            {
                listingStandard.Label("BEWH.MankindsFinest.ModSettings.PsykerPariahBirthChance".Translate(Settings.psykerPariahBirthChance));
                Settings.psykerPariahBirthChance = (int)listingStandard.Slider(Settings.psykerPariahBirthChance, 0, 100);
            }
            
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.PerpetualBirth".Translate(), ref Settings.perpetualBirth);
            if (Settings.perpetualBirth)
            {
                listingStandard.Label("BEWH.MankindsFinest.ModSettings.PerpetualBirthChance".Translate(Settings.perpetualBirthChance));
                Settings.perpetualBirthChance = (int)listingStandard.Slider(Settings.perpetualBirthChance, 0, 100);
            }
            
            listingStandard.Gap();
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintChance".Translate());
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintBigThreat".Translate(Settings.livingSaintBigThreat));
            Settings.livingSaintBigThreat = (int)listingStandard.Slider(Settings.livingSaintBigThreat, 0, 100);
            listingStandard.Label("BEWH.MankindsFinest.ModSettings.LivingSaintSmallThreat".Translate(Settings.livingSaintSmallThreat));
            Settings.livingSaintSmallThreat = (int)listingStandard.Slider(Settings.livingSaintSmallThreat, 0, 100);
            
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("BEWH.MankindsFinest.ModSettings.UseChaosVersionForBanner".Translate(), ref Settings.useChaosVersion);
            listingStandard.Indent(inRect.width * 0.25f);
            if (listingStandard.ButtonText("BEWH.MankindsFinest.ModSettings.DefaultChapterColours".Translate(), widthPct: 0.5f))
            {
                Find.WindowStack.Add(new Dialog_ChangeDefaultChapterColour(Settings));
            }
            listingStandard.Outdent(inRect.width * 0.25f);

            listingStandard.Gap();
            listingStandard.Label("\n" + "BEWH.ModSettings.CheckVEFPatches".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "BEWH.MankindsFinest.ModSettings.ModName".Translate();
        }
    }
}