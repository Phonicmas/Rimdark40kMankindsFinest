using System.Collections.Generic;
using System.Linq;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ExtraDecorationTab : ApparelColourTwoTabDrawer
    {
        private const int RowAmount = 6;

        private bool setupDone = false;

        private static float listScrollViewHeight = 0f;
        
        private List<ExtraDecorationDef> extraDecorationDefs = new List<ExtraDecorationDef>();
        
        private void Setup(Pawn pawn)
        {
            extraDecorationDefs = DefDatabase<ExtraDecorationDef>.AllDefs.ToList();
            extraDecorationDefs.SortBy(def => def.sortOrder);
        }
        
        public override void DrawTab(Rect rect, Pawn pawn, ref Vector2 apparelColorScrollPosition)
        {
            if (!setupDone)
            {
                setupDone = true;
                Setup(pawn);
            }
            
            var chapterApparel = (ExtraIconsChapterApparelColourTwo)pawn.apparel.WornApparel.First(a => a is ExtraIconsChapterApparelColourTwo);
            
            GUI.BeginGroup(rect);
            var outRect = new Rect(0f, 0f, rect.width, rect.height);
            var viewRect = new Rect(0f, 0f, rect.width - 16f, listScrollViewHeight);
            Widgets.BeginScrollView(outRect, ref apparelColorScrollPosition, viewRect);
            
            //Extra decoration title
            var nameRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            nameRect.width /= 2;
            nameRect.x += nameRect.width / 2;
            Widgets.DrawMenuSection(nameRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(nameRect, "BEWH.MankindsFinest.ShoulderIcon.LeftShoulder".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            
            var resetChapterIconRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
            resetChapterIconRect.width /= 5;
            resetChapterIconRect.x = nameRect.xMin - resetChapterIconRect.width - nameRect.width/20;
            
            var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
            var position = new Vector2(viewRect.x, resetChapterIconRect.yMax);
            
            var curX = position.x;
            var curY = position.y;
            
            var currentDecorations = chapterApparel.ExtraDecorationDefs;
            
            for (var i = 0; i < extraDecorationDefs.Count; i++)
            {
                position = new Vector2(curX, curY);
                var iconRect = new Rect(position, iconSize);
                
                curX += iconRect.width;

                if (i != 0 && (i+1) % RowAmount == 0)
                {
                    curY += iconRect.height;
                    curX = viewRect.position.x;
                }
                else if (i == extraDecorationDefs.Count - 1)
                {
                    curY += iconRect.height;
                }
                
                iconRect = iconRect.ContractedBy(5f);
                
                if (currentDecorations.ContainsKey(extraDecorationDefs[i]))
                {
                    Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
                }
                
                var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                GUI.color = color;
                GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, extraDecorationDefs[i].Icon);
                
                TooltipHandler.TipRegion(iconRect, extraDecorationDefs[i].label);
                
                if (Widgets.ButtonInvisible(iconRect))
                {
                    chapterApparel.AddOrRemoveDecoration(extraDecorationDefs[i]);
                }
            }
            
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        /*public override void OnClose(bool closeOnCancel, bool closeOnClickedOutside)
        {
            
        }

        public override void OnAccept()
        {
            
        }*/
    }
}   