using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k;

public class JobDriver_WaitAndExit : JobDriver
{
    private int waitTicks = -1;
    
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (waitTicks < 0)
        {
            waitTicks = Rand.Range(1000, 2000);
        }
        for (var i = 0; i < 5; i++)
        {
            if (CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map, 6, null, out var result))
            {
                yield return Toils_Goto.GotoCell(result, PathEndMode.OnCell);
            }
            
            yield return Toils_General.Wait(waitTicks/5);
        }
        
        RCellFinder.TryFindRandomExitSpot(pawn, out var spot);
        yield return Toils_Goto.GotoCell(spot, PathEndMode.OnCell);
        yield return Toils_General.Do(() => pawn.ExitMap(false, pawn.Rotation));
    }
    
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref waitTicks, "waitTicks", -1);
    }
}