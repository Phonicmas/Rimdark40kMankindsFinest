using Verse;

namespace Genes40k;

public class Gene_TwinConnected : Gene
{
    private Pawn twin = null;
    public Pawn Twin => twin;
    private bool twinSet = false;
        
    public void SetTwin(Pawn twinPawn)
    {
        twin = twinPawn;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref twin, "twin");
        Scribe_Values.Look(ref twinSet, "twinSet");
    }
}