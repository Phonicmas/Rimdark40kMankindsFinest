using Verse;

namespace Genes40k;

public class WorkerClass_GeneProgenoidRemovalSpaceMarine : WorkerClass_GeneProgenoidRemovalBase
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        return base.AvailableOnNow(thing, part) && ((Pawn)thing).IsFirstborn();
    }
}