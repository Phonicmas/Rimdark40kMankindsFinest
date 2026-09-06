using Verse;

namespace Genes40k;

public class DefModExtension_Psyker : DefModExtension
{
    public int naturalBornSelectionWeight = 0;

    //Higher tier wins when a pawn gains a second psyker gene. 0 means unset: falls back to displayOrderInCategory.
    public int tier = 0;
}