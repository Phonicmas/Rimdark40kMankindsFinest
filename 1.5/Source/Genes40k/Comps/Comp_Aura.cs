using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Comp_Aura : ThingComp
    {
        private CompProperties_Aura Props => (CompProperties_Aura)props;

        public override void CompTick()
        {
            base.CompTick();

            if (!parent.IsHashIntervalTick(500))
            {
                return;
            }

            var list = GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, Props.range, true).Where(thing => thing is Pawn pawn && pawn.Faction.IsPlayer);
            
            var things = list.ToList();
            
            if (things.NullOrEmpty())
            {
                return;
            }
            
            foreach (var thing in things)
            {
                if (!(thing is Pawn pawn))
                {
                    continue;
                }
                var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.givesHediff);
                if (firstHediffOfDef != null)
                {
                    pawn.health.RemoveHediff(firstHediffOfDef);
                }
                var hediff = HediffMaker.MakeHediff(Props.givesHediff, pawn, pawn.health.hediffSet.GetBrain());
                var hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                
                if (hediffComp_Disappears != null)
                {
                    hediffComp_Disappears.ticksToDisappear = Props.durationOutsideRange;
                }
                pawn.health.AddHediff(hediff);
            }
        }
        
        public override void PostDraw()
        {
            var cells = GenRadial.RadialCellsAround(parent.Position, 0, Props.range).ToList();
            GenDraw.DrawFieldEdges(cells, Color.green);
        }
    }
}