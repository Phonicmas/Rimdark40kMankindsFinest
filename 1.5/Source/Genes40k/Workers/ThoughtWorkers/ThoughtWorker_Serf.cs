using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class ThoughtWorker_Serf : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        var hediff = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            
        if (p.Map != null)
        {
            var allCol = p.Map.mapPawns.FreeColonistsSpawned.ToList();
                
            foreach (var colonist in allCol)
            {
                if (p == colonist || !colonist.IsSuperHuman())
                {
                    continue;
                }
                    
                if (!(colonist.Position.DistanceTo(p.Position) <= 20))
                {
                    if (hediff != null)
                    {
                        hediff.Severity = 1f;
                    }
                    return ThoughtState.ActiveAtStage(1);
                }
                    
                if (hediff != null)
                {
                    hediff.Severity = 2f;
                }
                return ThoughtState.ActiveAtStage(2);
            }
        }
            
        if(p.GetCaravan() != null)
        {
            if (Enumerable.Any(p.GetCaravan().pawns.InnerListForReading, colonist => colonist.IsSuperHuman()))
            {
                if (hediff != null)
                {
                    hediff.Severity = 2f;
                }
                return ThoughtState.ActiveAtStage(2);
            }
        }
            
        if (hediff != null)
        {
            hediff.Severity = 0.1f;
        }
        return ThoughtState.ActiveAtStage(0);
    }
        
    protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        return true;
    }
}