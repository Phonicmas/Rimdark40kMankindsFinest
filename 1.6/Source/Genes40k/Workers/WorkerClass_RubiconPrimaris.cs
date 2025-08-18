using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class WorkerClass_RubiconPrimaris : Recipe_Surgery
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (!base.AvailableOnNow(thing, part) || thing is not Pawn pawn)
        {
            return false;
        }
        
        if (pawn.genes == null)
        {
            return false;
        }

        return pawn.IsFirstborn() && !pawn.IsPrimaris();
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
        foreach (var gene in Genes40kUtils.PrimarisGenes.Where(gene => !pawn.genes.HasActiveGene(gene)))
        {
            pawn.genes.AddGene(gene, true);
        }
        pawn.genes.SetXenotypeDirect(Genes40kDefOf.BEWH_PrimarisSpaceMarine);
    }
}