using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Genes40kMod : Mod
{
    public string version = "1.0.0";
        
    public static Harmony harmony;

    private Genes40kModSettings settings;
    public Genes40kModSettings Settings => settings ??= GetSettings<Genes40kModSettings>();

    public Genes40kMod(ModContentPack content) : base(content)
    {
        harmony = new Harmony("Genes40k.Mod");
        harmony.PatchAll();
    }

    private static List<TabRecord> tabs = new List<TabRecord>();
    private bool tabsInitialized = false;
    private static ModSettingTab currentSettingTab;
    
    public ModSettingTab_Main mainSettings = new ModSettingTab_Main();
    public ModSettingTab_Geneseed geneseedSettings = new ModSettingTab_Geneseed();
    public ModSettingTab_LivingSaint livingSaintSettings = new ModSettingTab_LivingSaint();
    public ModSettingTab_Misc miscSettings = new ModSettingTab_Misc();
    public ModSettingTab_Psychic psychicSettings = new ModSettingTab_Psychic();
    
    public void InitializeTabs()
    {
        tabs = new List<TabRecord>();

        var mainTab = new TabRecord("BEWH.MankindsFinest.ModSettings.TabMain".Translate(), delegate
        {
            currentSettingTab = mainSettings;
        }, () => currentSettingTab == mainSettings);
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
        
        var miscTab = new TabRecord("BEWH.MankindsFinest.ModSettings.TabMisc".Translate(), delegate
        {
            currentSettingTab = miscSettings;
        }, () => currentSettingTab == miscSettings);
        tabs.Add(miscTab);

        currentSettingTab = mainSettings;
        tabsInitialized = true;
    }
    
    public override void DoSettingsWindowContents(Rect inRect)
    {
        if (!tabsInitialized)
        {
            InitializeTabs();
        }
        base.DoSettingsWindowContents(inRect);
        var menuRect = inRect.ContractedBy(10f);
        menuRect.y += 20f;
        menuRect.height -= 20f;
        Widgets.DrawMenuSection(menuRect);
        TabDrawer.DrawTabs(menuRect, tabs);
        currentSettingTab.DrawTab(menuRect.ContractedBy(5f), Settings);
    }

    public override string SettingsCategory()
    {
        return "BEWH.MankindsFinest.ModSettings.ModName".Translate(version);
    }
}