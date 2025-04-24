using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class JobDriver_FleeInducedFear : JobDriver_Flee
{
    private const int CowerTicks = 300;

    public override string GetReport()
    {
        if (pawn.CurJob == job && pawn.Position == job.GetTarget(TargetIndex.A).Cell)
        {
            return "ReportCowering".Translate();
        }
        return base.GetReport();
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        foreach (var item in base.MakeNewToils())
        {
            yield return item;
        }
        var toil = ToilMaker.MakeToil("MakeNewToils");
        toil.defaultCompleteMode = ToilCompleteMode.Delay;
        toil.defaultDuration = CowerTicks;
        yield return toil;
    }
}