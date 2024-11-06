using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Ability_PsychicEmp : VFECore.Abilities.Ability
    {
        private void AffectThings()
        {
            foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, GetRadiusForPawn(), useCenter: true))
            {
                if (item.HasComp<CompGlower>() && item.def.PlaceWorkers.Any(p => p is PlaceWorker_GlowRadius))
                {
                    item.TryGetComp<CompGlower>();
                    item.Kill();
                }
                else if (item is Pawn otherPawn && !otherPawn.Dead)
                {
                    if (otherPawn.Faction == Faction.OfPlayer)
                    {
                        continue;
                    }
                    
                    otherPawn.stances.stunner.StunFor(300, CasterPawn);
                }
            }
        }
        
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            AffectThings();
        }
        
        public override void GizmoUpdateOnMouseover()
        {
            var radiusForPawn = GetRadiusForPawn();
            GenDraw.DrawRadiusRing(pawn.Position, radiusForPawn, Color.black);
        }
    }
}