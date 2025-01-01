using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k
{
    public class PawnGroupKindWorker_40kChapter : PawnGroupKindWorker_Normal
    {
	    private Genes40kModSettings modSettings = null;

	    private Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());
	    
        protected override void GeneratePawns(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, List<Pawn> outPawns, bool errorOnZeroResults = true)
		{
			if (!CanGenerateFrom(parms, groupMaker))
			{
				if (errorOnZeroResults)
				{
					Log.Error(string.Concat("Cannot generate pawns for ", parms.faction, " with ", parms.points, ". Defaulting to a single random cheap group."));
				}
				return;
			}
			var allowFood = parms.raidStrategy == null || parms.raidStrategy.pawnsCanBringFood || (parms.faction != null && !parms.faction.HostileTo(Faction.OfPlayer));
			var validatorPostGear = parms.raidStrategy != null ? (Predicate<Pawn>)(p => parms.raidStrategy.Worker.CanUsePawn(parms.points, p, outPawns)) : null;
			var flag = false;
			foreach (var item in PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.options, parms))
			{
				var request = new PawnGenerationRequest(item.Option.kind, parms.faction, fixedIdeo: parms.ideo, forcedXenotype: item.Xenotype, tile: parms.tile, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: true, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true, allowFood: allowFood, allowAddictions: true, inhabitant: parms.inhabitants, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f, biocodeApparelChance: 0f, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1f, validatorPreGear: null, validatorPostGear: validatorPostGear);
				if (parms.raidAgeRestriction != null && parms.raidAgeRestriction.Worker.ShouldApplyToKind(item.Option.kind))
				{
					request.BiologicalAgeRange = parms.raidAgeRestriction.ageRange;
					request.AllowedDevelopmentalStages = parms.raidAgeRestriction.developmentStage;
				}
				if (item.Option.kind.pawnGroupDevelopmentStage.HasValue)
				{
					request.AllowedDevelopmentalStages = item.Option.kind.pawnGroupDevelopmentStage.Value;
				}
				if (!Find.Storyteller.difficulty.ChildRaidersAllowed && parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer))
				{
					request.AllowedDevelopmentalStages = DevelopmentalStage.Adult;
				}
				var pawn = PawnGenerator.GeneratePawn(request);
				if (parms.forceOneDowned && !flag)
				{
					pawn.health.forceDowned = true;
					if (pawn.guest != null)
					{
						pawn.guest.Recruitable = true;
					}
					pawn.mindState.canFleeIndividual = false;
					flag = true;
				}

				if (pawn.genes != null && Genes40kUtils.IsFirstborn(pawn))
				{
					var chapter = GetRandomChapterForRaid();
					pawn.genes.AddGene(chapter.relatedChapterGene, true);
					foreach (var apparel in pawn.apparel.WornApparel)
					{
						if (apparel is ChapterApparelColourTwo chapterApparelColourTwo)
						{
							chapterApparelColourTwo.ApplyColourPreset(chapter);
						}
					}
				}
				
				outPawns.Add(pawn);
			}
		}
        
		public override bool CanGenerateFrom(PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			if (!PawnGroupMakerUtility.AnyOptions(parms, parms.faction?.def, groupMaker.options, parms.points))
			{
				Log.Message("HERE");
				return false;
			}
			return true;
		}
		
		public override float MinPointsToGenerateAnything(PawnGroupMaker groupMaker, FactionDef faction, PawnGroupMakerParms parms = null)
		{
			float num = ((parms != null && parms.points > 0f) ? parms.points : 100000f);
			float num2 = float.MaxValue;
			
			Log.Message("HERE2");
			List<PawnGenOptionWithXenotype> options = PawnGroupMakerUtility.GetOptions(parms, faction, groupMaker.options, num, num, float.MaxValue);
			foreach (PawnGenOptionWithXenotype item in options)
			{
				if (item.Option.kind.isFighter && item.Cost < num2 && PawnGroupMakerUtility.PawnGenOptionValid(item.Option, parms))
				{
					num2 = item.Cost;
				}
			}
			if (num2 == float.MaxValue)
			{
				foreach (PawnGenOptionWithXenotype item2 in options)
				{
					if (item2.Cost < num2 && PawnGroupMakerUtility.PawnGenOptionValid(item2.Option, parms))
					{
						num2 = item2.Cost;
					}
				}
				return num2;
			}
			Log.Message("HERE3");
			return num2;
		}

		private ChapterColourDef GetRandomChapterForRaid()
		{
			var chapterColours = DefDatabase<ChapterColourDef>.AllDefsListForReading;

			if (ModSettings.currentlySelectedPreset != null)
			{
				chapterColours.Remove(ModSettings.currentlySelectedPreset);
			}

			return chapterColours.RandomElement();
		}
    }
}