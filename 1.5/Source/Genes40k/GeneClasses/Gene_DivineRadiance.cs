using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class Gene_DivineRadiance : Gene_Resource, IGeneResourceDrain
    {
        public Gene_Resource Resource => this;

        public bool CanOffset => true;

        public Pawn Pawn => pawn;
        
        private const float NewMax = 5f;

        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

        public float ResourceLossPerDay => def.resourceLossPerDay;

        public override float InitialResourceMax => NewMax;

        public override float MinLevelForAlert => 0.1f;
        public override float MaxLevelOffset => 2f;

        protected override Color BarColor => new ColorInt(240, 183, 29).ToColor;

        protected override Color BarHighlightColor => new ColorInt(255, 200, 51).ToColor;

        private bool canOvercharge = false;
        
        public override float Value
        {
            get => cur;
            set
            {
                var maxVal = max;
                if (canOvercharge)
                {
                    maxVal += MaxLevelOffset;
                    canOvercharge = false;
                }
                cur = Mathf.Clamp(value, 0f, maxVal);
            }
        }
        

        public Gene_DivineRadiance()
        {
            SetMax(NewMax);
        }

        public void ChangeDivineRadianceAmount(float amount, bool canOverchargeValue)
        {
            canOvercharge = canOverchargeValue;
            Value += amount;

            if (Value > Max)
            {
                //Add overcharge hediff here that increases consciousness and combat stats.
            }

            if (Value <= MinLevelForAlert)
            {
                //Send alert of low level divine radiance
            }
            
            if (Value > 1f)
            {
                return;
            }
            
            if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_DivineRadiaceFading))
            {
                pawn.health.AddHediff(Genes40kDefOf.BEWH_DivineRadiaceFading);
            }
        }

        public void KilledPawn(Pawn killedPawn)
        {
            var divineDrain = 10f;
            if (killedPawn.kindDef != null)
            {
                divineDrain = killedPawn.kindDef.combatPower / 10;
            }
            Value -= divineDrain;
        }

        public override void SetTargetValuePct(float val)
        {
            targetValue = Mathf.Clamp(val * Max, 0f, Max - MaxLevelOffset);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!Active)
            {
                yield break;
            }
            foreach (var baseGizmo in base.GetGizmos())
            {
                yield return baseGizmo;
            }
            
            if (DebugSettings.ShowDevGizmos)
            {
                var command_Action = new Command_Action
                {
                    defaultLabel = "DEV: Divine Radiance -10",
                    action = delegate
                    {
                        ChangeDivineRadianceAmount(-0.1f, false);
                    }
                };
                yield return command_Action;
                
                var command_Action2 = new Command_Action
                {
                    defaultLabel = "DEV: Divine Radiance +10",
                    action = delegate
                    {
                        ChangeDivineRadianceAmount(0.1f, true);
                    }
                };
                yield return command_Action2;
            }
        }
    }
}