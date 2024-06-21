using Verse;


namespace Genes40k
{
    public class Gene_LivingSaint : Gene
    {
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            GameComponent_LivingSaint gComp = Current.Game.GetComponent<GameComponent_LivingSaint>();
            gComp.AddSaintToSpawnable(pawn);
        }
    }
}