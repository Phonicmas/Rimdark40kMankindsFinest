using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Precept_Psyker : ThoughtWorker_Precept   
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.genes != null && Genes40kUtils.IsPsyker(p);
        }
    }
}