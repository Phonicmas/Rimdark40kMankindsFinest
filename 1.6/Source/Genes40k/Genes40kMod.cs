using System.Collections.Generic;
using Core40k;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Genes40kMod : CoreMod
{
    public static string CurrentVersion;
        
    public static Harmony harmony;

    private Genes40kModSettings settings;
    public override ModSettings Settings => settings ??= GetSettings<Genes40kModSettings>();

    public Genes40kMod(ModContentPack content) : base(content)
    {
        harmony = new Harmony("Genes40k.Mod");
        CurrentVersion = content.ModMetaData.ModVersion;
        
        HarmonyPatch();
    }

    private static void HarmonyPatch()
    {
        if (ModLister.GetActiveModWithIdentifier("hlx.UltratechAlteredCarbon") != null)
        {
            harmony.Patch(AccessTools.Method(AccessTools.TypeByName("AlteredCarbon.Recipe_RemoveNeuralStack"), "ApplyOnPawn"), prefix: new HarmonyMethod(typeof(ManualHarmonyPatches), "StackRemovalDontTriggerPerpetual"));
        }
        
        harmony.PatchAllUncategorized();
    }

    private readonly ModSettingTab_GeneMain geneMainSettings = new();
    private readonly ModSettingTab_Geneseed geneseedSettings = new();
    private readonly ModSettingTab_LivingSaint livingSaintSettings = new();
    private readonly ModSettingTab_GeneMisc geneMiscSettings = new();
    private readonly ModSettingTab_Psychic psychicSettings = new();
    
    public override void InitializeTabs()
    {
        var mainTab = new TabRecord("BEWH.ModSettings.TabMain".Translate(), delegate
        {
            currentSettingTab = geneMainSettings;
        }, () => currentSettingTab == geneMainSettings);
        tabs.Add(mainTab);
        
        var geneseedTab = new TabRecord("BEWH.MankindsFinest.ModSettings.TabGeneseed".Translate(), delegate
        {
            currentSettingTab = geneseedSettings;
        }, () => currentSettingTab == geneseedSettings);
        tabs.Add(geneseedTab);
        
        var psychicTab = new TabRecord("BEWH.MankindsFinest.ModSettings.TabPsychic".Translate(), delegate
        {
            currentSettingTab = psychicSettings;
        }, () => currentSettingTab == psychicSettings);
        tabs.Add(psychicTab);
        
        var livingSaintTab = new TabRecord("BEWH.MankindsFinest.ModSettings.TabLivingSaint".Translate(), delegate
        {
            currentSettingTab = livingSaintSettings;
        }, () => currentSettingTab == livingSaintSettings);
        tabs.Add(livingSaintTab);
        
        var miscTab = new TabRecord("BEWH.ModSettings.TabMisc".Translate(), delegate
        {
            currentSettingTab = geneMiscSettings;
        }, () => currentSettingTab == geneMiscSettings);
        tabs.Add(miscTab);

        currentSettingTab = geneMainSettings;
        base.InitializeTabs();
    }

    public override string SettingsCategory()
    {
        return "BEWH.MankindsFinest.ModSettings.ModName".Translate(CurrentVersion);
    }
}