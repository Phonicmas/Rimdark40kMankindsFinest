using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public static class NaturalBirthPsykerPariah
{
    static NaturalBirthPsykerPariah()
    {
        Genes40kMod.harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ApplyBirthOutcome"), null, new HarmonyMethod(AccessTools.Method(typeof(NaturalBirthPsykerPariah), "Postfix")));
    }

    public static void Postfix(ref Thing __result, Pawn geneticMother)
    {
        if (__result == null)
        {
            return;
        }
        var modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
        if (!modSettings.psykerPariahBirth)
        {
            return;
        }
        var pawn = (Pawn)__result;
        if (pawn.Faction != Faction.OfPlayer)
        {
            return;
        }

        var unnaturalChance = modSettings.psykerPariahBirthChance;
        var rand = new Random();
        if (rand.Next(0, 100) > unnaturalChance)
        {
            return;
        }

        if (pawn.genes == null || Enumerable.Any(pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_Pariah>() || gene.def.HasModExtension<DefModExtension_Psyker>()))
        {
            return;
        }

        var canBecomePsyker = true;
        var canBecomePariah = true;
        foreach (var gene in pawn.genes.GenesListForReading)
        {
            if (gene.def.exclusionTags.Contains("Psyker"))
            {
                canBecomePsyker = false;
            }
            if (gene.def.exclusionTags.Contains("Pariah"))
            {
                canBecomePariah = false;
            }
        }

        if (!canBecomePsyker && !canBecomePariah)
        {
            return;
        }
            
        var weightedSelection = new WeightedSelection<GeneDef>();
        
        if (canBecomePsyker)
        {
            //Psyker genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_IotaPsyker, 60);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_EpsilonPsyker, 40);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_DeltaPsyker, 12);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_BetaPsyker, 4);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_AlphaPsyker, 1);
        }

        if (canBecomePariah)
        {
            //Pariah Genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_SigmaPariah, 40);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_UpsilonPariah, 12);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_OmegaPariah, 4);
        }
        
        var chosenGene = weightedSelection.GetRandomUnique();
        var typeBorn = chosenGene.HasModExtension<DefModExtension_Pariah>() ? "BEWH.MankindsFinest.CommonKeywords.Pariah".Translate() : "BEWH.MankindsFinest.CommonKeywords.Psyker".Translate();

        var geneticMomName = geneticMother?.Named("PAWN") ?? "BEWH.MankindsFinest.CommonKeywords.Unknown".Translate();
        
        var letter = new StandardLetter
        {
            lookTargets = pawn,
            def = Genes40kDefOf.BEWH_NaturalBornX,
            Text = "BEWH.MankindsFinest.Event.NaturalBornXMessage".Translate(geneticMomName, pawn.Named("PAWN"), typeBorn),
            Label = "BEWH.MankindsFinest.Event.NaturalBornXLetter".Translate(typeBorn),

        };

        Find.LetterStack.ReceiveLetter(letter);
        pawn.genes.AddGene(chosenGene, true);
    }
}