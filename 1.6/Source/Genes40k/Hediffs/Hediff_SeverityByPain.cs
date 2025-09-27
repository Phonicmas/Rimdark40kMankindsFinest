using UnityEngine;
using Verse;

namespace Genes40k;

public class Hediff_SeverityByPain : HediffWithComps
{
    private float cachedSeverity = 0.01f;
    
    public override float Severity
    {
        get
        {
            if (!pawn.IsHashIntervalTick(125))
            {
                return cachedSeverity;
            }
            cachedSeverity = CalculatePain() + 0.01f;
            return cachedSeverity;
        }
    }
    
    private float CalculatePain()
    {
        var hediffs = pawn.health.hediffSet.hediffs;
        if (!pawn.RaceProps.IsFlesh || pawn.Dead)
        {
            return 0f;
        }
        var num = 0f;
        foreach (var hediff in hediffs)
        {
            if (hediff == this)
            {
                continue;
            }
            num += hediff.PainOffset;
        }
        if (pawn.genes != null)
        {
            num += pawn.genes.PainOffset;
        }
        if (pawn.story?.traits != null)
        {
            var allTraits = pawn.story.traits.allTraits;
            foreach (var trait in allTraits)
            {
                num += trait.CurrentData.painOffset;
            }
        }
        foreach (var hediff in hediffs)
        {
            if (hediff == this)
            {
                continue;
            }
            num *= hediff.PainFactor;
        }
        if (pawn.genes != null)
        {
            num *= pawn.genes.PainFactor;
        }
        if (pawn.story?.traits != null)
        {
            var allTraits2 = pawn.story.traits.allTraits;
            foreach (var trait in allTraits2)
            {
                num *= trait.CurrentData.painFactor;
            }
        }
        
        return Mathf.Clamp(num, 0f, 1f);
    }
}