using Verse;

namespace Genes40k
{
    public class WorkerClass_GeneProgenoidRemovalPrimarisMarine : WorkerClass_GeneProgenoidRemovalBase
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (base.AvailableOnNow(thing, part))
            {
                return Genes40kUtils.IsPrimaris((Pawn)thing);
            }
            return false;
        }

    }

}