using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

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
    public override float MaxLevelOffset => 1f;

    protected override Color BarColor => new ColorInt(240, 183, 29).ToColor;

    protected override Color BarHighlightColor => new ColorInt(255, 200, 51).ToColor;

    public bool isOvercharging = false;
    public bool overloadRadiance = false;

    public bool passivelyDrainRadiance = false;
    private bool sendMessageOfLowRadiance = true;
        
    public override float Value
    {
        get => cur;
        set
        {
            var maxVal = max;
            if (isOvercharging)
            {
                maxVal += MaxLevelOffset;
            }
            cur = Mathf.Clamp(value, 0f, maxVal);
            if (!overloadRadiance || !(cur > max))
            {
                return;
            }
            
            overloadRadiance = false;
            cur = max + MaxLevelOffset;
        }
    }
        
    public Gene_DivineRadiance()
    {
        SetMax(NewMax);
    }

    public void ChangeDivineRadianceAmount(float amount)
    {
        Value += amount;

        if (Value > Max)
        {
            if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension))
            {
                pawn.health.AddHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension);
            }
        }

        if (Value <= MinLevelForAlert && sendMessageOfLowRadiance)
        {
            Messages.Message("BEWH.MankindsFinest.LivingSaint.LowHolyRadiance".Translate(pawn), MessageTypeDefOf.NegativeEvent, false);
            sendMessageOfLowRadiance = false;
        }
        else
        {
            sendMessageOfLowRadiance = true;
        }
            
        passivelyDrainRadiance = Value < 0.1f;

        if (Value > 0.01f)
        {
            return;
        }
            
        if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_DivineRadiaceFading))
        {
            pawn.health.AddHediff(Genes40kDefOf.BEWH_DivineRadiaceFading);
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (passivelyDrainRadiance && pawn.IsHashIntervalTick(1250))
        {
            ChangeDivineRadianceAmount(-0.01f);
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
                defaultLabel = "DEV: Divine Radiance -30",
                action = delegate
                {
                    ChangeDivineRadianceAmount(-0.3f);
                }
            };
            yield return command_Action;
                
            var command_Action2 = new Command_Action
            {
                defaultLabel = "DEV: Divine Radiance +30",
                action = delegate
                {
                    isOvercharging = true;
                    overloadRadiance = true;
                    ChangeDivineRadianceAmount(0.3f);
                    overloadRadiance = false;
                }
            };
            yield return command_Action2;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref isOvercharging, "isOvercharging", false);
        Scribe_Values.Look(ref overloadRadiance, "overloadRadiance", false);
        Scribe_Values.Look(ref passivelyDrainRadiance, "passivelyDrainRadiance", false);
    }
}