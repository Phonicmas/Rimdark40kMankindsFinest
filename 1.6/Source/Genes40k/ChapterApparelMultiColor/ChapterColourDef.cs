using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ChapterColourDef : ColourPresetDef
{
    public GeneDef relatedChapterGene = null;

    public ShoulderIconDef relatedChapterIcon = null;
    
    public Color chapterIconColour = Color.white; 
}