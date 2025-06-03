using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Dialog_EditFlag : Window
{
    private Building_DecorativeFlag target;

    private Vector2 scrollPosition;

    public override Vector2 InitialSize => new (736f, 700f);
    
    private List<FlagIconDef> shoulderIcons;
    
    private const int RowAmount = 6;

    private static float listScrollViewHeight = 0f;
    
    public Dialog_EditFlag(Building_DecorativeFlag target)
    {
        this.target = target;
        closeOnClickedOutside = true;
        shoulderIcons = DefDatabase<FlagIconDef>.AllDefs.OrderBy(flagIcon => flagIcon.sortOrder).ToList();
    }

    public override void DoWindowContents(Rect inRect)
    {
        GUI.BeginGroup(inRect);
        var outRect = new Rect(0f, 0f, inRect.width, inRect.height);
        var viewRect = new Rect(0f, 0f, inRect.width - 16f, listScrollViewHeight);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

        var curY = inRect.yMin + 5f;
        
        var nameRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
        nameRect.width /= 2;
        nameRect.x += nameRect.width / 2;
        Widgets.DrawMenuSection(nameRect);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(nameRect, "BEWH.MankindsFinest.Decorations.FlagInsignia".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        
        var iconSize = new Vector2(viewRect.width/RowAmount, viewRect.width/RowAmount);
        var position = new Vector2(viewRect.x, nameRect.yMax);
            
        var curX = position.x;
        curY = position.y;
        
        for (var i = 0; i < shoulderIcons.Count; i++)
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
            
            if (target.FlagInsigniaFilePath == shoulderIcons[i].iconPath)
            {
                Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
            }
            
            var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
            GUI.color = color;
            GUI.DrawTexture(iconRect, Command.BGTexShrunk);
            GUI.color = Color.white;
            GUI.DrawTexture(iconRect, shoulderIcons[i].Icon);
                
            TooltipHandler.TipRegion(iconRect, shoulderIcons[i].label);

            if (Widgets.ButtonInvisible(iconRect))
            {
                target.SetFlagInsignia(shoulderIcons[i].iconPath, shoulderIcons[i].setsNull);
            }
        }
        
        listScrollViewHeight = position.y + iconSize.y + 10f;
            
        Widgets.EndScrollView();
        GUI.EndGroup();
    }
}