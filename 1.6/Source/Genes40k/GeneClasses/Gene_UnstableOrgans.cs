using System.Collections.Generic;
using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class Gene_UnstableOrgans : Gene
{
    private static readonly IntRange attemptOrganDecayRange = new (600000, 1800000);
    
    private int tickInterval = attemptOrganDecayRange.RandomInRange;
    
    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);
        if (!pawn.IsHashIntervalTick(tickInterval, delta))
        {
            return;
        }
        
        tickInterval = attemptOrganDecayRange.RandomInRange;

        var availableOrganDecayParts = new List<BodyPartDef>
        {
            BodyPartDefOf.Heart,
            BodyPartDefOf.Lung,
            Genes40kDefOf.Kidney
        };

        foreach (var part in availableOrganDecayParts.InRandomOrder())
        {
            if (!HediffGiverUtility.TryApply(pawn, HediffDefOf.OrganDecay, new[] { part }))
            {
                continue;
            }
            TaleRecorder.RecordTale(TaleDefOf.IllnessRevealed, pawn, HediffDefOf.OrganDecay);
            
            var letterTitle = "BEWH.MankindsFinest.ThunderWarrior.OrganDecayTitle".Translate(pawn.LabelShort);
            var letterText = "BEWH.MankindsFinest.ThunderWarrior.OrganDecayMessage".Translate(pawn.LabelShort, part);
            
            var letter = new StandardLetter 
            {
                def = LetterDefOf.NegativeEvent,
                Label = letterTitle,
                title = letterTitle,
                Text = letterText,
                lookTargets = pawn, 
            };
            Find.LetterStack.ReceiveLetter(letter);

            break;
        }
    }
}