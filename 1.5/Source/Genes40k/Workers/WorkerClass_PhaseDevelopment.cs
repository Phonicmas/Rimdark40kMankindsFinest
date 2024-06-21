using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class WorkerClass_PhaseDevelopment : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!base.AvailableOnNow(thing, part))
            {
                return false;
            }
            if (!(thing is Pawn pawn))
            {
                return false;
            }
            if (!pawn.health.hediffSet.HasHediff(recipe.removesHediff))
            {
                return false;
            }
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(recipe.removesHediff);
            if (hediff.Severity != hediff.def.maxSeverity)
            {
                return false;
            }

            return true;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
            }
            DefModExtension_PhaseDevelopment defMod = recipe.GetModExtension<DefModExtension_PhaseDevelopment>();
            if (!defMod.addsGenes.NullOrEmpty() && pawn.genes != null)
            {
                foreach (GeneDef gene in defMod.addsGenes)
                {
                    pawn.genes.AddGene(gene, true);
                }
            }
            if (defMod.addHediff != null)
            {
                pawn.health.AddHediff(defMod.addHediff);
            }
        }
    }
}