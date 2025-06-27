using System.Linq;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class Hediff_Serf : HediffWithComps
{
    private float cachedSeverity = 0.1f;
    public override float Severity
    {
        get
        {
            if (!pawn.IsHashIntervalTick(250))
            {
                return cachedSeverity;
            }

            var trait = pawn.story.traits.allTraits.FirstOrFallback(t => t.def == Genes40kDefOf.BEWH_Serf);
            if (trait == null)
            {
                return 0;
            }
            if (trait.Suppressed)
            {
                cachedSeverity = 0.1f;
                return cachedSeverity;
            }
                
            if (pawn.Map != null)
            {
                var allCol = pawn.Map.mapPawns.FreeColonistsSpawned.ToList();
                
                foreach (var colonist in allCol)
                {
                    if (pawn == colonist || !colonist.IsSuperHuman())
                    {
                        continue;
                    }
                    
                    if (!(colonist.Position.DistanceTo(pawn.Position) <= 20))
                    {
                        cachedSeverity = 2f;
                        return cachedSeverity;
                    }

                    cachedSeverity = 3f;
                    return cachedSeverity;
                }
            }
            
            if(pawn.GetCaravan() != null)
            {
                if (Enumerable.Any(pawn.GetCaravan().pawns.InnerListForReading, colonist => colonist.IsSuperHuman()))
                {
                    cachedSeverity = 3f;
                    return cachedSeverity;
                }
            }
                
            cachedSeverity = 1f;
            return cachedSeverity;
        }
    }
}