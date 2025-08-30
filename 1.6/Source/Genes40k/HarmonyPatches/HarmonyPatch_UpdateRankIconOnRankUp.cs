using Core40k;
using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(CompRankInfo), "UnlockRank")]
public class UpdateRankIconOnRankUp
{
    public static void Postfix(CompRankInfo __instance, RankDef rank)
    {
        if (rank.rankCategory != Genes40kDefOf.BEWH_AstartesRankCategory)
        {
            return;
        }

        if (__instance.parent is not Pawn pawn)
        {
            return;
        }

        if (rank.rankTier < __instance.HighestRank(Genes40kDefOf.BEWH_AstartesRankCategory))
        {
            return;
        }
        
        var apparel = pawn.apparel.WornApparel.FirstOrFallback(a =>
        {
            var temp = a.GetComp<CompChapterColorWithShoulderDecoration>();
            return temp != null;
        });
        
        apparel?.Notify_ColorChanged();
    }
}