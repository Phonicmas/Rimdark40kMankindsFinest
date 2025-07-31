using Core40k;
using Verse;

namespace Genes40k;

public class ShoulderIconSettings : ExtraDecorationSettings
{
    public ShoulderIconDef ShoulderIcon = null;
    
    public override void ExposeData()
    {
        Scribe_Defs.Look(ref ShoulderIcon, "ShoulderIcon");
        base.ExposeData();
    }
}