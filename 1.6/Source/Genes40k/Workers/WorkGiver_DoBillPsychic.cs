using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_DoBillPsychic : WorkGiver_DoBill
{
    private GameComponent_MankindFinestUtils GameComp => Current.Game?.GetComponent<GameComponent_MankindFinestUtils>();
    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (thing is not Building_GeneTable building_GeneTable)
        {
            return null;
        }
        
        var orgBills = new List<Bill>();
        orgBills.AddRange(building_GeneTable.billStack.Bills);
        
        var billAddPost = new List<Bill>();

        foreach (var bill in building_GeneTable.billStack.Bills)
        {
            if (bill.recipe.HasModExtension<DefModExtension_GeneMatrixRecipe>() && bill.recipe.GetModExtension<DefModExtension_GeneMatrixRecipe>().drainsUserWhenMaking)
            {
                if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0)
                {
                    billAddPost.Add(bill);
                    JobFailReason.Is("BEWH.MankindsFinest.GeneManupulationTable.PsychicSensitivityRequired".Translate(bill.recipe.products.First().Label), bill.Label);
                    continue;
                }
            }

            if (bill.recipe.HasModExtension<DefModExtension_LegionMaterialCreation>())
            {
                var defMod = bill.recipe.GetModExtension<DefModExtension_LegionMaterialCreation>();
                var comp = building_GeneTable.GetComp<CompAffectedByFacilities>();

                if (!GameComp.HasMaterial(defMod.requiredLegionMaterial))
                {
                    billAddPost.Add(bill);
                    JobFailReason.Is("BEWH.MankindsFinest.GeneManupulationTable.MissingLegionMaterial".Translate(bill.recipe.products.First().Label, defMod.requiredLegionMaterial.label), bill.Label);
                }
            }
        }

        foreach (var bill in billAddPost)
        {
            building_GeneTable.billStack.Bills.Remove(bill);
        }
        
        var job = base.JobOnThing(pawn, thing, forced);

        building_GeneTable.billStack.Bills.RemoveRange(0, building_GeneTable.billStack.Bills.Count);
        building_GeneTable.billStack.Bills.AddRange(orgBills);

        return job;
    }
}