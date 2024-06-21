using RimWorld;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class Genes40kUtils
    {
        public static List<GeneDef> SpaceMarineGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
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
            return genedef;
        }

        public static List<GeneDef> PrimarisGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_SinewCoil,
                Genes40kDefOf.BEWH_Magnificat,
                Genes40kDefOf.BEWH_BelisarianFurnace
            };
            return genedef;
        }

        public static List<GeneDef> CustodesGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_CustodesStature,
                Genes40kDefOf.BEWH_CustodesResilience,
                Genes40kDefOf.BEWH_CustodesToughness,
                Genes40kDefOf.BEWH_CustodesExpertise,
                Genes40kDefOf.BEWH_CustodesStrength,
                Genes40kDefOf.BEWH_CustodesAnatomy
            };
            return genedef;
        }

        public static List<GeneDef> PrimarchGenes()
        {
            List<GeneDef> genedef = new List<GeneDef>
            {
                Genes40kDefOf.BEWH_PrimarchStature,
                Genes40kDefOf.BEWH_PrimarchResilience,
                Genes40kDefOf.BEWH_PrimarchToughness,
                Genes40kDefOf.BEWH_PrimarchExpertise,
                Genes40kDefOf.BEWH_PrimarchStrength,
                Genes40kDefOf.BEWH_PrimarchAnatomy
            };
            return genedef;
        }



        public static bool IsSpaceMarine(Pawn pawn)
        {
            foreach (GeneDef geneDef in SpaceMarineGenes())
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
            foreach (GeneDef geneDef in PrimarisGenes())
            {
                if (!pawn.genes.HasActiveGene(geneDef))
                {
                    return false;
                }
            }
            return IsSpaceMarine(pawn);
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