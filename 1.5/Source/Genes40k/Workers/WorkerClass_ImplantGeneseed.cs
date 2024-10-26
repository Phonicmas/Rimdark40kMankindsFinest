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
            var list = pawn.Map.listerThings.ThingsOfDef(recipe.GetModExtension<DefModExtension_GeneseedVialRecipe>().geneseedVial);
            
            return !list.Any() || Enumerable.Any(list, item => !item.IsForbidden(pawn) && !item.Position.Fogged(pawn.Map));
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill)) return;
            
            var geneseedVial = (GeneseedVial)ingredients.First(x => x is GeneseedVial);
            ImplantGeneseed(pawn, geneseedVial);

            if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
            }

            if (ModsConfig.IdeologyActive)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
            }
        }

        private static void ImplantGeneseed(Pawn pawn, GeneseedVial geneseedVial)
        {
            var defMod = geneseedVial.def.GetModExtension<DefModExtension_GeneseedVial>();

            var failChanceAgeOffset = 0;
            if (!(defMod.minAgeImplant <= pawn.ageTracker.AgeBiologicalYears && defMod.maxAgeImplant >= pawn.ageTracker.AgeBiologicalYears))
            {
                var minAgeCheck = pawn.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
                var maxAgeCheck = pawn.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
                if (minAgeCheck < maxAgeCheck)
                {
                    minAgeCheck = maxAgeCheck;
                }
                failChanceAgeOffset = minAgeCheck * defMod.failureChancePerAgePast;
            }

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

            var rand = new Random();
            if (rand.Next(0, 100) < failChance)
            {
                pawn.Kill(null);
                return;
            }
            else
            {
                pawn.genes.SetXenotypeDirect(defMod.xenotype);
            }

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