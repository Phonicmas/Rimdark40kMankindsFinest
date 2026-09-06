using System.Text;
using Core40k;
using Verse;

namespace Genes40k;

public class ChapterRankDef : RankDef
{
    public ShoulderIconDef unlocksRankIcon = null;

    public override void UnlockRank(CompRankInfo rankComp)
    {
        base.UnlockRank(rankComp);
        SyncShoulderIcons(rankComp.ParentPawn);
    }

    public override void RemoveRank(CompRankInfo rankComp)
    {
        base.RemoveRank(rankComp);
        SyncShoulderIcons(rankComp.ParentPawn);
    }

    private static void SyncShoulderIcons(Pawn pawn)
    {
        if (pawn?.apparel == null)
        {
            return;
        }

        foreach (var apparel in pawn.apparel.WornApparel)
        {
            apparel.GetComp<CompChapterColorWithShoulderDecoration>()?.SyncRankIcon(pawn);
        }
    }

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
