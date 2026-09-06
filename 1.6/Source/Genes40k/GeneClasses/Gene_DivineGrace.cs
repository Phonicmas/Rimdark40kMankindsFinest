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

    private const int HediffCheckInterval = 60;

    private const int RadianceInterval = 250;

    private const float RadianceRadius = 10f;

    private const float RadianceMoodThreshold = 0.5f;

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
        ChangeDivineGraceAmount(amount, true);
    }

    /// <summary>
    /// Per-tick callers (the ascension hediff comp) pass periodic so the hediff scans only run on a
    /// hash interval; one-shot callers keep checking immediately.
    /// </summary>
    public void ChangeDivineGraceAmountPeriodic(float amount)
    {
        ChangeDivineGraceAmount(amount, pawn.IsHashIntervalTick(HediffCheckInterval));
    }

    private void ChangeDivineGraceAmount(float amount, bool checkHediffs)
    {
        Value += amount;

        if (Value > MinLevelForAlert)
        {
            sendMessageOfLowGrace = true;
        }
        else if (sendMessageOfLowGrace)
        {
            Messages.Message("BEWH.MankindsFinest.LivingSaint.LowHolyGrace".Translate(pawn), MessageTypeDefOf.NegativeEvent, false);
            sendMessageOfLowGrace = false;
        }

        passivelyDrainGrace = Value < 0.1f;

        if (!checkHediffs)
        {
            return;
        }

        if (Value > Max && !pawn.health.hediffSet.HasHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension))
        {
            pawn.health.AddHediff(Genes40kDefOf.BEWH_LivingSaintHolyAscension);
        }

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

        if (pawn.IsHashIntervalTick(RadianceInterval))
        {
            GrantHolyRadiance();
        }
    }

    /// <summary>
    /// Gives nearby acquaintances the holy radiance mood memory. Done once from the saint's side on an
    /// interval instead of from every observer's social-thought query.
    /// </summary>
    private void GrantHolyRadiance()
    {
        if (Value <= RadianceMoodThreshold || !pawn.Spawned || !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
        {
            return;
        }

        var position = pawn.Position;
        var radiusSquared = RadianceRadius * RadianceRadius;
        var spawned = pawn.Map.mapPawns.AllPawnsSpawned;

        for (var i = 0; i < spawned.Count; i++)
        {
            var other = spawned[i];

            if (other == pawn || other.genes == null || other.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintHolyRadiance))
            {
                continue;
            }

            if ((other.Position - position).LengthHorizontalSquared > radiusSquared)
            {
                continue;
            }

            var memories = other.needs?.mood?.thoughts?.memories;

            if (memories == null || !RelationsUtility.PawnsKnowEachOther(other, pawn))
            {
                continue;
            }

            memories.TryGainMemoryFast(Genes40kDefOf.BEWH_LivingSaintHolyRadianceThought);
        }
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
        Scribe_Values.Look(ref sendMessageOfLowGrace, "sendMessageOfLowGrace", true);
    }
}