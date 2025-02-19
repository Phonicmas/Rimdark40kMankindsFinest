using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class WorkerClass_ImplantGeneseed : Recipe_Surgery
    {
        private GeneseedVial geneseedVialForText = null;
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!base.AvailableOnNow(thing, part))
            {
                return false;
            }
            if (!(thing is Pawn pawn) || !pawn.Spawned)
            {
                return false;
            }
            if (pawn.IsSuperHuman())
            {
                return false;
            }
            if (pawn.UndergoingPhaseDevelopment())
            {
                return false;
            }
            if (pawn.story != null && pawn.story.traits.HasTrait(Genes40kDefOf.BEWH_Serf))
            {
                return false;
            }

            var defMod = recipe.GetModExtension<DefModExtension_GeneseedVialRecipe>();

            var list = pawn.Map.listerThings.ThingsOfDef(defMod.geneseedVial);

            var result = false;

            foreach (var item in list)
            {
                if (!(item is GeneseedVial geneseedVial))
                {
                    continue;
                }

                if (defMod.geneFromMaterial == null && geneseedVial.extraGeneFromMaterial == null)
                {
                    result = true;
                    geneseedVialForText = geneseedVial;
                    break;
                }

                if (defMod.geneFromMaterial != null && defMod.geneFromMaterial == geneseedVial.extraGeneFromMaterial)
                {
                    result = true;
                    geneseedVialForText = geneseedVial;
                    break;
                }
            }
            
            return result;
        }

        public override TaggedString GetConfirmation(Pawn pawn)
        {
            return Genes40kUtils.GetGeneseedImplantationSuccessChanceDesc(pawn, geneseedVialForText);
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
            {
                return;
            }
            
            var geneseedVial = (GeneseedVial)ingredients.First(x => x is GeneseedVial);
            
            ImplantGeneseed(pawn, geneseedVial);

            if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
            }
        }

        private static void ImplantGeneseed(Pawn pawn, GeneseedVial geneseedVial)
        {
            var defMod = geneseedVial.def.GetModExtension<DefModExtension_GeneseedVial>();

            var failChance = Genes40kUtils.GetGeneseedImplantationSuccessChance(pawn, geneseedVial);
            
            var rand = new Random();
            if (rand.Next(0, 100) < failChance)
            {
                pawn.Kill(null);
                return;
            }
            
            pawn.genes.SetXenotypeDirect(defMod.xenotype);

            if (defMod.overrideXenotypeGenesGiven)
            {
                foreach (var gene in defMod.overridenAddedGenes.Where(gene => !pawn.genes.HasActiveGene(gene)))
                {
                    pawn.genes.AddGene(gene, true);
                }
            }
            else
            {
                foreach (var gene in defMod.xenotype.genes.Where(gene => !pawn.genes.HasActiveGene(gene)))
                {
                    pawn.genes.AddGene(gene, true);
                }
            }

            if (geneseedVial.extraGeneFromMaterial != null)
            {
                pawn.genes.AddGene(geneseedVial.extraGeneFromMaterial, true);
            }

            if (defMod.appliesHediff != null)
            {
                pawn.health.AddHediff(defMod.appliesHediff);
            }
            
        }
    }
}