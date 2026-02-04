using System.Text;
using Core40k;
using Verse;

namespace Genes40k;

public class ChapterRankDef : RankDef
{
    public ShoulderIconDef unlocksRankIcon = null;

    public override string BuildRankBonusString(StringBuilder stringBuilder)
    {
        var result = base.BuildRankBonusString(stringBuilder);
        if (unlocksRankIcon == null)
        {
            return result;
        }

        var shoulderIconUnlockStringBuilder = new StringBuilder();
        shoulderIconUnlockStringBuilder.AppendLine("    " + unlocksRankIcon.LabelCap);
        var shoulderIconUnlock = "BEWH.MankindsFinest.RankSystem.ShoulderIconUnlock".Translate() + "\n" + shoulderIconUnlockStringBuilder.ToString();
        if (result.NullOrEmpty())
        {
            result = shoulderIconUnlock;
        }
        else
        {
            result += "\n" + shoulderIconUnlock;
        }

        return result;
    }
}