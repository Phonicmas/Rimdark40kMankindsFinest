using System.Collections.Generic;
using System.Linq;
using ColourPicker;
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

        private static float listScrollViewHeight = 0f;

        private void Setup(Pawn pawn)
        {
            var allShoulderIcons = DefDatabase<ShoulderIconDef>.AllDefs.ToList();
            foreach (var shoulderIcon in allShoulderIcons)
            {
                if (!shoulderIcon.HasRequirements(pawn))
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
        
        public override void DrawTab(Rect rect, Pawn pawn, ref Vector2 apparelColorScrollPosition)
        {
            if (!setupDone)
            {
                setupDone = true;
                Setup(pawn);
            }
            
            var chapterApparel = (BodyChapterApparelColourTwo)pawn.apparel.WornApparel.First(a => a is BodyChapterApparelColourTwo);
            
            GUI.BeginGroup(rect);
            var outRect = new Rect(0f, 0f, rect.width, rect.height);
            var viewRect = new Rect(0f, 0f, rect.width - 16f, listScrollViewHeight);
            Widgets.BeginScrollView(outRect, ref apparelColorScrollPosition, viewRect);
            
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
                chapterApparel.LeftShoulderIcon = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>().CurrentlySelectedPreset.relatedChapterIcon ?? null;
            }
            
            var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
            var position = new Vector2(viewRect.x, resetChapterIconRect.yMax);
            
            var curX = position.x;
            var curY = position.y;
            
            //Left icon colour selection if possible.
            /*if (chapterApparel.LeftShoulderIcon != null && chapterApparel.LeftShoulderIcon.useColour)
            {
                var tertiaryColourRect = new Rect(position, new Vector2(viewRect.width, 50f)).ContractedBy(5);
                Widgets.DrawMenuSection(tertiaryColourRect);
                tertiaryColourRect = tertiaryColourRect.ContractedBy(1);
                
                Widgets.DrawRectFast(tertiaryColourRect, chapterApparel.LeftShoulderIconColour);
                if (Widgets.ButtonInvisible(tertiaryColourRect))
                {
                    Find.WindowStack.Add( new Dialog_ColourPicker( chapterApparel.LeftShoulderIconColour, ( newColour ) =>
                    {
                        chapterApparel.LeftShoulderIconColour = newColour;
                    } ) );
                }

                curY = tertiaryColourRect.yMax;
            }*/
            
            //Left icon selection
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
            
            //Right icon colour selection if possible.
            if (chapterApparel.RightShoulderIcon != null && chapterApparel.RightShoulderIcon.useColour)
            {
                var tertiaryColourRect = new Rect(position, new Vector2(viewRect.width, 50f)).ContractedBy(5);
                Widgets.DrawMenuSection(tertiaryColourRect);
                tertiaryColourRect = tertiaryColourRect.ContractedBy(1);
                
                Widgets.DrawRectFast(tertiaryColourRect, chapterApparel.RightShoulderIconColour);
                if (Widgets.ButtonInvisible(tertiaryColourRect))
                {
                    Find.WindowStack.Add( new Dialog_ColourPicker( chapterApparel.RightShoulderIconColour, ( newColour ) =>
                    {
                        chapterApparel.RightShoulderIconColour = newColour;
                    } ) );
                }

                curY = tertiaryColourRect.yMax;
            }
                
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

            listScrollViewHeight = position.y + iconSize.y + 10f;
            
            Widgets.EndScrollView();
            GUI.EndGroup();
        }
    }
}   