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
        
        private List<ExtraDecorationDef> extraDecorationDefsBody = new List<ExtraDecorationDef>();
        private List<ExtraDecorationDef> extraDecorationDefsHelmet = new List<ExtraDecorationDef>();
        
        private void Setup(Pawn pawn)
        {
            var allExtraDecorations = DefDatabase<ExtraDecorationDef>.AllDefs.ToList();
            foreach (var extraDecoration in allExtraDecorations)
            {
                if (!extraDecoration.HasRequirements(pawn))
                {
                    continue;
                }
                if (extraDecoration.isHelmetDecoration)
                {
                    extraDecorationDefsHelmet.Add(extraDecoration);
                }
                else
                {
                    extraDecorationDefsBody.Add(extraDecoration);
                }
            }

            extraDecorationDefsBody.SortBy(def => def.sortOrder);
            extraDecorationDefsHelmet.SortBy(def => def.sortOrder);
        }
        
        public override void DrawTab(Rect rect, Pawn pawn, ref Vector2 apparelColorScrollPosition)
        {
            if (!setupDone)
            {
                setupDone = true;
                Setup(pawn);
            }
            
            GUI.BeginGroup(rect);
            var outRect = new Rect(0f, 0f, rect.width, rect.height);
            var viewRect = new Rect(0f, 0f, rect.width - 16f, listScrollViewHeight);
            Widgets.BeginScrollView(outRect, ref apparelColorScrollPosition, viewRect);
            
            var bodyApparel = (BodyChapterApparelColourTwo)pawn.apparel.WornApparel.FirstOrFallback(a => a is BodyChapterApparelColourTwo);

            var curX = viewRect.x;
            var curY = viewRect.y;
            
            if (bodyApparel != null)
            {
                //Extra decoration title
                var nameRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
                nameRect.width /= 2;
                nameRect.x += nameRect.width / 2;
                Widgets.DrawMenuSection(nameRect);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(nameRect, "BEWH.MankindsFinest.ExtraDecoration.BodyDecoration".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                
                var resetChapterIconRect = new Rect(viewRect.x, viewRect.y, viewRect.width, 30f);
                resetChapterIconRect.width /= 5;
                resetChapterIconRect.x = nameRect.xMin - resetChapterIconRect.width - nameRect.width/20;
                
                var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
                var position = new Vector2(viewRect.x, resetChapterIconRect.yMax);
                
                curX = position.x;
                curY = position.y;
                
                var currentDecorations = bodyApparel.ExtraDecorationDefs;
                
                for (var i = 0; i < extraDecorationDefsBody.Count; i++)
                {
                    position = new Vector2(curX, curY);
                    var iconRect = new Rect(position, iconSize);
                    
                    curX += iconRect.width;

                    if (i != 0 && (i+1) % RowAmount == 0)
                    {
                        curY += iconRect.height;
                        curX = viewRect.position.x;
                    }
                    else if (i == extraDecorationDefsBody.Count - 1)
                    {
                        curY += iconRect.height;
                    }
                    
                    iconRect = iconRect.ContractedBy(5f);
                    
                    if (currentDecorations.ContainsKey(extraDecorationDefsBody[i]))
                    {
                        Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
                    }
                    
                    var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                    GUI.color = color;
                    GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                    GUI.color = Color.white;
                    GUI.DrawTexture(iconRect, extraDecorationDefsBody[i].Icon);
                    
                    TooltipHandler.TipRegion(iconRect, extraDecorationDefsBody[i].label);
                    
                    if (Widgets.ButtonInvisible(iconRect))
                    {
                        bodyApparel.AddOrRemoveDecoration(extraDecorationDefsBody[i]);
                    }
                }
            }
            
            var helmetApparel = (HelmetChapterApparelColourTwo)pawn.apparel.WornApparel.FirstOrFallback(a => a is HelmetChapterApparelColourTwo);

            if (helmetApparel != null)
            {
                //Extra decoration title
                var nameRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
                nameRect.width /= 2;
                nameRect.x += nameRect.width / 2;
                Widgets.DrawMenuSection(nameRect);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(nameRect, "BEWH.MankindsFinest.ExtraDecoration.HelmetDecoration".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                
                var resetChapterIconRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
                resetChapterIconRect.width /= 5;
                resetChapterIconRect.x = nameRect.xMin - resetChapterIconRect.width - nameRect.width/20;
                
                var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
                var position = new Vector2(viewRect.x, resetChapterIconRect.yMax);
                
                curX = position.x;
                curY = position.y;
                
                var currentDecorations = helmetApparel.ExtraDecorationDefs;
                
                for (var i = 0; i < extraDecorationDefsHelmet.Count; i++)
                {
                    position = new Vector2(curX, curY);
                    var iconRect = new Rect(position, iconSize);
                    
                    curX += iconRect.width;

                    if (i != 0 && (i+1) % RowAmount == 0)
                    {
                        curY += iconRect.height;
                        curX = viewRect.position.x;
                    }
                    else if (i == extraDecorationDefsHelmet.Count - 1)
                    {
                        curY += iconRect.height;
                    }
                    
                    iconRect = iconRect.ContractedBy(5f);
                    
                    if (currentDecorations.ContainsKey(extraDecorationDefsHelmet[i]))
                    {
                        Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
                    }
                    
                    var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                    GUI.color = color;
                    GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                    GUI.color = Color.white;
                    GUI.DrawTexture(iconRect, extraDecorationDefsHelmet[i].Icon);
                    
                    TooltipHandler.TipRegion(iconRect, extraDecorationDefsHelmet[i].label);
                    
                    if (Widgets.ButtonInvisible(iconRect))
                    {
                        helmetApparel.AddOrRemoveDecoration(extraDecorationDefsHelmet[i]);
                    }
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