using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Precept_Psyker_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (otherPawn.genes != null)
            {
                return Genes40kUtils.IsPsyker(otherPawn);
            }
            return false;
        }
    }
}