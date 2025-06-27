using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public static class NaturalBirthPerpetual
{
    static NaturalBirthPerpetual()
    {
        Genes40kMod.harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ApplyBirthOutcome"), null, new HarmonyMethod(AccessTools.Method(typeof(NaturalBirthPsykerPariah), "Postfix")));
    }

    public static void Postfix(ref Thing __result, Pawn geneticMother)
    {
        var modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
        if (!modSettings.perpetualBirth)
        {
            return;
        }
        var pawn = (Pawn)__result;
        if (pawn.Faction != Faction.OfPlayer)
        {
            return;
        }
            
        if (pawn.genes == null || Enumerable.Any(pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_PerpetualGene>()))
        {
            return;
        }

        var unnaturalChance = modSettings.perpetualBirthChance;
        var rand = new Random();
        if (rand.Next(0, 100) > unnaturalChance)
        {
            return;
        }
            
        var weightedSelection = new WeightedSelection<GeneDef>();
        //Perpetual genes
        weightedSelection.AddEntry(Genes40kDefOf.BEWH_PerpetualGamma, 50);
        weightedSelection.AddEntry(Genes40kDefOf.BEWH_PerpetualBeta, 10);
        weightedSelection.AddEntry(Genes40kDefOf.BEWH_PerpetualAlpha, 1);
            
        var chosenGene = weightedSelection.GetRandomUnique();
        var typeBorn = "BEWH.MankindsFinest.CommonKeywords.Perpetual".Translate();
            
        var letter = new Letter_JumpTo
        {
            lookTargets = pawn,
            def = Genes40kDefOf.BEWH_NaturalBornX,
            Text = "BEWH.MankindsFinest.Event.NaturalBornXMessage".Translate(geneticMother.Named("PAWN"), pawn.Named("PAWN"), typeBorn),
            Label = "BEWH.MankindsFinest.Event.NaturalBornXLetter".Translate(typeBorn),

        };

        Find.LetterStack.ReceiveLetter(letter);
        pawn.genes.AddGene(chosenGene, true);
    }
}