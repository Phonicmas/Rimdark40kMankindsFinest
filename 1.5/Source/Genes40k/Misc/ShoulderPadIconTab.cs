using System;
using System.Collections.Generic;
using System.Linq;
using Core40k;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Genes40k
{
    public class ShoulderPadIconTab : ApparelColourTwoTabDrawer
    {
        private List<ChapterColourDef> presets = DefDatabase<ChapterColourDef>.AllDefs.ToList();

        private const int RowAmount = 6;
        
        public override void DrawTab(Rect rect, Pawn pawn, float viewRectHeight, ref Vector2 apparelColorScrollPosition)
        {
            var chapterApparel = (ChapterApparelColourTwo)pawn.apparel.WornApparel.First(a => a is ChapterApparelColourTwo);
            
            var viewRect = new Rect(rect.x, rect.y, rect.width - 16f, viewRectHeight);
            Widgets.BeginScrollView(rect, ref apparelColorScrollPosition, viewRect);
            
            //Chapter icon title
            var nameRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            nameRect.width /= 2;
            nameRect.x += nameRect.width / 2;
            Widgets.DrawMenuSection(nameRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(nameRect, "BEWH.ChapterIcons".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
                
            //Reset chapter icon to default
            var resetChapterIconRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            resetChapterIconRect.width /= 5;
            resetChapterIconRect.x = nameRect.xMin - resetChapterIconRect.width - nameRect.width/20;
            if (Widgets.ButtonText(resetChapterIconRect, "BEWH.ResetToDefault".Translate()))
            {
                chapterApparel.CurrentlySelectedChapterIcon = null;
            }
            
            //Chapter icons themselves;
            var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
            var position = new Vector2(resetChapterIconRect.x, resetChapterIconRect.yMax);
            
            var curX = position.x;
            var curY = position.y;
            
            for (var i = 0; i < presets.Count; i++)
            {
                position = new Vector2(curX, curY);
                var iconRect = new Rect(position, iconSize);
                
                curX += iconRect.width;

                if (i != 0 && (i+1) % RowAmount == 0)
                {
                    curY += iconRect.height;
                    curX = resetChapterIconRect.position.x;
                }
                
                iconRect = iconRect.ContractedBy(5f);
                
                var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                GUI.color = color;
                GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, presets[i].Icon);
                
                TooltipHandler.TipRegion(iconRect, presets[i].label);

                if (Widgets.ButtonInvisible(iconRect))
                {
                    chapterApparel.CurrentlySelectedChapterIcon = presets[i];
                }
            }

            curY += 10f;

            var rankComp = pawn.GetComp<CompRankInfo>();

            if (rankComp != null && !rankComp.UnlockedRanks.NullOrEmpty())
            {
                //Rank icon title
                var nameRect2 = new Rect(viewRect.x, curY, viewRect.width, 30f);
                nameRect2.width /= 2;
                nameRect2.x += nameRect2.width / 2;
            
                Widgets.DrawMenuSection(nameRect2);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(nameRect2, "BEWH.RankIcons".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            
                //Reset rank icon to default
                var resetRankIconRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
                resetRankIconRect.width /= 5;
                resetRankIconRect.x = nameRect.xMin - resetRankIconRect.width - nameRect.width/20;
                if (Widgets.ButtonText(resetRankIconRect, "BEWH.ResetToDefault".Translate()))
                {
                    chapterApparel.OverrideRankIcon = null;
                }
                
                position = new Vector2(resetRankIconRect.x, resetRankIconRect.yMax);
            
                curX = position.x;
                curY = position.y;

                var allRanks = rankComp.UnlockedRanks.Cast<ChapterRankDef>().Where(def => def.unlocksRankIconPath != string.Empty).ToList();
                
                //Rank icon themselves
                for (var i = 0; i < allRanks.Count; i++)
                {
                    position = new Vector2(curX, curY);
                    var iconRect = new Rect(position, iconSize);
                
                    curX += iconRect.width;

                    if (i != 0 && (i+1) % RowAmount == 0)
                    {
                        curY += iconRect.height;
                        curX = resetChapterIconRect.position.x;
                    }
                
                    iconRect = iconRect.ContractedBy(5f);
                
                    var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                    GUI.color = color;
                    GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                    GUI.color = Color.white;
                    GUI.DrawTexture(iconRect, allRanks[i].RankIcon);
                
                    TooltipHandler.TipRegion(iconRect, allRanks[i].label);

                    if (Widgets.ButtonInvisible(iconRect))
                    {
                        chapterApparel.OverrideRankIcon = allRanks[i].unlocksRankIconPath;
                    }
                }
            }
            
            Widgets.EndScrollView();
        }
    }
}   