using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class WorkGiver_DoBillPsychic : WorkGiver_DoBill
{
    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (thing is not Building_WorkTable building_WorkTable)
        {
            return null;
        }
        
        var orgBills = new List<Bill>();
        orgBills.AddRange(building_WorkTable.billStack.Bills);
        
        var billAddPost = new List<Bill>();

        foreach (var bill in building_WorkTable.billStack.Bills)
        {
            if (!bill.recipe.HasModExtension<DefModExtension_GeneMatrixRecipe>() || !bill.recipe.GetModExtension<DefModExtension_GeneMatrixRecipe>().drainsUserWhenMaking)
            {
                continue;
            }

            if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) > 0)
            {
                continue;
            }
            
            billAddPost.Add(bill);
            JobFailReason.Is("PsychicSensitivityRequired", bill.Label);
        }

        foreach (var bill in billAddPost)
        {
            building_WorkTable.billStack.Bills.Remove(bill);
        }
        
        var job = base.JobOnThing(pawn, thing, forced);

        building_WorkTable.billStack.Bills.RemoveRange(0, building_WorkTable.billStack.Bills.Count);
        building_WorkTable.billStack.Bills.AddRange(orgBills);

        return job;
    }
}