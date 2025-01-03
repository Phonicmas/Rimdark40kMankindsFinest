using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ShoulderPadIconTab : ApparelColourTwoTabDrawer
    {
        private List<ShoulderIconDef> rightShoulderIcons = new List<ShoulderIconDef>();
        private List<ShoulderIconDef> leftShoulderIcons = new List<ShoulderIconDef>();

        private const int RowAmount = 6;

        private bool setupDone = false;

        private void Setup(Pawn pawn)
        {
            var allShoulderIcons = DefDatabase<ShoulderIconDef>.AllDefs.ToList();
            foreach (var shoulderIcon in allShoulderIcons)
            {
                if (!HasRequirements(shoulderIcon, pawn))
                {
                    continue;
                }
                if (shoulderIcon.leftShoulder)
                {
                    leftShoulderIcons.Add(shoulderIcon);
                }
                if (shoulderIcon.rightShoulder)
                {
                    rightShoulderIcons.Add(shoulderIcon);
                }
            }

            rightShoulderIcons.SortBy(def => def.sortOrder);
            leftShoulderIcons.SortBy(def => def.sortOrder);
        }

        private static bool HasRequirements(ShoulderIconDef shoulderIconDef, Pawn pawn)
        {
            if (shoulderIconDef.mustHaveRank != null)
            {
                if (!pawn.HasComp<CompRankInfo>())
                {
                    return false;
                }
                var comp = pawn.GetComp<CompRankInfo>();
                if (!comp.HasRank(shoulderIconDef.mustHaveRank))
                {
                    return false;
                }
            }
            return true;
        }
        
        public override void DrawTab(Rect rect, Pawn pawn, float viewRectHeight, ref Vector2 apparelColorScrollPosition)
        {
            if (!setupDone)
            {
                setupDone = true;
                Setup(pawn);
            }
            
            var chapterApparel = (ExtraIconsChapterApparelColourTwo)pawn.apparel.WornApparel.First(a => a is ExtraIconsChapterApparelColourTwo);
            
            var viewRect = new Rect(rect.x, rect.y, rect.width - 16f, viewRectHeight);
            Widgets.BeginScrollView(rect, ref apparelColorScrollPosition, viewRect);
            
            //Left shoulder icon title
            var nameRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            nameRect.width /= 2;
            nameRect.x += nameRect.width / 2;
            Widgets.DrawMenuSection(nameRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(nameRect, "BEWH.MankindsFinest.ShoulderIcon.LeftShoulder".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
                
            //Reset left shoulder icon to default
            var resetChapterIconRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            resetChapterIconRect.width /= 5;
            resetChapterIconRect.x = nameRect.xMin - resetChapterIconRect.width - nameRect.width/20;
            if (Widgets.ButtonText(resetChapterIconRect, "BEWH.MankindsFinest.ShoulderIcon.ResetToDefault".Translate()))
            {
                chapterApparel.LeftShoulderIcon = null;
            }
            
            //Left shoulder icons
            var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
            var position = new Vector2(viewRect.x, resetChapterIconRect.yMax);
            
            var curX = position.x;
            var curY = position.y;
            
            for (var i = 0; i < leftShoulderIcons.Count; i++)
            {
                position = new Vector2(curX, curY);
                var iconRect = new Rect(position, iconSize);
                
                curX += iconRect.width;

                if (i != 0 && (i+1) % RowAmount == 0)
                {
                    curY += iconRect.height;
                    curX = viewRect.position.x;
                }
                else if (i == leftShoulderIcons.Count - 1)
                {
                    curY += iconRect.height;
                }
                
                iconRect = iconRect.ContractedBy(5f);
                
                var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                GUI.color = color;
                GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, leftShoulderIcons[i].Icon);
                
                TooltipHandler.TipRegion(iconRect, leftShoulderIcons[i].label);

                if (Widgets.ButtonInvisible(iconRect))
                {
                    chapterApparel.LeftShoulderIcon = leftShoulderIcons[i];
                }
            }

            curY += 34f;

            var rankComp = pawn.GetComp<CompRankInfo>();

            if (rankComp != null && !rankComp.UnlockedRanks.NullOrEmpty())
            {
                //Right Shoulder title
                var nameRect2 = new Rect(viewRect.x, curY, viewRect.width, 30f);
                nameRect2.width /= 2;
                nameRect2.x += nameRect2.width / 2;
            
                Widgets.DrawMenuSection(nameRect2);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(nameRect2, "BEWH.MankindsFinest.ShoulderIcon.RightShoulder".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            
                //Reset right Shoulder to default
                var resetRankIconRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
                resetRankIconRect.width /= 5;
                resetRankIconRect.x = nameRect.xMin - resetRankIconRect.width - nameRect.width/20;
                if (Widgets.ButtonText(resetRankIconRect, "BEWH.MankindsFinest.ShoulderIcon.ResetToDefault".Translate()))
                {
                    chapterApparel.RightShoulderIcon = null;
                }
                
                position = new Vector2(viewRect.x, resetRankIconRect.yMax);
            
                curX = position.x;
                curY = position.y;
                
                //Right Shoulder Icons
                for (var i = 0; i < rightShoulderIcons.Count; i++)
                {
                    position = new Vector2(curX, curY);
                    var iconRect = new Rect(position, iconSize);
                
                    curX += iconRect.width;

                    if (i != 0 && (i+1) % RowAmount == 0)
                    {
                        curY += iconRect.height;
                        curX = viewRect.position.x;
                    }
                
                    iconRect = iconRect.ContractedBy(5f);
                
                    var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                    GUI.color = color;
                    GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                    GUI.color = Color.white;
                    GUI.DrawTexture(iconRect, rightShoulderIcons[i].Icon);
                
                    TooltipHandler.TipRegion(iconRect, rightShoulderIcons[i].label);

                    if (Widgets.ButtonInvisible(iconRect))
                    {
                        chapterApparel.RightShoulderIcon = rightShoulderIcons[i];
                    }
                }
            }
            
            Widgets.EndScrollView();
        }
    }
}   