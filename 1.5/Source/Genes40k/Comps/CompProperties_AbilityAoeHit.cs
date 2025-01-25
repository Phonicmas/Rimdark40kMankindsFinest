using RimWorld;
using Verse;


namespace Genes40k
{
    public class CompProperties_AbilityAoeHit : CompProperties_AbilityEffect
    {
        public DamageDef damageDef = null;
        
        public float damageAmount = 0f;

        public StatDef scaleStat = null;
        
        public float scaleFactor = 1f;
        
        public FleckDef fleckDefLocation = null;
        
        public FleckDef fleckDefTarget = null;
        
        public CompProperties_AbilityAoeHit()
        {
            compClass = typeof(CompAbilityEffect_AoeHit);
        }
    }
}