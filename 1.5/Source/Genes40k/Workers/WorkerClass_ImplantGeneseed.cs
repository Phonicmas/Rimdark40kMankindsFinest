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

            int num1 = pawn.ageTracker.AgeBiologicalYears - defMod.minAgeImplant;
            int num2 = pawn.ageTracker.AgeBiologicalYears - defMod.maxAgeImplant;
            if (num1 < num2)
            {
                num1 = num2;
            }
            int failureChance = num1 * defMod.failureChancePerAgePast;
            if (failureChance > defMod.failChanceCap)
            {
                failureChance = defMod.failChanceCap;
            }
            Random rand = new Random();
            if (rand.Next(0, 100) < failureChance)
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