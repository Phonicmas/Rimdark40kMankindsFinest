using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace Genes40k;

public class Gene_Pariah : Gene
{
    private const int tickInterval = 501;

    private static readonly List<Pawn> tmpAffectedPawns = new();

    private DefModExtension_Pariah cachedDefMod;
    private DefModExtension_Pariah DefMod => cachedDefMod ??= def.GetModExtension<DefModExtension_Pariah>();

    public override void Tick()
    {
        base.Tick();
        if (!pawn.IsHashIntervalTick(tickInterval) || pawn.needs?.mood == null || pawn.Faction == null)
        {
            return;
        }

        var defMod = DefMod;

        if (defMod == null)
        {
            return;
        }

        if (pawn.Spawned)
        {
            var position = pawn.Position;
            var radiusSquared = defMod.radius * defMod.radius;
            var spawned = pawn.Map.mapPawns.AllPawnsSpawned;

            tmpAffectedPawns.Clear();

            for (var i = 0; i < spawned.Count; i++)
            {
                if ((spawned[i].Position - position).LengthHorizontalSquared <= radiusSquared)
                {
                    tmpAffectedPawns.Add(spawned[i]);
                }
            }

            AffectPawns(pawn, tmpAffectedPawns, defMod);
            tmpAffectedPawns.Clear();
            return;
        }

        var caravan = pawn.GetCaravan();
        if (caravan != null)
        {
            AffectPawns(pawn, caravan.pawns.InnerListForReading, defMod);
        }
    }

    private static void AffectPawns(Pawn p, List<Pawn> pawns, DefModExtension_Pariah defMod)
    {
        if (pawns.NullOrEmpty())
        {
            return;
        }

        var pariahHediffDefs = Genes40kUtils.PariahHediffDefs;

        foreach (var affectedPawn in pawns)
        {
            if (affectedPawn == null || p == affectedPawn || !p.RaceProps.Humanlike || affectedPawn.needs?.mood?.thoughts == null || affectedPawn.genes == null || Genes40kUtils.IsPariah(affectedPawn))
            {
                continue;
            }

            Hediff hediff = null;
            var hediffs = affectedPawn.health.hediffSet.hediffs;

            for (var i = 0; i < hediffs.Count; i++)
            {
                if (pariahHediffDefs.Contains(hediffs[i].def))
                {
                    hediff = hediffs[i];
                    break;
                }
            }

            if (hediff != null)
            {
                if (hediff.Severity < defMod.tier)
                {
                    hediff.Severity = defMod.tier;
                    var disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (disappears != null)
                    {
                        disappears.ticksToDisappear = disappears.disappearsAfterTicks;
                    }
                }
                else if (hediff.Severity == defMod.tier)
                {
                    var disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (disappears != null)
                    {
                        disappears.ticksToDisappear = disappears.disappearsAfterTicks;
                    }
                }
            }
            else
            {
                if (affectedPawn.Faction == Faction.OfPlayer)
                {
                    affectedPawn.health.AddHediff(Genes40kDefOf.BEWH_PariahEffecter);
                }
                else
                {
                    affectedPawn.health.AddHediff(Genes40kDefOf.BEWH_PariahEffecterEnemies);
                }
            }
        }
    }
}