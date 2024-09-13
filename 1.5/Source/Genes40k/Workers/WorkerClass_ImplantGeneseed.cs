using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class WorkerClass_ImplantGeneseed : Recipe_Surgery
    {
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
            /*List<Thing> list = pawn.Map.listerThings.ThingsOfDef(recipe.GetModExtension<DefModExtension_GeneseedVialRecipe>().geneseedVial);
            if (list.Any())
            {
                foreach (Thing item in list)
                {
                    if (!item.IsForbidden(pawn) && !item.Position.Fogged(pawn.Map))
                    {
                        return true;
                    }
                }
            }*/
            return true;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (!CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
            {
                if (bill.xenogerm != null)
                {
                    GeneseedVial geneseedVial = (GeneseedVial)ingredients.Where(x => x is GeneseedVial).First();
                    ImplantGeneseed(pawn, geneseedVial);
                }

                if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
                {
                    ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
                }

                if (ModsConfig.IdeologyActive)
                {
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
                }
            }
        }

        private void ImplantGeneseed(Pawn pawn, GeneseedVial geneseedVial)
        {
            DefModExtension_GeneseedVial defMod = geneseedVial.def.GetModExtension<DefModExtension_GeneseedVial>();

            var minAgeCheck = pawn.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
            var maxAgeCheck = pawn.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
            if (minAgeCheck < maxAgeCheck)
            {
                minAgeCheck = maxAgeCheck;
            }
            var failChanceAgeOffset = minAgeCheck * defMod.failureChancePerAgePast;

            var failChance = 0;
            var failChanceGeneOffset = 0;

            var failCapChance = defMod.failChanceCap;
            var failChanceCapGeneOffset = 0;

            if (geneseedVial.extraGeneFromMaterial != null && geneseedVial.extraGeneFromMaterial.HasModExtension<DefModExtension_GeneseedPurity>())
            {
                var geneDefMod = geneseedVial.extraGeneFromMaterial.GetModExtension<DefModExtension_GeneseedPurity>();
                failChanceCapGeneOffset += geneDefMod.additionalChanceCapOffset;
                failChanceGeneOffset += geneDefMod.additionalChanceOffset;
            }

            failCapChance += failChanceCapGeneOffset;
            failChance += (failChanceAgeOffset + failChanceGeneOffset);

            if (failCapChance > 100)
            {
                failCapChance = 100;
            }

            if (failChance > failCapChance)
            {
                failChance = failCapChance;
            }

            Random rand = new Random();
            if (rand.Next(0, 100) < failChance)
            {
                pawn.Kill(null);
            }
            else
            {
                pawn.genes.SetXenotypeDirect(defMod.xenotype);
            }

            if (defMod.overrideXenotypeGenesGiven)
            {
                foreach (GeneDef gene in defMod.overridenAddedGenes)
                {
                    if (!pawn.genes.HasActiveGene(gene))
                    {
                        pawn.genes.AddGene(gene, true);
                    }
                }
            }
            else
            {
                foreach (GeneDef gene in defMod.xenotype.genes)
                {
                    if (!pawn.genes.HasActiveGene(gene))
                    {
                        pawn.genes.AddGene(gene, true);
                    }
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