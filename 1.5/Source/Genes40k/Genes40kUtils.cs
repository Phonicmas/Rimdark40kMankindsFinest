using System;
using RimWorld;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Verse;


namespace Genes40k
{
    public class Genes40kUtils
    {
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

        public static bool IsThunderWarrior(Pawn pawn)
        {
            return ThunderWarriorGenes.All(geneDef => pawn.genes.HasActiveGene(geneDef));
        }

        public static bool IsFirstborn(Pawn pawn)
        {
            return SpaceMarineGenes.All(geneDef => pawn.genes.HasActiveGene(geneDef));
        }

        public static bool IsPrimaris(Pawn pawn)
        {
            return PrimarisGenes.All(geneDef => pawn.genes.HasActiveGene(geneDef)) && IsFirstborn(pawn);
        }

        public static bool IsCustodes(Pawn pawn)
        {
            return CustodesGenes.All(geneDef => pawn.genes.HasActiveGene(geneDef));
        }

        public static bool IsPrimarch(Pawn pawn)
        {
            return PrimarchGenes.All(geneDef => pawn.genes.HasActiveGene(geneDef));
        }
        
        public static bool IsSuperHuman(Pawn pawn)
        {
            //Primaris not check, as if they are primaris, then they are by extension also firstborn
            return IsThunderWarrior(pawn) || IsFirstborn(pawn) || IsCustodes(pawn) || IsPrimarch(pawn);
        }
        
        public static bool IsPsyker(Pawn pawn)
        {
            return Enumerable.Any(PsykerGenes, gene => pawn.genes.HasActiveGene(gene));
        }

        public static bool IsPariah(Pawn pawn)
        {
            return Enumerable.Any(PariahGenes, gene => pawn.genes.HasActiveGene(gene));
        }

        public static bool UndergoingPhaseDevelopment(Pawn pawn)
        {
            return Enumerable.Any(DevelopmentPhases, hediff => pawn.health.hediffSet.HasHediff(hediff));
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

            GeneDef extraGeneFromMaterial = null;

            if (pawn.genes != null)
            {
                extraGeneFromMaterial = pawn.genes.GenesListForReading.First(gene => gene.Active && gene.def.HasModExtension<DefModExtension_ChapterGene>()).def;
            }

            if (extraGeneFromMaterial != null)
            {
                geneseedVial.extraGeneFromMaterial = extraGeneFromMaterial;
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
                failChanceAgeOffset = pawn.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
            }
            failChanceAgeOffset *= defMod.failureChancePerAgePast;

            var failChance = defMod.baseFailureChance;
            var failChanceGeneOffset = 0;

            var failCapChance = defMod.failChanceCap;
            var failChanceCapGeneOffset = 0;

            if (geneseedVial.extraGeneFromMaterial != null && geneseedVial.extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = geneseedVial.extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
            }
            
            failChance += failChanceAgeOffset + failChanceGeneOffset;

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
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(failChanceAgeOffset, "BEWH.MankindsFinest.GeneseedVial.OutsideOptimalAgeRange".Translate(pawn, defMod.minAgeImplant, defMod.maxAgeImplant)));
            }
            else if (pawn.ageTracker.AgeBiologicalYears > defMod.maxAgeImplant)
            {
                failChanceAgeOffset = pawn.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(failChanceAgeOffset, "BEWH.MankindsFinest.GeneseedVial.OutsideOptimalAgeRange".Translate(pawn, defMod.minAgeImplant, defMod.maxAgeImplant)));
            }
            failChanceAgeOffset *= defMod.failureChancePerAgePast;

            var failChance = defMod.baseFailureChance;
            var failChanceGeneOffset = 0;

            var failCapChance = defMod.failChanceCap;
            var failChanceCapGeneOffset = 0;

            if (geneseedVial.extraGeneFromMaterial != null && geneseedVial.extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = geneseedVial.extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
                failChanceCausedBy.Add("\t- " + "BEWH.MankindsFinest.GeneseedVial.FailureChanceCause".Translate(geneDefMod.additionalChanceOffset, geneseedVial.extraGeneFromMaterial.label));
            }

            failCapChance += failChanceCapGeneOffset;
            failChance += failChanceGeneOffset + failChanceAgeOffset;

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
        
        public static void OffsetDivineRadiance(Pawn pawn, float offset)
        {
            var geneDivineRadiance = pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>();
            geneDivineRadiance?.ChangeDivineRadianceAmount(offset);
        }
        
        public static void SetupChapterForPawn(Pawn pawn)
        {
            if (pawn.genes == null || !IsFirstborn(pawn))
            {
                return;
            }
            
            var chapter = Current.Game.GetComponent<GameComponent_MankindFinestUtils>().CurrentChapterColour;
            
            //Temp
            chapter = Genes40kDefOf.BEWH_ChapterColourVII;

            if (!pawn.genes.HasActiveGene(chapter.relatedChapterGene))
            {
                pawn.genes.AddGene(chapter.relatedChapterGene, true);
            }
            
            foreach (var apparel in pawn.apparel.WornApparel)
            {
                switch (apparel)
                {
                    case ExtraIconsChapterApparelColourTwo extraIconsChapterApparelColourTwo:
                        extraIconsChapterApparelColourTwo.ApplyColourPreset(chapter);
                        extraIconsChapterApparelColourTwo.LeftShoulderIcon = chapter.relatedChapterIcon;
                        break;
                    case ChapterApparelColourTwo chapterApparelColourTwo:
                        chapterApparelColourTwo.ApplyColourPreset(chapter);
                        break;
                }
            }
        }
        
    }
}