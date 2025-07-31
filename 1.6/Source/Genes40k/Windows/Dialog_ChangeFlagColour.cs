using System.Collections.Generic;
using System.Linq;
using ColourPicker;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Dialog_ChangeFlagColour : Window
{
    private readonly List<ChapterColourDef> chapterColours;

    private ChapterColourDef currentlySelectedPreset;
    private Color currentlySelectedPrimaryColour;
    private Color currentlySelectedSecondaryColour;
    private string currentlySelectedIcon;

    private Texture2D CurrentlySelectedIconTexture => ContentFinder<Texture2D>.Get(currentlySelectedIcon);

    private readonly Building_DecorativeFlag decoFlag;
    
    private readonly Genes40kModSettings modSettings = Genes40kUtils.ModSettings;

    private List<FlagIconDef> flagIcons;
        
    public override Vector2 InitialSize => new (900f, 700f);

    public Dialog_ChangeFlagColour(Building_DecorativeFlag decoFlag)
    {
        this.decoFlag = decoFlag;
        closeOnClickedOutside = true;

        currentlySelectedPreset = decoFlag.currentlySelectedPreset;
        currentlySelectedIcon = decoFlag.FlagInsigniaFilePath;
        currentlySelectedPrimaryColour = decoFlag.DrawColor;
        currentlySelectedSecondaryColour = decoFlag.DrawColorTwo;
        chapterColours = DefDatabase<ChapterColourDef>.AllDefsListForReading;
        
        flagIcons = DefDatabase<FlagIconDef>.AllDefs.OrderBy(flagIcon => flagIcon.sortOrder).ToList();
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

        if (Widgets.ButtonText(defaultChapterButton, "BEWH.MankindsFinest.ModSettings.ColourPreset".Translate(currentlySelectedPreset?.label ?? modSettings.CustomPreset.label)))
        {
            var list = new List<FloatMenuOption>();
            var customMenuOption = new FloatMenuOption("BEWH.MankindsFinest.ModSettings.CustomColour".Translate(), delegate
            {
                currentlySelectedPreset = modSettings.CustomPreset;
                
                currentlySelectedPrimaryColour = currentlySelectedPreset.primaryColour;
                currentlySelectedSecondaryColour = currentlySelectedPreset.secondaryColour;
                currentlySelectedIcon = currentlySelectedPreset.relatedChapterIcon.iconPath;
                
                decoFlag.currentlySelectedPreset = null;
            }, Core40kUtils.ThreeColourPreview(modSettings.CustomPreset.primaryColour, modSettings.CustomPreset.secondaryColour, modSettings.CustomPreset.tertiaryColour), Color.white);
            
            list.Add(customMenuOption);
            foreach (var colour in chapterColours.Where(ccd => ccd.relatedChapterGene != null))
            {
                var menuOption = new FloatMenuOption(colour.label.CapitalizeFirst(), delegate
                {
                    currentlySelectedPreset = colour;
                    
                    currentlySelectedPrimaryColour = currentlySelectedPreset.primaryColour;
                    currentlySelectedSecondaryColour = currentlySelectedPreset.secondaryColour;
                    currentlySelectedIcon = currentlySelectedPreset.relatedChapterIcon.iconPath;
                    
                    decoFlag.currentlySelectedPreset = currentlySelectedPreset;
                }, Core40kUtils.ThreeColourPreview(colour.primaryColour, colour.secondaryColour, colour.tertiaryColour), Color.white);
                list.Add(menuOption);
            }
                
            Find.WindowStack.Add(new FloatMenu(list));
        }
            
        var colourFields = new Rect(inRect);
        colourFields.y = defaultChapterButton.yMax + gap;
        colourFields.height /= 3;
            
        var primaryColorRect = new Rect(colourFields);
        primaryColorRect.width /= 2f;
        primaryColorRect = primaryColorRect.ContractedBy(5);
        primaryColorRect.x = inRect.xMin + 1f;
            
        Widgets.DrawMenuSection(primaryColorRect.ContractedBy(-1));
        Widgets.DrawRectFast(primaryColorRect, currentlySelectedPrimaryColour);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(primaryColorRect, "BEWH.Framework.ApparelMultiColor.PrimaryColor".Translate());
        TooltipHandler.TipRegion(primaryColorRect, "BEWH.Framework.ApparelMultiColor.ChooseCustomColour".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        if (Widgets.ButtonInvisible(primaryColorRect))
        {
            Find.WindowStack.Add( new Dialog_ColourPicker( currentlySelectedPrimaryColour, ( newColour ) =>
            {
                currentlySelectedPrimaryColour = newColour;
                decoFlag.currentlySelectedPreset = null;
            } ) );
        }

        var secondaryColorRect = new Rect(colourFields);
        secondaryColorRect.width /= 2f;
        secondaryColorRect.x = primaryColorRect.xMax;
        secondaryColorRect = secondaryColorRect.ContractedBy(5);
        secondaryColorRect.x = inRect.xMax - secondaryColorRect.width - 1f;
            
        Widgets.DrawMenuSection(secondaryColorRect.ContractedBy(-1));
        Widgets.DrawRectFast(secondaryColorRect, currentlySelectedSecondaryColour);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(secondaryColorRect, "BEWH.Framework.ApparelMultiColor.SecondaryColor".Translate());
        TooltipHandler.TipRegion(secondaryColorRect, "BEWH.Framework.ApparelMultiColor.ChooseCustomColour".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        if (Widgets.ButtonInvisible(secondaryColorRect))
        {
            Find.WindowStack.Add( new Dialog_ColourPicker( currentlySelectedSecondaryColour, ( newColour ) =>
            {
                currentlySelectedSecondaryColour = newColour;
                decoFlag.currentlySelectedPreset = null;
            } ) );
        }
        
        if (decoFlag.currentlySelectedPreset == null)
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

            var viewRectHeight = inRect.yMax - customIconName.yMax - CloseButSize.y;
            var outRect = new Rect(inRect.x, curY, inRect.width, viewRectHeight);
            var viewRect = new Rect(inRect.x, curY, inRect.width - 16f, Mathf.Max(scrollViewHeight, viewRectHeight));
            scrollViewHeight = viewRectHeight;
            
            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
            
            //Show icons here
            var iconSide = viewRect.width / RowAmount;
            var iconSize = new Vector2(iconSide, iconSide);
            var position = new Vector2(viewRect.x, curY);
            
            var curX = position.x;
            
            var rowsMade = 1;
            for (var i = 0; i < flagIcons.Count; i++)
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
            
                if (currentlySelectedIcon == flagIcons[i].iconPath)
                {
                    Widgets.DrawStrongHighlight(iconRect.ExpandedBy(3f));
                }
            
                var color = Mouse.IsOver(iconRect) ? GenUI.MouseoverColor : Color.white;
                GUI.color = color;
                GUI.DrawTexture(iconRect, Command.BGTexShrunk);
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, flagIcons[i].Icon);
                
                TooltipHandler.TipRegion(iconRect, flagIcons[i].label);

                if (Widgets.ButtonInvisible(iconRect))
                {
                    currentlySelectedIcon = flagIcons[i].iconPath;
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
            decoFlag.SetPrimaryColor(currentlySelectedPrimaryColour);
            decoFlag.SetSecondaryColor(currentlySelectedSecondaryColour);
            decoFlag.SetFlagInsignia(currentlySelectedIcon);
            decoFlag.SetOriginals();
            decoFlag.Notify_ColorChanged();
            _ = decoFlag.Graphic;
            Close();
        }
            
        if (Widgets.ButtonText(new Rect(inRect.xMin, inRect.yMax - CloseButSize.y, CloseButSize.x, CloseButSize.y), "Close".Translate()))
        {
            decoFlag.Reset();
            Close();
        }
    }
    

    public override void Notify_ClickOutsideWindow()
    {
        decoFlag.Reset();
        base.Notify_ClickOutsideWindow();
    }
}