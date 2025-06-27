using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class CompAbilityEffect_AoeHit : CompAbilityEffect
    {
        public new CompProperties_AbilityAoeHit Props => (CompProperties_AbilityAoeHit)props;
        
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var pawn = target.Pawn;
            if (pawn == null)
            {
                return;
            }
            
            if (Props.fleckDefTarget != null)
            {
                FleckMaker.AttachedOverlay(pawn, Props.fleckDefTarget, Vector3.zero);
            }
            
            var damageAmount = Props.damageAmount;

            if (Props.scaleStat != null)
            {
                var stat = parent.pawn.GetStatValue(Props.scaleStat) * Props.scaleFactor;
                damageAmount *= stat;
            }

            var dInfo = new DamageInfo(Props.damageDef, damageAmount);
            
            pawn.TakeDamage(dInfo);
        }
        
        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            if (Props.fleckDefLocation != null)
            {
                FleckMaker.Static(parent.verb.CurrentTarget.Cell, map, Props.fleckDefLocation);
            }
        }
    }
}