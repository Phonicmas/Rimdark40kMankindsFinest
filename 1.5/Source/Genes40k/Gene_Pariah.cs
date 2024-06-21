using RimWorld.Planet;
using System.Collections.Generic;
using Verse;


namespace Genes40k
{
    public class Gene_Pariah : Gene
    {
        private readonly int tickInterval = 501;

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(tickInterval) || pawn.needs == null || pawn.needs.mood == null || pawn.Faction == null)
            {
                return;
            }
            if (pawn.Spawned)
            {
                List<Pawn> pawnsT = (List<Pawn>)pawn.Map.mapPawns.AllPawnsSpawned;
                List<Pawn> pawns = pawnsT.FindAll(x => pawn.Position.DistanceTo(x.Position) <= def.GetModExtension<DefModExtension_Pariah>().radius);
                AffectPawns(pawn, pawns);
                return;
            }
            Caravan caravan = pawn.GetCaravan();
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
            foreach (Pawn pawn in pawns)
            {
                if (pawn == null || p == pawn || !p.RaceProps.Humanlike || pawn.needs == null || pawn.needs.mood == null || pawn.needs.mood.thoughts == null || pawn.genes == null || Genes40kUtils.IsPariah(pawn))
                {
                    continue;
                }
                DefModExtension_Pariah defMod = def.GetModExtension<DefModExtension_Pariah>();
                Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => x.def.HasModExtension<DefModExtension_Pariah>());
                if (hediff != null)
                {
                    if (hediff.Severity < defMod.tier)
                    {
                        hediff.Severity = defMod.tier;
                        HediffComp_Disappears disappears = hediff.TryGetComp<HediffComp_Disappears>();
                        if (disappears != null)
                        {
                            disappears.ticksToDisappear = disappears.disappearsAfterTicks;
                        }
                    }
                    else if (hediff.Severity == defMod.tier)
                    {
                        HediffComp_Disappears disappears = hediff.TryGetComp<HediffComp_Disappears>();
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