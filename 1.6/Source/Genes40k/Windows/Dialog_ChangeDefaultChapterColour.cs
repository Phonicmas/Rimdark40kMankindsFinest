using System.Collections.Generic;
using System.Linq;
using ColourPicker;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Dialog_ChangeDefaultChapterColour : Window
{
    private Genes40kModSettings settings;
        
    private List<ChapterColourDef> chapterColours;

    private ChapterColourDef currentlySelectedPreset;
    private Texture2D CurrentlySelectedIconTexture => ContentFinder<Texture2D>.Get(currentlySelectedPreset.relatedChapterIcon.iconPath);
        
    public override Vector2 InitialSize => new (900f, 700f);

    public Dialog_ChangeDefaultChapterColour(Genes40kModSettings settings)
    {
        this.settings = settings;
        closeOnClickedOutside = true;
            
        currentlySelectedPreset = settings.CurrentlySelectedPreset ?? settings.CustomPreset;
        chapterColours = DefDatabase<ChapterColourDef>.AllDefs.ToList();
    }
    
    private Vector2 scrollPos;
    private float scrollViewHeight;
    
    private const int RowAmount = 6;
    private const float gap = 5f;
        
    public override void DoWindowContents(Rect inRect)
    {
        inRect.xMin += 50f;
        inRect.xMax -= 50f;
            
        var defaultChapterButton = new Rect(inRect);
        defaultChapterButton.height = 40f;
        defaultChapterButton.width /= 2;
        defaultChapterButton.x += defaultChapterButton.width / 2;

        if (Widgets.ButtonText(defaultChapterButton, "BEWH.MankindsFinest.ModSettings.ColourPreset".Translate(currentlySelectedPreset.label)))
        {
            var list = new List<FloatMenuOption>();
            var customMenuOption = new FloatMenuOption("BEWH.MankindsFinest.ModSettings.CustomColour".Translate(), delegate
            {
                currentlySelectedPreset = settings.CustomPreset;
            }, Core40kUtils.TwoColourPreview(settings.CustomPreset.primaryColour, settings.CustomPreset.secondaryColour), Color.white);
            
            list.Add(customMenuOption);
            foreach (var colour in chapterColours.Where(ccd => ccd.relatedChapterGene != null))
            {
                var menuOption = new FloatMenuOption(colour.label.CapitalizeFirst(), delegate
                {
                    currentlySelectedPreset = colour;
                    settings.chapterShoulderIconColor = null;
                }, Core40kUtils.TwoColourPreview(colour.primaryColour, colour.secondaryColour), Color.white);
                list.Add(menuOption);
            }
                
            if (!list.NullOrEmpty())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }
            
        var colourFields = new Rect(inRect);
        colourFields.y = defaultChapterButton.yMax + gap;
        colourFields.height /= 3;
            
        var primaryColorRect = new Rect(colourFields);
        primaryColorRect.width /= 2f;
        primaryColorRect = primaryColorRect.ContractedBy(5);
        primaryColorRect.x = inRect.xMin + 1f;
            
        Widgets.DrawMenuSection(primaryColorRect.ContractedBy(-1));
        Widgets.DrawRectFast(primaryColorRect, currentlySelectedPreset.primaryColour);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(primaryColorRect, "BEWH.Framework.ApparelColourTwo.PrimaryColor".Translate());
        TooltipHandler.TipRegion(primaryColorRect, "BEWH.Framework.ApparelColourTwo.ChooseCustomColour".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        if (Widgets.ButtonInvisible(primaryColorRect))
        {
            Find.WindowStack.Add( new Dialog_ColourPicker( currentlySelectedPreset.primaryColour, ( newColour ) =>
            {
                settings.CustomPreset.primaryColour = newColour;
                settings.CustomPreset.secondaryColour = currentlySelectedPreset.secondaryColour;
                currentlySelectedPreset = settings.CustomPreset;
            } ) );
        }

        var secondaryColorRect = new Rect(colourFields);
        secondaryColorRect.width /= 2f;
        secondaryColorRect.x = primaryColorRect.xMax;
        secondaryColorRect = secondaryColorRect.ContractedBy(5);
        secondaryColorRect.x = inRect.xMax - secondaryColorRect.width - 1f;
            
        Widgets.DrawMenuSection(secondaryColorRect.ContractedBy(-1));
        Widgets.DrawRectFast(secondaryColorRect, currentlySelectedPreset.secondaryColour);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(secondaryColorRect, "BEWH.Framework.ApparelColourTwo.SecondaryColor".Translate());
        TooltipHandler.TipRegion(secondaryColorRect, "BEWH.Framework.ApparelColourTwo.ChooseCustomColour".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        if (Widgets.ButtonInvisible(secondaryColorRect))
        {
            Find.WindowStack.Add( new Dialog_ColourPicker( currentlySelectedPreset.secondaryColour, ( newColour ) =>
            {
                settings.CustomPreset.secondaryColour = newColour;
                settings.CustomPreset.primaryColour = currentlySelectedPreset.primaryColour;
                currentlySelectedPreset = settings.CustomPreset;
            } ) );
        }
        
        if (currentlySelectedPreset.defName == "BEWH_CustomChapterDef")
        {
            var customIconName = new Rect(colourFields)
            {
                height = defaultChapterButton.height,
                width = defaultChapterButton.width,
                x = defaultChapterButton.x,
                y = colourFields.yMax + gap,
            };
            
            Widgets.DrawMenuSection(customIconName);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(customIconName, "Custom Icon");
            Text.Anchor = TextAnchor.UpperLeft;
            
            var curY = customIconName.yMax + gap;

            if (settings.CurrentlySelectedPreset.relatedChapterIcon != null && settings.CurrentlySelectedPreset.relatedChapterIcon.useColour)
            {
                var customIconColour = new Rect(customIconName)
                {
                    y = customIconName.yMax + gap
                };
                
                Widgets.DrawMenuSection(customIconColour);
                customIconColour = customIconColour.ContractedBy(1);
                
                Widgets.DrawRectFast(customIconColour, settings.CurrentlySelectedPreset.chapterIconColour);
                if (Widgets.ButtonInvisible(customIconColour))
                {
                    Find.WindowStack.Add( new Dialog_ColourPicker( settings.CurrentlySelectedPreset.chapterIconColour, ( newColour ) =>
                    {
                        settings.CurrentlySelectedPreset.chapterIconColour = newColour;
                        settings.chapterShoulderIconColor = newColour;
                    } ) );
                }

                curY = customIconColour.yMax;
            }

            var viewRectHeight = inRect.yMax - customIconName.yMax - CloseButSize.y - CloseButSize.y;
            var outRect = new Rect(inRect.x, curY, inRect.width, viewRectHeight);
            var viewRect = new Rect(inRect.x, curY, inRect.width - 16f, Mathf.Max(scrollViewHeight, viewRectHeight));
            scrollViewHeight = viewRectHeight;
            
            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
            
            //Show icons here
            var iconSide = viewRect.width / RowAmount;
            var iconSize = new Vector2(iconSide, iconSide);
            var smallIconSize = new Vector2(iconSize.x / 4, iconSize.y / 4);
            var position = new Vector2(viewRect.x, curY);
            
            var curX = position.x;
            
            var shoulderIcons = Genes40kUtils.LeftShoulderIconDef;
            var rowsMade = 1;
            for (var i = 0; i < shoulderIcons.Count; i++)
            {
                position = new Vector2(curX, curY);
                var iconRect = new Rect(position, iconSize);
                
                curX += iconRect.width;

                if (i != 0 && (i+1) % RowAmount == 0)
                {
                    curY += iconRect.height;
                    curX = viewRect.position.x;
                    rowsMade++;
                }
                
                iconRect = iconRect.ContractedBy(5f);
            
                if (settings.CustomPreset.relatedChapterIcon == shoulderIcons[i])
                {
                    Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
                }
            
                var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                GUI.color = color;
                GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, shoulderIcons[i].Icon);
                
                if (shoulderIcons[i].useColour)
                {
                    var flippedIconRect = new Rect(new Vector2(position.x + 7f, position.y + 5f), smallIconSize);
                    GUI.DrawTexture(flippedIconRect, Genes40kUtils.PaintableIcon.Texture);
                }
                
                TooltipHandler.TipRegion(iconRect, shoulderIcons[i].label);

                if (Widgets.ButtonInvisible(iconRect))
                {
                    settings.chapterShoulderIcon = shoulderIcons[i];
                    settings.chapterShoulderIconColor = shoulderIcons[i].defaultColour;
                    settings.CustomPreset.relatedChapterIcon = shoulderIcons[i];
                }
            }
            

            scrollViewHeight = (rowsMade * iconSide) + 5f;
            
            Widgets.EndScrollView();
        }
        else
        {
            var iconSide = inRect.width / 3;
            
            var iconRect = new Rect(colourFields)
            {
                height = iconSide,
                width = iconSide,
                x = inRect.x + iconSide,
                y = colourFields.yMax + gap * 4,
            };
            
            GUI.DrawTexture(iconRect, Command.BGTexShrunk);
            GUI.DrawTexture(iconRect, CurrentlySelectedIconTexture);
        }
        
        if (Widgets.ButtonText(new Rect(inRect.xMax - CloseButSize.x, inRect.yMax - CloseButSize.y, CloseButSize.x, CloseButSize.y), "Accept".Translate()))
        {
            settings.chapterColorOne = currentlySelectedPreset.primaryColour;
            settings.chapterColorTwo = currentlySelectedPreset.secondaryColour;
            settings.CurrentlySelectedPreset = currentlySelectedPreset;
            settings.chapterShoulderIconColor = currentlySelectedPreset.chapterIconColour;
            Close();
        }
            
        if (Widgets.ButtonText(new Rect(inRect.xMin, inRect.yMax - CloseButSize.y, CloseButSize.x, CloseButSize.y), "Close".Translate()))
        {
            Close();
        }
    }
}