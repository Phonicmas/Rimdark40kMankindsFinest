﻿using RimWorld;
using Verse;


namespace Genes40k
{
    [DefOf]
    public static class Genes40kDefOf
    {
        //Thunder warrior genes
        public static GeneDef BEWH_ProtoOssmodula;
        public static GeneDef BEWH_Musculeator;
        public static GeneDef BEWH_Mentanifex;
        public static GeneDef BEWH_Vigoranis;
        public static GeneDef BEWH_Hyperanatomica;
        public static GeneDef BEWH_Furybound;

        //Space marine genes
        public static GeneDef BEWH_SecondaryHeart;
        public static GeneDef BEWH_Ossmodula;
        public static GeneDef BEWH_Biscopea;
        public static GeneDef BEWH_Haemastamen;
        public static GeneDef BEWH_LarramansOrgan;
        public static GeneDef BEWH_CatalepseanNode;
        public static GeneDef BEWH_Preomnor;
        public static GeneDef BEWH_Omophagea;
        public static GeneDef BEWH_MultiLung;
        public static GeneDef BEWH_Occulobe;
        public static GeneDef BEWH_LymansEar;
        public static GeneDef BEWH_SusAnMembrane;
        public static GeneDef BEWH_Melanochrome;
        public static GeneDef BEWH_OoliticKidney;
        public static GeneDef BEWH_Neuroglottis;
        public static GeneDef BEWH_Mucranoid;
        public static GeneDef BEWH_BetchersGland;
        public static GeneDef BEWH_ProgenoidGlands;
        public static GeneDef BEWH_BlackCarapace;

        //Primaris genes
        public static GeneDef BEWH_SinewCoil;
        public static GeneDef BEWH_Magnificat;
        public static GeneDef BEWH_BelisarianFurnace;

        //Custodes genes
        public static GeneDef BEWH_ImmunisLeucocyte;
        public static GeneDef BEWH_AthanaticVitae;
        public static GeneDef BEWH_FulguriteNervePlexus;
        public static GeneDef BEWH_AtlasMorphogen;
        public static GeneDef BEWH_MnemosyneMindshield;
        public static GeneDef BEWH_FulgurVitaliumstrand;

        //Priamrch genes
        public static GeneDef BEWH_ImmortisGland;
        public static GeneDef BEWH_TempestusOcularium;
        public static GeneDef BEWH_ThalaxCortex;
        public static GeneDef BEWH_HelixomeArray;
        public static GeneDef BEWH_VermillionCache;
        public static GeneDef BEWH_CelerityNexus;
        public static GeneDef BEWH_HyperionMuscleStrands;

        //Psyker genes
        public static GeneDef BEWH_IotaPsyker;
        public static GeneDef BEWH_Psyker;
        public static GeneDef BEWH_DeltaPsyker;
        public static GeneDef BEWH_BetaPsyker;
        
        //Pariah genes
        public static GeneDef BEWH_SigmaPariah;
        public static GeneDef BEWH_UpsilonPariah;
        public static GeneDef BEWH_OmegaPariah;

        //Living saint genes
        public static GeneDef BEWH_LivingSaintBeingOfFaith;
        public static GeneDef BEWH_LivingSaintHolyRadiance;

        //Chapter specific genes
        public static GeneDef BEWH_ChapterXVThousandSons;

        //Xenotype
        public static XenotypeDef BEWH_LivingSaint;

        //Researchprojects
        public static ResearchProjectDef BEWH_GeneseedExtractionFirstborn;
        public static ResearchProjectDef BEWH_GeneseedExtractionPrimaris;

        //Heddifs
        public static HediffDef BEWH_SecondGeneseedHarvest;
        public static HediffDef BEWH_PsychicComa;
        public static HediffDef BEWH_PsychicConnectionSevered;
        public static HediffDef BEWH_PsychicCrafting;
        public static HediffDef BEWH_DeniedWitch;

        public static HediffDef BEWH_PariahEffecter;

        //Letters
        public static LetterDef BEWH_NaturalBornX;
        public static LetterDef BEWH_GoldenPositive;

        //DamageDefs
        public static DamageDef BEWH_WarpEnergy;

        //WeatherDefs
        public static WeatherDef BEWH_BloodRain;

        //ThingDefs
        public static ThingDef BEWH_GeneseedGestator;
        public static ThingDef BEWH_GeneticCryostaticStorage;
        public static ThingDef BEWH_PrimarchEmbryoContainer;
        public static ThingDef BEWH_SangprimusPortum;
        public static ThingDef BEWH_PrimarchGrowthVat;

        public static ThingDef BEWH_GeneseedVialFirstborn;
        public static ThingDef BEWH_GeneseedVialPrimaris;

        public static ThingDef BEWH_PrimarchEmbryo;

        //JobDefs
        public static JobDef BEWH_FillGeneGestator;
        public static JobDef BEWH_FillPrimarchGrowthVat;

        //ThoughtDefs
        public static ThoughtDef BEWH_LivingSaintHolyRadianceThought;

        static Genes40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Genes40kDefOf));
        }
    }
}