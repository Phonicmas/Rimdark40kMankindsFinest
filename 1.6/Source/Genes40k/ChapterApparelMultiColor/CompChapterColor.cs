using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class CompChapterColor : CompMultiColor
{
    public new CompProperties_ChapterColor Props => (CompProperties_ChapterColor)props;
    
    private Genes40kModSettings modSettings = null;
    protected Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
    
    public override void InitialColors()
    {
        base.InitialColors();
        DrawColor = ModSettings?.chapterColorOne ?? DrawColor;
        DrawColorTwo = ModSettings?.chapterColorTwo ?? DrawColorTwo;
        DrawColorThree = ModSettings?.chapterColorThree ?? DrawColorThree;
    }
}