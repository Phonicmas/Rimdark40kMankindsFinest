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
        if (__result is not Pawn pawn)
        {
            return;
        }
        
        var modSettings = Genes40kUtils.ModSettings;
        if (!modSettings.psykerPariahBirth)
        {
            return;
        }
        
        if (pawn.Faction == null || pawn.Faction != Faction.OfPlayer)
        {
            return;
        }

        if (pawn.genes == null || pawn.genes.GenesListForReading.NullOrEmpty() || Enumerable.Any(pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_Pariah>() || gene.def.HasModExtension<DefModExtension_Psyker>()))
        {
            return;
        }

        var canBecomePsyker = true;
        var canBecomePariah = true;
        foreach (var gene in pawn.genes.GenesListForReading)
        {
            if (gene.def.exclusionTags.NullOrEmpty())
            {
                continue;
            }
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
        
        var unnaturalChance = modSettings.psykerPariahBirthChance;
        var rand = new Random();
        if (rand.Next(0, 100) > unnaturalChance)
        {
            return;
        }
            
        var weightedSelection = new WeightedSelection<GeneDef>();
        
        if (canBecomePsyker)
        {
            foreach (var psykerGene in Genes40kUtils.PsykerGenes)
            {
                weightedSelection.AddEntry(psykerGene, psykerGene.GetModExtension<DefModExtension_Psyker>().naturalBornSelectionWeight);
            }
        }

        if (canBecomePariah)
        {
            foreach (var pariahGene in Genes40kUtils.PariahGenes)
            {
                weightedSelection.AddEntry(pariahGene, pariahGene.GetModExtension<DefModExtension_Pariah>().naturalBornSelectionWeight);
            }
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