using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public static class Genes40kUtils
    {
        private static Genes40kModSettings modSettings = null;

        public static Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());

        private static List<ShoulderIconDef> rightShoulderIconDef = null;
        
        public static List<ShoulderIconDef> RightShoulderIconDef => rightShoulderIconDef ?? (rightShoulderIconDef = DefDatabase<ShoulderIconDef>.AllDefsListForReading.Where(rightShoulderDef => rightShoulderDef.rightShoulder).ToList());
        
        private static List<ShoulderIconDef> leftShoulderIconDef = null;
        
        public static List<ShoulderIconDef> LeftShoulderIconDef => leftShoulderIconDef ?? (leftShoulderIconDef = DefDatabase<ShoulderIconDef>.AllDefsListForReading.Where(leftShoulderDef => leftShoulderDef.leftShoulder).ToList());

        public static readonly Texture2D MindShieldOffIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOff");
        public static readonly Texture2D MindShieldOnIcon = ContentFinder<Texture2D>.Get("UI/Abilities/BEWH_MindShieldOn");
        
        public static List<GeneDef> ThunderWarriorGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_ProtoOssmodula,
                Genes40kDefOf.BEWH_Musculeator,
                Genes40kDefOf.BEWH_Mentanifex,
                Genes40kDefOf.BEWH_Vigoranis,
                Genes40kDefOf.BEWH_Hyperanatomica,
                Genes40kDefOf.BEWH_Furybound,
            };

        public static List<GeneDef> SpaceMarineGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SecondaryHeart,
                Genes40kDefOf.BEWH_Ossmodula,
                Genes40kDefOf.BEWH_Biscopea,
                Genes40kDefOf.BEWH_Haemastamen,
                Genes40kDefOf.BEWH_LarramansOrgan,
                Genes40kDefOf.BEWH_CatalepseanNode,
                Genes40kDefOf.BEWH_Preomnor,
                Genes40kDefOf.BEWH_Omophagea,
                Genes40kDefOf.BEWH_MultiLung,
                Genes40kDefOf.BEWH_Occulobe,
                Genes40kDefOf.BEWH_LymansEar,
                Genes40kDefOf.BEWH_SusAnMembrane,
                Genes40kDefOf.BEWH_Melanochrome,
                Genes40kDefOf.BEWH_OoliticKidney,
                Genes40kDefOf.BEWH_Neuroglottis,
                Genes40kDefOf.BEWH_Mucranoid,
                Genes40kDefOf.BEWH_BetchersGland,
                Genes40kDefOf.BEWH_ProgenoidGlands,
                Genes40kDefOf.BEWH_BlackCarapace
            };

        public static List<GeneDef> PrimarisGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SinewCoil,
                Genes40kDefOf.BEWH_Magnificat,
                Genes40kDefOf.BEWH_BelisarianFurnace
            };

        public static List<GeneDef> CustodesGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_ImmunisLeucocyte,
                Genes40kDefOf.BEWH_AthanaticVitae,
                Genes40kDefOf.BEWH_FulguriteNervePlexus,
                Genes40kDefOf.BEWH_AtlasMorphogen,
                Genes40kDefOf.BEWH_MnemosyneMindshield,
                Genes40kDefOf.BEWH_FulgurVitaliumstrand
            };

        public static List<GeneDef> PrimarchGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_ImmortisGland,
                Genes40kDefOf.BEWH_TempestusOcularium,
                Genes40kDefOf.BEWH_ThalaxCortex,
                Genes40kDefOf.BEWH_HelixomeArray,
                Genes40kDefOf.BEWH_VermillionCache,
                Genes40kDefOf.BEWH_CelerityNexus,
                Genes40kDefOf.BEWH_HyperionMuscleStrands
            };
        
        public static List<GeneDef> PsykerGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_IotaPsyker,
                Genes40kDefOf.BEWH_Psyker,
                Genes40kDefOf.BEWH_DeltaPsyker,
                Genes40kDefOf.BEWH_BetaPsyker,
                Genes40kDefOf.BEWH_AlphaPsyker
            };
        
        public static List<GeneDef> PariahGenes => new List<GeneDef>
            {
                Genes40kDefOf.BEWH_OmegaPariah,
                Genes40kDefOf.BEWH_SigmaPariah,
                Genes40kDefOf.BEWH_UpsilonPariah,
            };

        public static List<HediffDef> DevelopmentPhases => new List<HediffDef>
            {
                Genes40kDefOf.BEWH_FirstbornPhaseOne,
                Genes40kDefOf.BEWH_FirstbornPhaseTwo,
                Genes40kDefOf.BEWH_FirstbornPhaseThree,
                
                Genes40kDefOf.BEWH_PrimarisPhaseOne,
                Genes40kDefOf.BEWH_PrimarisPhaseTwo,
                Genes40kDefOf.BEWH_PrimarisPhaseThree,
            };
        
        public static bool HasGene(this Pawn_GeneTracker geneTracker, GeneDef geneDef)
        {
            if (geneDef == null)
            {
                return false;
            }
            var genesListForReading = geneTracker.GenesListForReading;
            
            foreach (var gene in genesListForReading)
            {
                if (gene.def == geneDef)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool HasGenes(this Pawn_GeneTracker geneTracker, List<GeneDef> geneDefs)
        {
            if (geneDefs.NullOrEmpty())
            {
                return false;
            }
            var genesListForReading = geneTracker.GenesListForReading.Select(gene => gene.def).ToList();

            return geneDefs.All(geneDef => genesListForReading.Contains(geneDef));
        }
        
        public static bool IsThunderWarrior(this Pawn pawn)
        {
            return pawn.genes.HasGenes(ThunderWarriorGenes);
        }

        public static bool IsFirstborn(this Pawn pawn)
        {
            return pawn.genes.HasGenes(SpaceMarineGenes);
        }

        public static bool IsPrimaris(this Pawn pawn)
        {
            return pawn.genes.HasGenes(PrimarisGenes) && pawn.IsFirstborn();
        }

        public static bool IsCustodes(this Pawn pawn)
        {
            return pawn.genes.HasGenes(CustodesGenes);
        }

        public static bool IsPrimarch(this Pawn pawn)
        {
            return pawn.genes.HasGenes(PrimarchGenes);
        }
        
        public static bool IsSuperHuman(this Pawn pawn)
        {
            //Primaris is not checked, as if they are primaris, then they are by extension also firstborn
            return pawn.IsThunderWarrior() || pawn.IsFirstborn() || pawn.IsCustodes() || pawn.IsPrimarch();
        }
        
        public static bool IsPsyker(this Pawn pawn)
        {
            return Enumerable.Any(PsykerGenes, gene => pawn.genes.HasActiveGene(gene));
        }

        public static bool IsPariah(this Pawn pawn)
        {
            return Enumerable.Any(PariahGenes, gene => pawn.genes.HasActiveGene(gene));
        }

        public static bool UndergoingPhaseDevelopment(this Pawn pawn)
        {
            return Enumerable.Any(DevelopmentPhases, hediff => pawn.health.hediffSet.HasHediff(hediff));
        }
        
        public static void OffsetDivineRadiance(Pawn pawn, float offset)
        {
            var geneDivineRadiance = pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>();
            geneDivineRadiance?.ChangeDivineRadianceAmount(offset);
        }
        
        public static ChapterColourDef SetupChapterForPawn(Pawn pawn, bool randomChapter)
        {
            if (pawn.genes == null || !pawn.IsFirstborn())
            {
                return null;
            }

            if (Enumerable.Any(pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_ChapterGene>()))
            {
                return null;
            }

            var xenotypeName = string.Empty;
            
            var chapter = randomChapter ? Current.Game.GetComponent<GameComponent_MankindFinestUtils>().CurrentChapterColour : ModSettings.CurrentlySelectedPreset;
            
            var chapterColourPrimary = chapter.primaryColour;
            var chapterColourSecondary = chapter.secondaryColour;
            var shoulderIconDef = chapter.relatedChapterIcon;
            GeneDef chapterGene = null;

            if (chapter.relatedChapterGene != null)
            {
                chapterGene = chapter.relatedChapterGene;
            }
            else if (shoulderIconDef != null)
            {
                chapterGene = shoulderIconDef.relatedChapterGene;
            }
            
            if (chapterGene != null && chapterGene.HasModExtension<DefModExtension_ChapterGene>())
            {
                xenotypeName = chapterGene.GetModExtension<DefModExtension_ChapterGene>().chapterName;
            }

            if (chapterGene != null && !pawn.genes.HasActiveGene(chapterGene))
            {
                pawn.genes.AddGene(chapterGene, true);
                if (xenotypeName != string.Empty)
                {
                    pawn.genes.xenotypeName = xenotypeName;
                    pawn.genes.iconDef = Genes40kDefOf.BEWH_AstartesIcon;
                }
            }
            
            foreach (var apparel in pawn.apparel.WornApparel)
            {
                switch (apparel)
                {
                    case ChapterBodyDecorativeApparelColourTwo extraIconsChapterApparelColourTwo:
                        extraIconsChapterApparelColourTwo.ApplyColourPreset(chapterColourPrimary, chapterColourSecondary);
                        extraIconsChapterApparelColourTwo.LeftShoulderIcon = shoulderIconDef;
                        break;
                    case ChapterHeadDecorativeApparelColourTwo chapterApparelColourTwo:
                        chapterApparelColourTwo.ApplyColourPreset(chapterColourPrimary, chapterColourSecondary);
                        break;
                }
            }

            return chapter;
        }

        public static void MakeGeneseedVial(Pawn pawn, bool isPrimaris)
        {
            GeneseedVial geneseedVial;
            
            if (isPrimaris)
            {
                geneseedVial = (GeneseedVial)ThingMaker.MakeThing(Genes40kDefOf.BEWH_GeneseedVialPrimaris);
            }
            else
            {
                geneseedVial = (GeneseedVial)ThingMaker.MakeThing(Genes40kDefOf.BEWH_GeneseedVialFirstborn);
            }

            if (pawn.genes?.GenesListForReading != null)
            {
                var gene = pawn.genes.GenesListForReading.FirstOrFallback(gene => gene.Active && gene.def.HasModExtension<DefModExtension_ChapterGene>(), null);
                if (gene != null)
                {
                    geneseedVial.extraGeneFromMaterial = gene.def;
                }
            }

            if (GenPlace.TryPlaceThing(geneseedVial, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
            {
                return;
            }
            Log.Error("Could not drop item near " + pawn.PositionHeld);
        }

        public static void InspectPrimarchEmbryoGenes(PrimarchEmbryo embryo)
        {
            if (embryo == null)
            {
                return;
            }
            
            var pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist);
            pawn.ageTracker.AgeBiologicalTicks = 3600000 * 25;
            
            foreach (var gene in pawn.genes.GenesListForReading)
            {
                pawn.genes.RemoveGene(gene);
            }
            
            foreach (var gene in embryo.birthGenes.GenesListForReading)
            {
                pawn.genes.AddGene(gene, false);
            }
            
            foreach (var gene in embryo.primarchGenes.GenesListForReading)
            {
                pawn.genes.AddGene(gene, true);
            }

            pawn.genes.SetXenotypeDirect(Genes40kDefOf.BEWH_Primarch);
            
            Find.WindowStack.Add(new Dialog_ViewGenes(pawn));
            
            pawn.Destroy();
            Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);
        }
        
        public static void InspectGeneseedVialGenes(GeneseedVial geneseedVial)
        {
            if (geneseedVial == null)
            {
                return;
            }
            
            var pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist);
            pawn.ageTracker.AgeBiologicalTicks = 3600000 * 25;
            
            foreach (var gene in pawn.genes.GenesListForReading)
            {
                pawn.genes.RemoveGene(gene);
            }
            
            foreach (var gene in geneseedVial.GeneSet.GenesListForReading)
            {
                pawn.genes.AddGene(gene, true);
            }

            if (geneseedVial.extraGeneFromMaterial != null)
            {
                pawn.genes.AddGene(geneseedVial.extraGeneFromMaterial, true);
            }

            var xenotypeDef = XenotypeDefOf.Baseliner;

            if (geneseedVial.xenotype != null)
            {
                xenotypeDef = geneseedVial.xenotype;
            }

            pawn.genes.SetXenotypeDirect(xenotypeDef);
            
            Find.WindowStack.Add(new Dialog_ViewGenes(pawn));
            
            pawn.Destroy();
            Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);
        }

        public static int GetGeneseedImplantationSuccessChance(Pawn pawn, GeneseedVial geneseedVial)
        {
            var defMod = geneseedVial.def.GetModExtension<DefModExtension_GeneseedVial>();

            var failChanceAgeOffset = 0;
            if (pawn.ageTracker.AgeBiologicalYears < defMod.minAgeImplant)
            {
                failChanceAgeOffset = defMod.minAgeImplant - pawn.ageTracker.AgeBiologicalYears;
            }
            else if (pawn.ageTracker.AgeBiologicalYears > defMod.maxAgeImplant)
            {
                failChanceAgeOffset = pawn.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
            }
            failChanceAgeOffset *= defMod.failureChancePerAgePast;
            
            var failChanceGeneOffset = 0;
            var failChanceCapGeneOffset = 0;

            if (geneseedVial.extraGeneFromMaterial != null && geneseedVial.extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = geneseedVial.extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
            }
            
            var failChance = defMod.baseFailureChance;
            failChance += failChanceAgeOffset + failChanceGeneOffset;

            var failCapChance = defMod.failChanceCap;
            failCapChance += failChanceCapGeneOffset;
            
            if (failCapChance > 100)
            {
                failCapChance = 100;
            }

            if (failChance > failCapChance)
            {
                failChance = failCapChance;
            }

            return failChance;
        }

        public static string GetGeneseedImplantationSuccessChanceDesc(Pawn pawn, GeneseedVial geneseedVial)
        {
            if (geneseedVial == null)
            {
                return string.Empty;
            }
            
            var text = "BEWH.MankindsFinest.GeneseedVial.ImplantGeneseedDesc".Translate(pawn, geneseedVial.xenotypeName);
            var defMod = geneseedVial.def.GetModExtension<DefModExtension_GeneseedVial>();
            var failChanceCausedBy = new List<string>();
            
            failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(defMod.baseFailureChance, "BEWH.MankindsFinest.GeneseedVial.BaseFailureChance".Translate()));
            
            var failChanceAgeOffset = 0;
            if (pawn.ageTracker.AgeBiologicalYears < defMod.minAgeImplant)
            {
                failChanceAgeOffset = defMod.minAgeImplant - pawn.ageTracker.AgeBiologicalYears;
            }
            else if (pawn.ageTracker.AgeBiologicalYears > defMod.maxAgeImplant)
            {
                failChanceAgeOffset = pawn.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
            }

            if (failChanceAgeOffset != 0)
            {
                failChanceAgeOffset *= defMod.failureChancePerAgePast;
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(failChanceAgeOffset, "BEWH.MankindsFinest.GeneseedVial.OutsideOptimalAgeRange".Translate(pawn, defMod.minAgeImplant, defMod.maxAgeImplant)));
            }
            
            var failChanceGeneOffset = 0;
            var failChanceCapGeneOffset = 0;

            if (geneseedVial.extraGeneFromMaterial != null && geneseedVial.extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = geneseedVial.extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(geneDefMod.additionalChanceOffset, geneseedVial.extraGeneFromMaterial.label));
            }

            var failChance = defMod.baseFailureChance;
            failChance += failChanceGeneOffset + failChanceAgeOffset;
            
            var failCapChance = defMod.failChanceCap;
            failCapChance += failChanceCapGeneOffset;
            
            if (ModSettings.implantationSuccessOffset != 0)
            {
                failChance += ModSettings.implantationSuccessOffset;
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(ModSettings.implantationSuccessOffset, "BEWH.Framework.CommonKeywords.ModSettings".Translate()));
            }
            
            if (ModSettings.implantationCapOffset != 0)
            {
                failCapChance += ModSettings.implantationCapOffset;
            }
            
            if (failCapChance > 100)
            {
                failCapChance = 100;
            }

            var wasCapped = false;

            if (failChance > failCapChance)
            {
                failChance = failCapChance;
                wasCapped = true;
            }

            if (failChance > 0)
            {
                text += "\n\n" + "BEWH.MankindsFinest.GeneseedVial.CurrentFailureChance".Translate(failChance);

                text += "\n\n" + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCausedBy".Translate();

                foreach (var failChanceCause in failChanceCausedBy)
                {
                    text += "\n" + failChanceCause;
                }

                if (wasCapped)
                {
                    text += "\n\n" + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCapped".Translate(failCapChance);
                }
            }

            text += "\n\n" + "WouldYouLikeToContinue".Translate();

            return text;
        }
        
        
    }
}