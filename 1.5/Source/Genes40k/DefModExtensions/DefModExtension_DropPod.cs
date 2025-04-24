using RimWorld;
using Verse;

namespace Genes40k;

public class DefModExtension_DropPod : DefModExtension
{
    public bool usePlayerColours = false;

    public int dropPodAmount = 1;
        
    public int marinesToSpawn = 4;

    public FactionDef fromFaction;

    public string openGraphic;
        
    public string openGraphicMask;
        
    public SoundDef openSound;
}