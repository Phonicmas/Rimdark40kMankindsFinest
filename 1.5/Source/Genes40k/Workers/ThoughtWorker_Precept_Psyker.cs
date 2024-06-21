using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Precept_Psyker : ThoughtWorker_Precept   
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.genes != null)
            {
                return Genes40kUtils.IsPsyker(p);
            }
            return false;
        }
    }
}