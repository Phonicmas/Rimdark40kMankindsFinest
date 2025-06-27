using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Gene_DivineGrace : Gene_Resource, IGeneResourceDrain
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
    public bool overloadGrace = false;

    public bool passivelyDrainGrace = false;
    private bool sendMessageOfLowGrace = true;
        
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
            if (!overloadGrace || !(cur > max))
            {
                return;
            }
            
            overloadGrace = false;
            cur = max + MaxLevelOffset;
        }
    }
        
    public Gene_DivineGrace()
    {
        SetMax(NewMax);
    }

    public void ChangeDivineGraceAmount(float amount)
    {
        Value += amount;

        if (Value > Max)
        {
            if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension))
            {
                pawn.health.AddHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension);
            }
        }

        if (Value <= MinLevelForAlert && sendMessageOfLowGrace)
        {
            Messages.Message("BEWH.MankindsFinest.LivingSaint.LowHolyGrace".Translate(pawn), MessageTypeDefOf.NegativeEvent, false);
            sendMessageOfLowGrace = false;
        }
        else
        {
            sendMessageOfLowGrace = true;
        }
            
        passivelyDrainGrace = Value < 0.1f;

        if (Value > 0.01f)
        {
            return;
        }
            
        if (!pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_DivineGraceFading))
        {
            pawn.health.AddHediff(Genes40kDefOf.BEWH_DivineGraceFading);
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (passivelyDrainGrace && pawn.IsHashIntervalTick(1250))
        {
            ChangeDivineGraceAmount(-0.01f);
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
                defaultLabel = "DEV: Divine Grace -30",
                action = delegate
                {
                    ChangeDivineGraceAmount(-0.3f);
                }
            };
            yield return command_Action;
                
            var command_Action2 = new Command_Action
            {
                defaultLabel = "DEV: Divine Grace +30",
                action = delegate
                {
                    isOvercharging = true;
                    overloadGrace = true;
                    ChangeDivineGraceAmount(0.3f);
                    overloadGrace = false;
                }
            };
            yield return command_Action2;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref isOvercharging, "isOvercharging", false);
        Scribe_Values.Look(ref overloadGrace, "overloadGrace", false);
        Scribe_Values.Look(ref passivelyDrainGrace, "passivelyDrainGrace", false);
    }
}