using Verse;

namespace Genes40k
{
    public class Gene_TwinConnected : Gene
    {
        private Pawn pawn = null;
        public Pawn Pawn => pawn;
        private bool twinSet = false;
        
        public void SetTwin(Pawn twin)
        {
            if (!twinSet)
            {
                pawn = twin;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref twinSet, "twinSet");
        }
    }
}