using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Genes40k
{
	[StaticConstructorOnStartup]
    public static class NaturalBirthPsykerPariah
    {
		static NaturalBirthPsykerPariah()
		{
			Genes40kMod.harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ApplyBirthOutcome"), null, new HarmonyMethod(AccessTools.Method(typeof(NaturalBirthPsykerPariah), "Postfix")));
		}

		public static void Postfix(ref Thing __result, Pawn geneticMother)
        {
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
            
            var weightedSelection = new WeightedSelection<GeneDef>();
            //Psyker genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_IotaPsyker, 60);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_EpsilonPsyker, 40);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_DeltaPsyker, 12);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_BetaPsyker, 4);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_AlphaPsyker, 1);
            //Pariah Genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_SigmaPariah, 40);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_UpsilonPariah, 12);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_OmegaPariah, 4);
            
            var chosenGene = weightedSelection.GetRandomUnique();
            var typeBorn = chosenGene.HasModExtension<DefModExtension_Pariah>() ? "BEWH.MankindsFinest.CommonKeywords.Pariah".Translate() : "BEWH.MankindsFinest.CommonKeywords.Psyker".Translate();
            
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
}