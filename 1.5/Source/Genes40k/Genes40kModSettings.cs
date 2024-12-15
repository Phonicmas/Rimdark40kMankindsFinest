using UnityEngine;
using Verse;


namespace Genes40k
{
    public class Genes40kModSettings : ModSettings
    {
        public bool psychicPhenomena = true;
        public bool psykerPariahBirth = true;
        public int psykerPariahBirthChance = 10;
        
        public bool perpetualBirth = true;
        public int perpetualBirthChance = 3;

        public int livingSaintBigThreat = 65;
        public int livingSaintSmallThreat = 35;
        
        public ChapterColourDef currentlySelectedPreset = null;
        public Color chapterColorOne = Color.black;
        public Color chapterColorTwo = Color.red;
        
        public bool useChaosVersion = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref psychicPhenomena, "psychicPhenomena", true);
            Scribe_Values.Look(ref psykerPariahBirth, "psykerPariahBirth", true);
            Scribe_Values.Look(ref psykerPariahBirthChance, "psykerPariahBirthChance", 10);
            Scribe_Values.Look(ref perpetualBirthChance, "perpetualBirthChance", 3);
            Scribe_Values.Look(ref chapterColorOne, "chapterColorOne", Color.black);
            Scribe_Values.Look(ref chapterColorTwo, "chapterColorTwo", Color.red);
            Scribe_Values.Look(ref useChaosVersion, "useChaosVersion", false);
            Scribe_Defs.Look(ref currentlySelectedPreset, "currentlySelectedPreset");
            base.ExposeData();
        }
    }
}