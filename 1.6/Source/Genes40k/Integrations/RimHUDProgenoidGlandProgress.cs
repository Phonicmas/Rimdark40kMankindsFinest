#nullable enable
using System;
using System.Text;
using Verse;

namespace Genes40k;

public static class RimHUDProgenoidGlandProgress
{
    public static (string? label, string? value, Func<string>? tooltip, Action? onHover, Action? onClick) GetParameters(Pawn pawn)
    {
        if (pawn.genes == null || !pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ProgenoidGlands))
        {
            return (null, null, null, null, null);
        }
            
        var stringBuilder = new StringBuilder();

        var progenoidGlands = (Gene_ProgenoidGlands)pawn.genes.GetGene(Genes40kDefOf.BEWH_ProgenoidGlands);

        stringBuilder.AppendLine("\n");
            
        if (progenoidGlands.FirstProgenoidGlandHarvested)
        {
            stringBuilder.AppendLine("BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvested".Translate());
        }
        else
        {
            var secondProgenoid = !progenoidGlands.SecondProgenoidGlandHarvested
                ? " " + (string)"BEWH.MankindsFinest.SpaceMarine.SecondGeneseedsHarvestableUponDeath".Translate()
                : string.Empty;

            float ticksLeft = progenoidGlands.TicksUntilHarvestable;
            stringBuilder.AppendLine(ticksLeft > 0
                ? "BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvestableIn".Translate((ticksLeft / 60000).ToString("0.00"), secondProgenoid)
                : "BEWH.MankindsFinest.SpaceMarine.FirstGeneseedsHarvestable".Translate());
                
        }
            
        var label = stringBuilder.ToString().TrimEndNewlines();
        
        return (label, null, null, null, null);
    }
}