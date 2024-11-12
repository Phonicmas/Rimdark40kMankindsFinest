﻿using HarmonyLib;
using UnityEngine;
using Verse;


namespace Genes40k
{
    public class Genes40kMod : Mod
    {
        public static Harmony harmony;

        readonly Genes40kModSettings settings;

        public Genes40kMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Genes40kModSettings>();
            harmony = new Harmony("Genes40k.Mod");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled("BEWH.PsychicPhenomena".Translate(), ref settings.psychicPhenomena);

            listingStandard.CheckboxLabeled("BEWH.PsykerPariahBirth".Translate(), ref settings.psykerPariahBirth);

            if (settings.psykerPariahBirth)
            {
                listingStandard.Label("BEWH.PsykerPariahBirthChance".Translate(settings.psykerPariahBirthChance));
                settings.psykerPariahBirthChance = (int)listingStandard.Slider(settings.psykerPariahBirthChance, 0, 100);
            }
            
            listingStandard.CheckboxLabeled("BEWH.PerpetualBirth".Translate(), ref settings.perpetualBirth);
            
            if (settings.perpetualBirth)
            {
                listingStandard.Label("BEWH.PerpetualBirthChance".Translate(settings.perpetualBirthChance));
                settings.perpetualBirthChance = (int)listingStandard.Slider(settings.perpetualBirthChance, 0, 100);
            }

            listingStandard.Label("\n" + "BEWH.CheckVEFPatches".Translate());

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "BEWH.ModSettingsNameGenes".Translate();
        }
    }
}