using Verse;

namespace Genes40k
{
    public class WorkerClass_GeneProgenoidRemovalPrimarisMarine : WorkerClass_GeneProgenoidRemovalBase
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            return base.AvailableOnNow(thing, part) && Genes40kUtils.IsPrimaris((Pawn)thing);
        }

    }

}