using System.Linq;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class Hediff_Serf : HediffWithComps
{
    private const int RecalculateInterval = 250;

    private const float NearbyRadius = 20f;

    private float cachedSeverity = 0.1f;

    public override float Severity => cachedSeverity;

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);
        cachedSeverity = CalculateSeverity();
    }

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);

        if (!pawn.IsHashIntervalTick(RecalculateInterval, delta))
        {
            return;
        }

        cachedSeverity = CalculateSeverity();
    }

    /// <summary>
    /// 0 once the serf trait is gone, 0.1 while it is suppressed, 1 with no superhuman around,
    /// 2 with one elsewhere on the map and 3 with one within NearbyRadius cells.
    /// </summary>
    private float CalculateSeverity()
    {
        var trait = pawn.story?.traits?.GetTrait(Genes40kDefOf.BEWH_Serf);

        if (trait == null)
        {
            return 0f;
        }

        if (trait.Suppressed)
        {
            return 0.1f;
        }

        if (pawn.Map != null)
        {
            var superHumanOnMap = false;

            foreach (var colonist in pawn.Map.mapPawns.FreeColonistsSpawned)
            {
                if (pawn == colonist || !colonist.IsSuperHuman())
                {
                    continue;
                }

                if (colonist.Position.DistanceTo(pawn.Position) <= NearbyRadius)
                {
                    return 3f;
                }

                superHumanOnMap = true;
            }

            return superHumanOnMap ? 2f : 1f;
        }

        var caravan = pawn.GetCaravan();

        if (caravan != null && caravan.pawns.InnerListForReading.Any(colonist => colonist.IsSuperHuman()))
        {
            return 3f;
        }

        return 1f;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref cachedSeverity, "cachedSeverity", 0.1f);
    }
}