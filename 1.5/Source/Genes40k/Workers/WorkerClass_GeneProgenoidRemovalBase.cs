using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Genes40k
{
    public class WorkerClass_GeneProgenoidRemovalBase : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (!base.AvailableOnNow(thing, part) || !(thing is Pawn pawn))
            {
                return false;
            }
            if (!pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
            {
                return false;
            }
                
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int index = 0; index < hediffs.Count; ++index)
            {
                if ((!recipe.targetsBodyPart || hediffs[index].Part != null) && hediffs[index].def == recipe.removesHediff && hediffs[index].Visible)
                {
                    if (hediffs[index].Severity < 1f)
                    {
                        return false;
                    }
                        
                }
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
                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
                {
                    string text = (recipe.successfullyRemovedHediffMessage.NullOrEmpty() ? ((string)"MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, recipe.removesHediff.label.Named("HEDIFF"), billDoer.Named("SURGEON"), pawn.Named("PATIENT"))) : ((string)recipe.successfullyRemovedHediffMessage.Formatted(billDoer.LabelShort, pawn.LabelShort)));
                    Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
            Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == recipe.removesHediff && x.Part == part && x.Visible);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
                OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
            }
        }

        protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Genes40kUtils.MakeGeneseedVial(pawn, Genes40kUtils.IsPrimaris(pawn));
        }

    }
}