using System;
using Verse;

namespace Genes40k;

public class Gene_Furybound : Gene
{
    private const int tickInterval = 15000;
    private const float percentChanceIncrease = 2.5f;
    public float percentChance = 0;
        
    public override void Tick()
    {
        base.Tick();
        if (!pawn.IsHashIntervalTick(tickInterval))
        {
            return;
        }

        if (!pawn.Spawned || pawn.Downed || pawn.Crawling)
        {
            return;
        }

        percentChance += percentChanceIncrease;
            
        var random = new Random();
        if (random.Next(0, 100) > percentChance)
        {
            return;
        }
        
        percentChance = 0;

        //TODO: fix by changing def to some sort of short berserk, or manipulate tick remaining on normal berserk
        def.mentalBreakDef.Worker.TryStart(pawn, "MentalStateReason_Gene".Translate() + ": " + LabelCap, causedByMood: false);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref percentChance, "percentChance");
    }
}