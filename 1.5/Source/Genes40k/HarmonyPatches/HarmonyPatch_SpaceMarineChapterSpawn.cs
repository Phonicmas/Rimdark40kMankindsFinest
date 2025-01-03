using HarmonyLib;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public class SpaceMarineChapterSpawn
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Faction == null || __instance.Faction.IsPlayer)
            {
                return;
            }

            if (__instance.NonHumanlikeOrWildMan())
            {
                return;
            }

            if (__instance.genes == null || !Genes40kUtils.IsFirstborn(__instance))
            {
                return;
            }

            var chapter = Current.Game.GetComponent<GameComponent_MankindFinestUtils>().CurrentChapterColour;
			    
            __instance.genes.AddGene(chapter.relatedChapterGene, true);
            foreach (var apparel in __instance.apparel.WornApparel)
            {
                switch (apparel)
                {
                    case ExtraIconsChapterApparelColourTwo extraIconsChapterApparelColourTwo:
                        extraIconsChapterApparelColourTwo.ApplyColourPreset(chapter);
                        extraIconsChapterApparelColourTwo.LeftShoulderIcon = chapter.relatedChapterIcon;
                        break;
                    case ChapterApparelColourTwo chapterApparelColourTwo:
                        chapterApparelColourTwo.ApplyColourPreset(chapter);
                        break;
                }
            }
        }
    }
}