using RimWorld;
using Verse;

namespace Genes40k;

public class DefModExtension_DropPod : DefModExtension
{
    public bool usePlayerColours = false;
    
    public ChapterColourDef chapterColour;

    public int dropPodAmount = 1;
        
    public int marinesToSpawn = 4;

    public FactionDef fromFaction;

    public string openGraphic;
        
    public string openGraphicMask;
        
    public SoundDef openSound;

    public ThingDef skyFaller;

    public ThingDef innerThing;
}