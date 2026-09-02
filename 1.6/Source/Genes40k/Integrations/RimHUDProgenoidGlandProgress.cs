#nullable enable
using System;
using Verse;

namespace Genes40k;

public static class RimHUDProgenoidGlandProgress
{
    public static (string? label, string? value, Func<string>? tooltip, Action? onHover, Action? onClick) GetParameters(Pawn pawn)
    {
        var line = Genes40kUtils.ProgenoidProgressLine(pawn);

        if (line == null)
        {
            return (null, null, null, null, null);
        }

        return (line, null, null, null, null);
    }
}