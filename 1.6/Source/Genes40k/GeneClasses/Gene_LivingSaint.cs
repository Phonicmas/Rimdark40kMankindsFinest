using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Genes40k;

public class Gene_LivingSaint : Gene
{
    public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
    {
        base.Notify_PawnDied(dinfo, culprit);
        if (pawn.Faction != Faction.OfPlayer)
        {
            return;
        }
            
        if (pawn.Corpse is { Spawned: true })
        {
            pawn.Corpse.DeSpawn();
        }

        if (!pawn.Spawned && !pawn.Discarded && !Find.WorldPawns.Contains(pawn))
        {
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
        }
    }
}