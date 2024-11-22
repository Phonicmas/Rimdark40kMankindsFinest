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
            if (Genes40kUtils.IsSuperHuman(pawn))
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
                    break;
                }

                if (!geneseedVial.GeneSet.GenesListForReading.Contains(defMod.geneFromMaterial))
                {
                    continue;
                }
                
                result = true;
                break;
            }
            
            return result;
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

            if (ModsConfig.IdeologyActive)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InstalledProsthetic, billDoer.Named(HistoryEventArgsNames.Doer)));
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