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
        
    public override Vector2 InitialSize => new Vector2(900f, 700f);

    public Dialog_ChangeDefaultChapterColour(Genes40kModSettings settings)
    {
        this.settings = settings;
        closeOnClickedOutside = true;
            
        currentlySelectedPreset = settings.CurrentlySelectedPreset;
        if (currentlySelectedPreset == null)
        {
            currentlySelectedPreset = settings.CustomPreset;
        }
        chapterColours = DefDatabase<ChapterColourDef>.AllDefs.ToList();
    }
        
    public override void DoWindowContents(Rect inRect)
    {
        const float gap = 5f;
        inRect.yMax -= CloseButSize.y;
            
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
            foreach (var colour in chapterColours)
            {
                var menuOption = new FloatMenuOption(colour.label.CapitalizeFirst(), delegate
                {
                    currentlySelectedPreset = colour;
                }, Core40kUtils.TwoColourPreview(colour.primaryColour, colour.secondaryColour), Color.white);
                list.Add(menuOption);
            }
                
            if (!list.NullOrEmpty())
            {
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }
            
        var colourFields = new Rect(inRect);
        colourFields.height -= defaultChapterButton.yMax + gap;
        colourFields.y = defaultChapterButton.yMax + gap;
            
            
        var primaryColorRect = new Rect(colourFields);
        primaryColorRect.width /= 2f;
        primaryColorRect = primaryColorRect.ContractedBy(5);
        primaryColorRect.x = inRect.xMin + 1f;
            
        Widgets.DrawMenuSection(primaryColorRect.ContractedBy(-1));
        Widgets.DrawRectFast(primaryColorRect, currentlySelectedPreset.primaryColour);
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
        if (Widgets.ButtonInvisible(secondaryColorRect))
        {
            Find.WindowStack.Add( new Dialog_ColourPicker( currentlySelectedPreset.secondaryColour, ( newColour ) =>
            {
                settings.CustomPreset.secondaryColour = newColour;
                settings.CustomPreset.primaryColour = currentlySelectedPreset.primaryColour;
                currentlySelectedPreset = settings.CustomPreset;
            } ) );
        }
            
            
        if (Widgets.ButtonText(new Rect(inRect.xMax - CloseButSize.x, inRect.yMax, CloseButSize.x, CloseButSize.y), "Close".Translate()))
        {
            Close();
        }
            
        if (Widgets.ButtonText(new Rect(inRect.xMin, inRect.yMax, CloseButSize.x, CloseButSize.y), "Accept".Translate()))
        {
            settings.chapterColorOne = currentlySelectedPreset.primaryColour;
            settings.chapterColorTwo = currentlySelectedPreset.secondaryColour;
            settings.CurrentlySelectedPreset = currentlySelectedPreset;
            Close();
        }
    }
}