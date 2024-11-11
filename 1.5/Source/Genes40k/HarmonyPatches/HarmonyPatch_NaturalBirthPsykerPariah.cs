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

            if (Enumerable.Any(pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_Pariah>() || gene.def.HasModExtension<DefModExtension_Psyker>()))
            {
                return;
            }
            
            var weightedSelection = new WeightedSelection<GeneDef>();
            //Psyker genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_IotaPsyker, 40);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_Psyker, 20);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_DeltaPsyker, 6);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_BetaPsyker, 2);
            //Pariah Genes
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_SigmaPariah, 20);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_UpsilonPariah, 10);
            weightedSelection.AddEntry(Genes40kDefOf.BEWH_OmegaPariah, 2);
            
            var chosenGene = weightedSelection.GetRandomUnique();
            var typeBorn = chosenGene.HasModExtension<DefModExtension_Pariah>() ? "Pariah".Translate() : "Psyker".Translate();
            
            var letter = new Letter_JumpTo
            {
                lookTargets = pawn,
                def = Genes40kDefOf.BEWH_NaturalBornX,
                Text = "BEWH.NaturalBornXMessage".Translate(geneticMother.Named("PAWN"), pawn.Named("PAWN"), typeBorn),
                Label = "BEWH.NaturalBornXLetter".Translate(typeBorn),

            };

            Find.LetterStack.ReceiveLetter(letter);
            pawn.genes.AddGene(chosenGene, true);
        }
    }
}