using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Genes40k;

public class WorkerClass_GeneProgenoidRemovalBase : Recipe_Surgery
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (!base.AvailableOnNow(thing, part) || !(thing is Pawn pawn))
        {
            return false;
        }
            
        if (pawn.genes == null || !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
        {
            return false;
        }

        if (!(pawn.genes.GetGene(Genes40kDefOf.BEWH_ProgenoidGlands) is Gene_ProgenoidGlands progenoidGlands))
        {
            return false;
        }

        return progenoidGlands.CanHarvestFirstProgenoidGland();
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
            
        OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
    }

    protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        var progenoidGlands = (Gene_ProgenoidGlands)pawn.genes.GetGene(Genes40kDefOf.BEWH_ProgenoidGlands);
        if (!progenoidGlands.HarvestFirstProgenoidGland())
        {
            return;
        }
            
        Genes40kUtils.MakeGeneseedVial(pawn, pawn.IsPrimaris());
    }
}