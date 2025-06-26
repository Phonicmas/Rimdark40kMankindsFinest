using Verse;

namespace Genes40k;

public class Gene_Furybound : Gene
{
    private const int tickInterval = 7200000;
    private int counter = 0;
    private int currentThoughtStage = 0;

    public int CurrentThoughtStage
    {
        get => currentThoughtStage;
        private set => currentThoughtStage = value > 4 ? 4 : value;
    }
        
    public override void Tick()
    {
        base.Tick();
        if (counter < tickInterval)
        {
            counter++;
            return;
        }

        if (!pawn.Spawned)
        {
            return;
        }

        counter = 0;
        CurrentThoughtStage++;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref currentThoughtStage, "currentThoughtStage");
        Scribe_Values.Look(ref counter, "counter");
    }
}