using RimWorld.Planet;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class Gene_Pariah : Gene
    {
        private const int tickInterval = 501;

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(tickInterval) || pawn.needs?.mood == null || pawn.Faction == null)
            {
                return;
            }
            if (pawn.Spawned)
            {
                var pawnsT = (List<Pawn>)pawn.Map.mapPawns.AllPawnsSpawned;
                var pawns = pawnsT.FindAll(x => pawn.Position.DistanceTo(x.Position) <= def.GetModExtension<DefModExtension_Pariah>().radius);
                AffectPawns(pawn, pawns);
                return;
            }
            var caravan = pawn.GetCaravan();
            if (caravan != null)
            {
                AffectPawns(pawn, caravan.pawns.InnerListForReading);
            }
        }

        private void AffectPawns(Pawn p, List<Pawn> pawns)
        {
            if (pawns.NullOrEmpty())
            {
                return;
            }
            foreach (var pawn in pawns)
            {
                if (pawn == null || p == pawn || !p.RaceProps.Humanlike || pawn.needs?.mood?.thoughts == null || pawn.genes == null || Genes40kUtils.IsPariah(pawn))
                {
                    continue;
                }
                var defMod = def.GetModExtension<DefModExtension_Pariah>();
                var hediff = pawn.health.hediffSet.hediffs.Find(x => x.def.HasModExtension<DefModExtension_Pariah>());
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
                    pawn.health.AddHediff(Genes40kDefOf.BEWH_PariahEffecter);
                }
            }
        }
    }
}