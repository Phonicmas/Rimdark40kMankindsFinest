using RimWorld;
using Verse;


namespace Genes40k
{
    public class ThoughtWorker_Precept_Psyker_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return otherPawn.genes != null && otherPawn.IsPsyker();
        }
    }
}