using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k;

public class WorkerClass_ImplantLegionMaterial : Recipe_Surgery
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

        if (!pawn.IsFirstborn())
        {
            return false;
        }
        
        if (pawn.genes.GenesListForReading.Any(gene => gene.def.HasModExtension<DefModExtension_ChapterGene>()))
        {
            return false;
        }
        var sangprimus = (Building_SangprimusPortum)pawn.Map.listerThings.ThingsOfDef(Genes40kDefOf.BEWH_SangprimusPortum).FirstOrFallback();
        var neededMaterial = recipe.GetModExtension<DefModExtension_LegionMaterialCreation>().requiredLegionMaterial;
        if (sangprimus == null || (!sangprimus.SearchableContentsChapter.Any(thing1 => thing1.def == neededMaterial) && !sangprimus.SearchableContentsPrimarch.Any(thing1 => thing1.def == neededMaterial)))
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
        OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
    }

    protected override void OnSurgerySuccess(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        var material = recipe.GetModExtension<DefModExtension_LegionMaterialCreation>().requiredLegionMaterial;
        var addedGene = material.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene;
        pawn.genes.AddGene(addedGene, true);
    }
}