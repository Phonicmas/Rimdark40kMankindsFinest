﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsThunderWarrior(Pawn pawn)
        {
            foreach (GeneDef geneDef in ThunderWarriorGenes)
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsSpaceMarine(Pawn pawn)
        {
            foreach (GeneDef geneDef in SpaceMarineGenes)
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsPrimaris(Pawn pawn)
        {
            foreach (GeneDef geneDef in PrimarisGenes)
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return IsSpaceMarine(pawn);
        }

        public static bool IsCustodes(Pawn pawn)
        {
            foreach (GeneDef geneDef in CustodesGenes)
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return IsSpaceMarine(pawn);
        }

        public static bool IsPrimarch(Pawn pawn)
        {
            foreach (GeneDef geneDef in PrimarchGenes)
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return IsSpaceMarine(pawn);
        }


        public static bool IsSuperHuman(Pawn pawn)
        {
            return (IsThunderWarrior(pawn) || IsSpaceMarine(pawn) || IsPrimaris(pawn) || IsCustodes(pawn) || IsPrimarch(pawn));
        }


        public static bool IsPsyker(Pawn pawn)
        {
            if (pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_IotaPsyker) || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_Psyker) || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_DeltaPsyker) || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_BetaPsyker))
            {
                return true;
            }
            return false;
        }

        public static bool IsPariah(Pawn pawn)
        {
            if (pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_OmegaPariah) || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_SigmaPariah) || pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_UpsilonPariah))
            {
                return true;
            }
            return false;
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

            List<GeneDef> extraGenes = new List<GeneDef>();

            if (pawn.genes != null)
            {
                foreach (Gene gene in pawn.genes.GenesListForReading)
                {
                    if (gene.def.HasModExtension<DefModExtension_ChapterGene>())
                    {
                        extraGenes.Add(gene.def);
                    }
                }
            }

            if (!extraGenes.NullOrEmpty())
            {
                geneseedVial.AddExtraGenes(extraGenes);
            }

            if (GenPlace.TryPlaceThing(geneseedVial, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
            {
                return;
            }
            Log.Error("Could not drop item near " + pawn.PositionHeld);
        }

    }
}