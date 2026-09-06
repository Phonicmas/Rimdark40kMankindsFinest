using Core40k;
using Verse;

namespace Genes40k;

//Only read back from pre-1.1.34 saves, where the two shoulder slots lived on the chapter comp.
public class ShoulderIconSettings : DecorationSettings
{
    public ShoulderIconDef ShoulderIcon = null;

    public ShoulderIconSettings()
    {
    }
    
    public override void ExposeData()
    {
        Scribe_Defs.Look(ref ShoulderIcon, "ShoulderIcon");
        base.ExposeData();
    }
}
