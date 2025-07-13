using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k;

public class WorkerClass_LegionMaterialCreation : Recipe_Surgery
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (!base.AvailableOnNow(thing, part) || !(thing is Pawn pawn))
        {
            return false;
        }
            
        if (pawn.genes == null || !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ImmortisGland))
        {
            return false;
        }
        
        if (pawn.genes.GetGene(Genes40kDefOf.BEWH_ImmortisGland) is not Gene_ImmortisGland immortisGland)
        {
            return false;
        }

        if (pawn.genes.GenesListForReading.Where(gene => gene.def.HasModExtension<DefModExtension_PrimarchMaterial>()).EnumerableNullOrEmpty())
        {
            return false;
        }

        return !immortisGland.MaterialExtracted;
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
        var immortisGland = (Gene_ImmortisGland)pawn.genes.GetGene(Genes40kDefOf.BEWH_ImmortisGland);
        immortisGland.ExtractedMaterial();

        var gene = pawn.genes.GenesListForReading.First(gene => gene.def.HasModExtension<DefModExtension_PrimarchMaterial>());
        var legionMaterial = gene.def.GetModExtension<DefModExtension_PrimarchMaterial>().relatedMaterial;
        
        GenSpawn.Spawn(legionMaterial, billDoer.Position, billDoer.Map);
    }
}